using LogglyAPI.Helpers;
using LogglyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogglyAPI.Contracts
{
    public interface ILogglyClient
    {
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
        Task<SearchResult> Search(
            string queryString,
            DateParameter from = null,
            DateParameter until = null,
            SearchOrder? order = SearchOrder.DESC,
            int? size = 50);



        Task<EventsResult> GetEvents(long searchId, int page = 0);

        Task<IEnumerable<T>> GetRawEvents<T>(long searchId, int page = 0);
    }
}
