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
            int? size = 50)
        {
            from = from ?? DateParameter.DEFAULT_FROM;
            until = until ?? DateParameter.DEFAULT_UNTIL;
            var requestUrl = GenerateRequestUrl(queryString, from.Query, until.Query, order.Value, size.Value);

            try
            {
                using (var webClient = GetConfiguredWebClient())
                {
                    var json = await webClient.DownloadStringTaskAsync(requestUrl);
                    var jsonObject = new { rsid = (SearchResult)null };
                    return JsonConvert.DeserializeAnonymousType(json, jsonObject).rsid;
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

        public async Task<IEnumerable<T>> GetRawEvents<T>(long searchId, int page = 0)
        {
            var requestUrl = BaseUrl + $"/events?rsid={searchId}&page={page}&format=raw";

            using (var webClient = GetConfiguredWebClient())
            {
                var rawText = await webClient.DownloadStringTaskAsync(requestUrl);
                var json = this.FixRawEventsText(rawText);
                var eventsResult = JsonConvert.DeserializeObject<List<T>>(json);
                return eventsResult;
            }
        }

        public async Task<EventsResult> GetEvents(long searchId, int page = 0)
        {
            var requestUrl = BaseUrl + $"/events?rsid={searchId}&page={page}";

            using (var webClient = GetConfiguredWebClient())
            {
                var json = await webClient.DownloadStringTaskAsync(requestUrl);
                var eventsResult = JsonConvert.DeserializeObject<EventsResult>(json);
                return eventsResult;
            }
        }

        #region Private methods

        private string FixRawEventsText(string rawText)
        {
            var fixedJsonResponse = $"[{rawText}]";
            return fixedJsonResponse.Replace("\n", ", ");
        }

        private string GenerateRequestUrl(
            string queryString,
            string from,
            string until,
            SearchOrder order = SearchOrder.DESC,
            int size = 50)
        {
            var requestUrl = BaseUrl + $"/search?{queryString}";
            requestUrl += $"&from={from}";
            requestUrl += $"&until={until}";
            requestUrl += $"&order={order.GetDescription()}";
            requestUrl += $"&size={size}";

            return requestUrl;
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
