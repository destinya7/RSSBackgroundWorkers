using System;
using System.ServiceProcess;
using System.Text;
using RabbitMQ.Client.Events;
using RSSFetcherService.Core;
using RSSFetcherService.Services;

namespace RSSFetcherService
{
    public partial class RSSFetcherService : ServiceBase
    {
        private IWorkerQueueConsumerService _consumerService;
        private IFetcherCore _fetcherCore;
        private ILoggerService _logger;

        public RSSFetcherService(
            IWorkerQueueConsumerService consumerService,
            IFetcherCore fetcherCore,
            ILoggerService logger
        )
        {
            InitializeComponent();

            _consumerService = consumerService;
            _fetcherCore = fetcherCore;
            _logger = logger;
        }

        protected override void OnStart(string[] args)
        {
            _logger.Debug("Service Start " + DateTime.Now);
            SetupConnectionToQueue();
        }

        protected override void OnStop()
        {
            _logger.Debug("Service Stopped " + DateTime.Now);
            TearDownConnection();
        }

        private void SetupConnectionToQueue()
        {
            _consumerService.SetupConnection();
            _consumerService.Consumer.Received += OnMessageReceived;
        }

        private void TearDownConnection()
        {
            _consumerService.Consumer.Received -= OnMessageReceived;
            _consumerService.Channel.Close();
            _consumerService.Connection.Close();
        }

        private void OnMessageReceived(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
            var message = Encoding.UTF8.GetString(body);

            _consumerService.Channel.BasicAck(e.DeliveryTag, false);
            _logger.Debug($"Message Received: {message} - {DateTime.Now}");
        }
    }
}
