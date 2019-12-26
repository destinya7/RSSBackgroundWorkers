using System;
using RabbitMQ.Client;

namespace RSSCollectorService.Services
{
    public class WorkerQueuePublisher : IWorkerQueuePublisher
    {
        private IConnection _connection;
        private IModel _channel;

        private ILoggerService _logger;

        public WorkerQueuePublisher()
        {

        }

        public void SetupConnection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "128.199.155.230",
                UserName = "dev",
                Password = "dev123"
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.QueueDeclare(queue: "message_queue",
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

        public void PublishUrl(string url)
        {
            throw new System.NotImplementedException();
        }
    }
}
