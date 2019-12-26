namespace RSSCollectorService.Services
{
    public interface IWorkerQueuePublisher
    {
        void SetupConnection();

        void PublishUrl(string url);
    }
}
