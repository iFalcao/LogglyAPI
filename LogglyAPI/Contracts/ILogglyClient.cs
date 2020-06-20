using LogglyAPI.Models;
using System.Threading.Tasks;

namespace LogglyAPI.Contracts
{
    public interface ILogglyClient
    {
        Task<SearchResult> Search(
            string queryString,
            DateParameter from = null,
            DateParameter until = null,
            SearchOrder? order = SearchOrder.DESC,
            int? size = 50);
    }
}
