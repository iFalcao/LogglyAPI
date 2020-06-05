using LogglyAPI.Models;
using System.Threading.Tasks;

namespace LogglyAPI.Contracts
{
    public interface ILogglyClient
    {
        Task<SearchResult> Search(string queryString);

        Task<SearchResult> Search(
            string queryString,
            DateParameter from = null,
            DateParameter until = null,
            SearchOrder? order = SearchOrder.DESC,
            int? size = 50);

        Task<SearchResult> Search(
            string queryString,
            string from = "-24h",
            string until = "now",
            SearchOrder? order = SearchOrder.DESC,
            int? size = 50);
    }
}
