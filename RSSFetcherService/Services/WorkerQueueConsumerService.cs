using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RSSFetcherService.Config;

namespace RSSFetcherService.Services
{
    public class WorkerQueueConsumerService : IWorkerQueueConsumerService
    {
        private IConnection _connection;
        private IModel _channel;
        private EventingBasicConsumer _consumer;

        ILoggerService _logger;
        IConfigurationManager _configurationManager;

        public WorkerQueueConsumerService(
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
                _channel.BasicQos(0, 1, false);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;

                _consumer = new EventingBasicConsumer(_channel);
            }
            catch (Exception e)
            {
                _logger.Debug(e.ToString());
            }
        }

        public void StartListening()
        {
            _channel.BasicConsume(queue: "worker_queue1",
                                  autoAck: false,
                                  consumer: _consumer);
        }

        public void CloseConnection()
        {
            if (!_channel.IsClosed) _channel.Close();
            if (_connection != null) _connection.Close();
        }

        public EventingBasicConsumer Consumer => _consumer;

        public IModel Channel => _channel;

        public IConnection Connection => _connection;
    }
}
