using Kinvo.Utilities.Extensions;
using LogglyAPI.Configuration;
using LogglyAPI.Contracts;
using LogglyAPI.Errors;
using LogglyAPI.Helpers;
using LogglyAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LogglyAPI.Services
{
    public class LogglyClient : ILogglyClient
    {
        private readonly LogglyConfig _config;
        private readonly string BaseUrl;

        public LogglyClient(LogglyConfig config)
        {
            _config = config;

            BaseUrl = $"https://{_config.Account}.loggly.com/apiv2";
        }

        public async Task<SearchResult> Search(
            string queryString,
            DateParameter from = null,
            DateParameter until = null,
            SearchOrder? order = SearchOrder.DESC,
            int? pageSize = 50)
        {
            from = from ?? DateParameter.DEFAULT_FROM;
            until = until ?? DateParameter.DEFAULT_UNTIL;
            var requestParameters = GenerateRequestParameters(queryString, from.Query, until.Query, order.Value, pageSize.Value);
            var requestUrl = this.BaseUrl + $"/search{requestParameters}";

            try
            {
                using (var webClient = GetConfiguredWebClient())
                {
                    var json = await webClient.DownloadStringTaskAsync(requestUrl);
                    var searchResult = JsonConvert.DeserializeObject<SearchResult>(json);
                    return searchResult;
                }
            }
            catch (WebException webException)
            {
                var statusCode = (webException.Response as HttpWebResponse).StatusCode;

                if (statusCode == HttpStatusCode.ServiceUnavailable)
                    throw new RateLimitException();
                if (statusCode == HttpStatusCode.InternalServerError || statusCode == HttpStatusCode.GatewayTimeout)
                    throw new TimeoutException();

                throw;
            }
        }

        public async Task<IEnumerable<T>> GetRawEventsByRsid<T>(long rsidId, int page = 0)
        {
            var requestUrl = BaseUrl + $"/events?rsid={rsidId}&page={page}&format=raw";

            using (var webClient = GetConfiguredWebClient())
            {
                var rawText = await webClient.DownloadStringTaskAsync(requestUrl);
                var json = this.FixRawEventsText(rawText);
                var eventsResult = JsonConvert.DeserializeObject<List<T>>(json);
                return eventsResult;
            }
        }

        public async Task<EventsResult> GetEventsByRsid(long rsidId, int page = 0)
        {
            var requestUrl = BaseUrl + $"/events?rsid={rsidId}&page={page}";

            using (var webClient = GetConfiguredWebClient())
            {
                var json = await webClient.DownloadStringTaskAsync(requestUrl);
                var eventsResult = JsonConvert.DeserializeObject<EventsResult>(json);
                return eventsResult;
            }
        }

        public async Task<EventsIteratorResult> GetEventsIterator(
            string queryString,
            DateParameter from = null,
            DateParameter until = null,
            SearchOrder? order = SearchOrder.DESC,
            int? pageSize = 50)
        {
            if (pageSize > 1000)
                throw new ArgumentException("Page size must be less than 1000");

            var requestParameters = GenerateRequestParameters(queryString, from.Query, until.Query, order.Value, pageSize.Value);
            var requestUrl = this.BaseUrl + $"/events/iterate{requestParameters}";

            using (var webClient = GetConfiguredWebClient())
            {
                var json = await webClient.DownloadStringTaskAsync(requestUrl);
                var eventsIteratorResult = JsonConvert.DeserializeObject<EventsIteratorResult>(json);
                return eventsIteratorResult;
            }
        }

        #region Private methods

        private string FixRawEventsText(string rawText)
        {
            var fixedJsonResponse = $"[{rawText}]";
            return fixedJsonResponse.Replace("\n", ", ");
        }

        private string GenerateRequestParameters(
            string queryString,
            string from,
            string until,
            SearchOrder order = SearchOrder.DESC,
            int size = 50)
        {
            var requestParameters = $"?q={queryString}";
            requestParameters += $"&from={from}";
            requestParameters += $"&until={until}";
            requestParameters += $"&order={order.GetDescription()}";
            requestParameters += $"&size={size}";

            return requestParameters;
        }

        private WebClient GetConfiguredWebClient()
        {
            var webClient = new WebClient();
            var authString = $"{_config.Username}:{_config.Password}";
            authString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authString));
            webClient.Headers.Add("Authorization", $"Basic {authString}");
            return webClient;
        }

        #endregion
    }
}
