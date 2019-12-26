namespace RSSFetcherService.Config
{
    public interface IConfigurationManager
    {
        QueueVariable GetWorkerQueueEnvironmentVariable();

        QueueVariable GetMessageQueueEnvironmentVariable();
    }
}
