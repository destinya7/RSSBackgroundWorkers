using RSSBackgroundWorkerBusiness.Models;

namespace RSSFetcherService.Utils
{
    public interface IRSSParser
    {
        Channel ParseRSS(string url);
    }
}
