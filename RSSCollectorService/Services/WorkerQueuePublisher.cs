using System;
using System.Text;
using RabbitMQ.Client;
using RSSFetcherService.Config;

namespace RSSCollectorService.Services
{
    public class WorkerQueuePublisher : IWorkerQueuePublisher
    {
        private IConnection _connection;
        private IModel _channel;

        private ILoggerService _logger;
        private IAppConfigManager _configurationManager;

        private QueueVariable _credentials;

        public WorkerQueuePublisher(
            ILoggerService logger,
            IAppConfigManager configurationManager
        )
        {
            _logger = logger;
            _configurationManager = configurationManager;
        }

        public void SetupConnection()
        {
            _credentials =
                _configurationManager.GetWorkerQueueEnvironmentVariable();
            var factory = new ConnectionFactory()
            {
                HostName = _credentials.Hostname,
                UserName = _credentials.Username,
                Password = _credentials.Password
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.QueueDeclare(queue: _credentials.QueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;
            }
            catch (Exception e)
            {
                _logger.Debug(e.ToString());
            }
        }

        public void PublishMessage(string message)
        {
            byte[] body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(
                exchange: "",
                routingKey: _credentials.QueueName,
                basicProperties: null,
                body: body);
        }
    }
}
