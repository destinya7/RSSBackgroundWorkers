using System;
using System.Text;
using RabbitMQ.Client;
using RSSFetcherService.Config;

namespace RSSFetcherService.Services
{
    class MessageQueuePublisherService : IMessageQueuePublisherService
    {
        private IConnection _connection;
        private IModel _channel;

        private ILoggerService _logger;
        private IConfigurationManager _configurationManager;

        public MessageQueuePublisherService(
            ILoggerService logger,
            IConfigurationManager configurationManager
        )
        {
            _logger = logger;
            _configurationManager = configurationManager;
        }

        public void SetupConnection()
        {
            var credential =
                _configurationManager.GetWorkerQueueEnvironmentVariable();

            var factory = new ConnectionFactory()
            {
                HostName = credential.Hostname,
                UserName = credential.Username,
                Password = credential.Password
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.QueueDeclare(queue: credential.QueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            }
            catch (Exception e)
            {
                _logger.Debug(e.ToString());
            }
        }

        public void CloseConnection()
        {
            if (!_channel.IsClosed) _channel.Close();

            if (_connection != null) _connection.Close();
        }

        public void PublishMessage(string message)
        {
            byte[] body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(
                exchange: "", 
                routingKey: "message_queue", 
                basicProperties: null, 
                body: body);
        }
    }
}
