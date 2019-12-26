using System;

namespace RSSFetcherService.Config
{
    public class ConfigurationManager : IConfigurationManager
    {
        private const string WorkerQueueUsername = "RSS_WQ_USER";
        private const string WorkerQueuePassword = "RSS_WQ_PASS";
        private const string WorkerQueueHostname = "RSS_WQ_HOST";
        private const string WorkerQueuePort = "RSS_WQ_PORT";
        private const string WorkerQueueName = "RSS_WQ_NAME";

        public QueueVariable GetWorkerQueueEnvironmentVariable()
        {
            return new QueueVariable
            {
                Hostname = GetEnvironmentVariable(WorkerQueueHostname),
                Port = GetEnvironmentVariable(WorkerQueuePort),
                Username = GetEnvironmentVariable(WorkerQueueUsername),
                Password = GetEnvironmentVariable(WorkerQueuePassword),
                QueueName = GetEnvironmentVariable(WorkerQueueName)
            };
        }

        private string GetEnvironmentVariable(string message)
        {
            return Environment.GetEnvironmentVariable(message);
        }
    }
}
