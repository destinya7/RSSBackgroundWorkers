namespace RSSCollectorService.Services
{
    public interface IWorkerQueuePublisher
    {
        void PublishUrl(string url);
    }
}
