using Newtonsoft.Json;

namespace RSSFetcherService.Utils
{
    public class MessageConverter<T> : IMessageConverter<T>
        where T : class
    {
        public T DeserializeJson(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string SerializeJson(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
