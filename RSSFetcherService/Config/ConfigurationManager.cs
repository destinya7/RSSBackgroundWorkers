using System;

namespace RSSFetcherService.Config
{
    public class ConfigurationManager : IConfigurationManager
    {
        private const string WorkerQueueUsername = "wq_user";
        private const string WorkerQueuePassword = "wq_pass";
        private const string WorkerQueueHostname = "wq_host";
        private const string WorkerQueuePort = "wq_port";

        public ServiceVariable GetWorkerQueueEnvironmentVariable()
        {
            return new ServiceVariable
            {
                Hostname = GetEnvironmentVariable(WorkerQueueHostname),
                Port = GetEnvironmentVariable(WorkerQueuePort),
                Username = GetEnvironmentVariable(WorkerQueueUsername),
                Password = GetEnvironmentVariable(WorkerQueuePassword)
            };
        }

        private string GetEnvironmentVariable(string message)
        {
            return Environment.GetEnvironmentVariable(message);
        }
    }
}
