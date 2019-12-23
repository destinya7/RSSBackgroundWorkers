using RSSBackgroundWorkerBusiness.Models;
using System.Threading.Tasks;

namespace RSSFetcherService.Core
{
    public interface IFetcherCore
    {
        Task<Channel> fetchChannel(string url);
    }
}
