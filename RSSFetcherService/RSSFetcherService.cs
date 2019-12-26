using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using RabbitMQ.Client.Events;
using RSSBackgroundWorkerBusiness.Models;
using RSSFetcherService.Core;
using RSSFetcherService.Services;
using RSSFetcherService.Utils;

namespace RSSFetcherService
{
    public partial class RSSFetcherService : ServiceBase
    {
        private IWorkerQueueConsumerService _consumerService;
        private IFetcherCore _fetcherCore;
        private ILoggerService _logger;
        private IMessageQueuePublisherService _publisherService;
        private IArticleMessageConverter _articleMessageConverter;

        public RSSFetcherService(
            IWorkerQueueConsumerService consumerService,
            IFetcherCore fetcherCore,
            ILoggerService logger,
            IMessageQueuePublisherService publisherService,
            IArticleMessageConverter articleMessageConverter
        )
        {
            InitializeComponent();

            _consumerService = consumerService;
            _fetcherCore = fetcherCore;
            _logger = logger;
            _publisherService = publisherService;
            _articleMessageConverter = articleMessageConverter;
        }

        protected override void OnStart(string[] args)
        {
            _logger.Debug("Service Start " + DateTime.Now);
            SetupConnectionToQueues();
        }

        protected override void OnStop()
        {
            _logger.Debug("Service Stopped " + DateTime.Now);
            CloseConnectionToQueues();
        }

        private async void OnMessageReceived(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
            var message = Encoding.UTF8.GetString(body);

            try
            {
                _logger.Debug($"Message Received: {message}. Fetching channel");

                Channel channel = await _fetcherCore.FetchChannel(message);

                _logger.Debug($"Done Fetching Channel");

                _consumerService.Channel.BasicAck(e.DeliveryTag, false);

                _logger.Debug($"Sent Acknowledgement for {message}. Publishing Channel Updates");

                PublishArticles(channel.Articles);

                _logger.Debug($"Channel Updates Sent");
            }
            catch (Exception ex)
            {
                _logger.Debug(ex.ToString());
            }
        }

        private void PublishArticles(List<Article> articles)
        {
            foreach(var article in articles)
            {
                var jsonString =
                    _articleMessageConverter.SerializeJson(article);

                _logger.Debug($"Sending Article {jsonString}");

                _publisherService.PublishMessage(jsonString);
            }
        }

        private void SetupConnectionToQueues()
        {
            _consumerService.SetupConnection();
            _publisherService.SetupConnection();
            _consumerService.Consumer.Received += OnMessageReceived;
        }

        private void CloseConnectionToQueues()
        {
            _consumerService.Consumer.Received -= OnMessageReceived;
            _consumerService.CloseConnection();
            _publisherService.CloseConnection();
        }
    }
}
