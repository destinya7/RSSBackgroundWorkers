using System.Threading.Tasks;

namespace RSSFetcherService.Services
{
    public interface IHttpRSSClient
    {
        Task<string> GetRSSXmlString(string url);
    }
}
