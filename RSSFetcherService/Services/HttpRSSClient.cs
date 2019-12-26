using System;
using System.Net;
using System.Threading.Tasks;

namespace RSSFetcherService.Services
{
    public class HttpRSSClient : IHttpRSSClient
    {
        public async Task<string> GetRSSXmlString(string url)
        {
            WebClient client = new WebClient();
            string xmlString;

            try
            {
                xmlString = await client.DownloadStringTaskAsync(url);
            }
            catch (Exception e)
            {
                throw e;
            }

            return xmlString;
        }
    }
}
