namespace RSSFetcherService.Services
{
    public interface IMessageQueuePublisherService
    {
        void SetupConnection();

        void CloseConnection();

        void PublishMessage(string message);
    }
}
