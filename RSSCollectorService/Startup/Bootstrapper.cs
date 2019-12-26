﻿using System;
using Autofac;
using RSSBackgroundWorkerBusiness.DAL;
using RSSBackgroundWorkerBusiness.Repositories;
using RSSCollectorService.Core;
using RSSCollectorService.Services;

namespace RSSCollectorService.Startup
{
    public class Bootstrapper
    {
        private static IContainer _container;

        public static void Bootstrap()
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<RSSContext>().AsSelf();
            containerBuilder.RegisterType<ChannelRepository>()
                .As<IChannelRepository>();
            containerBuilder.RegisterType<ArticleRepository>()
                .As<IArticleRepository>();

            containerBuilder.RegisterType<WorkerQueuePublisher>()
                .As<IWorkerQueuePublisher>();
            containerBuilder.RegisterType<LoggerService>()
                .As<ILoggerService>();

            containerBuilder.RegisterType<CollectorCore>()
                .As<ICollectorCore>();

            containerBuilder.RegisterType<RSSCollectorService>().AsSelf();

            _container = containerBuilder.Build();
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public static object Resolve(Type objectType)
        {
            return _container.Resolve(objectType);
        }
    }
}
