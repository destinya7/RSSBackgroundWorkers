namespace RSSFetcherService.Config
{
    public interface IAppConfigManager
    {
        QueueVariable GetWorkerQueueEnvironmentVariable();

        QueueVariable GetMessageQueueEnvironmentVariable();

        string GetLogSourceName();
    }
}
