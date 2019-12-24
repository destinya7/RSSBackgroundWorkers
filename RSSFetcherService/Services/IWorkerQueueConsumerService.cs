﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RSSFetcherService.Services
{
    public interface IWorkerQueueConsumerService
    {
        void SetupConnection();

        EventingBasicConsumer Consumer { get; }

        IModel Channel { get; }

        IConnection Connection { get; }
    }
}
