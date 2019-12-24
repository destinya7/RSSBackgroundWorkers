using System.Threading.Tasks;

namespace RSSFetcherService.Utils
{
    public interface IHttpRSSClient
    {
        Task<string> GetRSSXmlString(string url);
    }
}
