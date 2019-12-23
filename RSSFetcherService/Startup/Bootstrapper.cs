using System;
using Autofac;
using RSSBackgroundWorkerBusiness.DAL;
using RSSBackgroundWorkerBusiness.Repositories;
using RSSFetcherService.Core;
using RSSFetcherService.Utils;

namespace RSSFetcherService.Startup
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

            containerBuilder.RegisterType<RSSParser>()
                .As<IRSSParser>();
            containerBuilder.RegisterType<FetcherCore>()
                .As<IFetcherCore>();

            containerBuilder.RegisterType<RSSFetcherService>().AsSelf();

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
