namespace RSSFetcherService.Services
{
    public interface IMessageQueuePublisherService
    {
        void PublishMessage(string message);
    }
}
