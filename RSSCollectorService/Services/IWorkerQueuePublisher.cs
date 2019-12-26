namespace RSSCollectorService.Services
{
    public interface IWorkerQueuePublisher
    {
        void SetupConnection();

        void PublishMessage(string message);
    }
}
