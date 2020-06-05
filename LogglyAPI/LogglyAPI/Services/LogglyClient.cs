using Kinvo.Utilities.Extensions;
using LogglyAPI.Contracts;
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
        private readonly string BaseUrl;
        public readonly string UserName;
        public readonly string Password;
        public readonly string Account;

        public LogglyClient(LogglyConfig config)
        {
            this.UserName = config.Username;
            this.Password = config.Password;
            this.Account = config.Account;

            this.BaseUrl = $"https://{this.Account}.loggly.com/apiv2";
        }

        public async Task<SearchResult> Search(string queryString)
        {
            return await this.Search(queryString,
                                     DateParameter.DEFAULT_FROM,
                                     DateParameter.DEFAULT_UNTIL);
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
            return await this.Search(queryString, from.Query, until.Query, order, size);
        }

        public async Task<SearchResult> Search(
            string queryString,
            string from = "-24h",
            string until = "now",
            SearchOrder? order = SearchOrder.DESC,
            int? size = 50)
        {
            var requestUrl = this.GenerateRequestUrl(queryString, from, until, order.Value, size.Value);
            using (var webClient = this.GetConfiguredWebClient())
            {
                var json = await webClient.DownloadStringTaskAsync(requestUrl);
                var jsonObject = new { rsid = (SearchResult)null };
                return JsonConvert.DeserializeAnonymousType(json, jsonObject).rsid;
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
            var authString = $"{this.UserName}:{this.Password}";
            authString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(authString));
            webClient.Headers.Add("Authorization", $"Basic {authString}");
            return webClient;
        }

        #endregion
    }
}
