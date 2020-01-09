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
            _logger.Info("Service Start " + DateTime.Now);
            SetupConnectionToQueues();
        }

        protected override void OnStop()
        {
            _logger.Info("Service Stopped " + DateTime.Now);
            CloseConnectionToQueues();
        }

        private async void OnMessageReceived(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
            var message = Encoding.UTF8.GetString(body);

            try
            {
                _logger.Info($"Message Received: {message}. Fetching channel");

                Channel channel = await _fetcherCore.FetchChannel(message);

                _logger.Info($"Done Fetching Channel");

                _logger.Info($"Sent Acknowledgement for {message}. Publishing Channel Updates");

                PublishArticles(channel.Articles);

                _consumerService.Channel.BasicAck(e.DeliveryTag, false);

                _logger.Info($"Channel Updates Sent");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
        }

        private void PublishArticles(List<Article> articles)
        {
            foreach(var article in articles)
            {
                var jsonString =
                    _articleMessageConverter.SerializeJson(article);

                _logger.Info($"Sending Article {jsonString}");

                _publisherService.PublishMessage(jsonString);
            }
        }

        private void SetupConnectionToQueues()
        {
            _logger.Info("Setting up connections to queue");
            _consumerService.SetupConnection();
            _logger.Info("Done setting connection to worker queue");
            _publisherService.SetupConnection();
            _logger.Info("Done setting connection to message queue");
            _consumerService.Consumer.Received += OnMessageReceived;
            _logger.Info("Done attaching listener for worker queue");
            _consumerService.StartListening();
            _logger.Info("Starting to listen");
            _logger.Info("Done setting up connections to queue");
        }

        private void CloseConnectionToQueues()
        {
            _logger.Info("Closing connections to queue");
            _consumerService.Consumer.Received -= OnMessageReceived;
            _consumerService.CloseConnection();
            _publisherService.CloseConnection();
            _logger.Info("Done closing connections to queue");
        }
    }
}
