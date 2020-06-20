using Kinvo.Utilities.Extensions;
using LogglyAPI.Contracts;
using LogglyAPI.Errors;
using LogglyAPI.Models;
using Newtonsoft.Json;
using System;
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
            this._config = config;

            this.BaseUrl = $"https://{this._config.Account}.loggly.com/apiv2";
        }

        /// <summary>
        /// Creates a query for Loggly's search endpoint with the specified query string and date parameters
        /// </summary>
        /// <param name="queryString">query that will be applied to the Loggly API, follows: https://documentation.solarwinds.com/en/Success_Center/loggly/Content/admin/search-query-language.htm</param></param>
        /// <param name="from">Defaults to 24h ago</param>
        /// <param name="until">Defaults to now</param>
        /// <param name="order">Order results either as DESC or ASC. Defaults to: DESC</param>
        /// <param name="size">Amount of logs returned. Defaults to: 50</param>
        /// <returns>The search object with a RSID that can be used on the Event's endpoint</returns>
        /// <exception cref="RateLimitException">You've reached the API requests limit</exception>
        /// <exception cref="TimeoutException">Loggly couldn't handle the request in time, you need to retry the search</exception>
        public async Task<SearchResult> Search(
            string queryString,
            DateParameter from = null,
            DateParameter until = null,
            SearchOrder? order = SearchOrder.DESC,
            int? size = 50)
        {
            from = from ?? DateParameter.DEFAULT_FROM;
            until = until ?? DateParameter.DEFAULT_UNTIL;
            var requestUrl = this.GenerateRequestUrl(queryString, from.Query, until.Query, order.Value, size.Value);

            try
            {
                using (var webClient = this.GetConfiguredWebClient())
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

        #region Private methods

        private string GenerateRequestUrl(
            string queryString,
            string from,
            string until,
            SearchOrder order = SearchOrder.DESC,
            int size = 50)
        {
            var requestUrl = this.BaseUrl + $"/search?{queryString}";
            requestUrl += $"&from={from}";
            requestUrl += $"&until={until}";
            requestUrl += $"&order={order.GetDescription()}";
            requestUrl += $"&size={size}";

            return requestUrl;
        }

        private WebClient GetConfiguredWebClient()
        {
            var webClient = new WebClient();
            var authString = $"{this._config.Username}:{this._config.Password}";
            authString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authString));
            webClient.Headers.Add("Authorization", $"Basic {authString}");
            return webClient;
        }

        #endregion
    }
}
