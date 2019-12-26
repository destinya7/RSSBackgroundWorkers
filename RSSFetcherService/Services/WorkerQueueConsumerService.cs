﻿using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RSSFetcherService.Services
{
    public class WorkerQueueConsumerService : IWorkerQueueConsumerService
    {
        ILoggerService _logger;
        private IConnection _connection;
        private IModel _channel;
        private EventingBasicConsumer _consumer;

        public WorkerQueueConsumerService(ILoggerService logger)
        {
            _logger = logger;
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

                _channel.QueueDeclare(queue: "worker_queue1",
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
