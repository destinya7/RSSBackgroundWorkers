namespace RSSFetcherService.Services
{
    public interface IMessageQueuePublisherService
    {
        void SetupConnection();

        void PublishMessage(string message);
    }
}
