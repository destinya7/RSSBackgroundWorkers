using RSSBackgroundWorkerBusiness.Models;

namespace RSSFetcherService.Utils
{
    public interface IMessageConverter<T> where T : class
    {
        string SerializeJson(T obj);

        T DeserializeJson(string json);
    }
}
