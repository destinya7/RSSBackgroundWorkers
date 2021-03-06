﻿using System.ServiceProcess;
using System.Timers;
using RSSCollectorService.Core;
using RSSCollectorService.Services;

namespace RSSCollectorService
{
    public partial class RSSCollectorService : ServiceBase
    {
        private Timer _timer;

        private ICollectorCore _collectorCore;
        private ILoggerService _logger;
        private IWorkerQueuePublisher _publisherService;

        public RSSCollectorService(
            ICollectorCore collectorCore,
            ILoggerService logger,
            IWorkerQueuePublisher publisherService
        )
        {
            InitializeComponent();

            _collectorCore = collectorCore;
            _logger = logger;
            _publisherService = publisherService;

            _timer = new Timer();
        }

        protected override void OnStart(string[] args)
        {
            _logger.Info("Service Started");

            _publisherService.SetupConnection();

            StartCollecting();

            _timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            _timer.Interval = 30000;
            _timer.Enabled = true;
            _timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            StartCollecting();
        }

        protected override void OnStop()
        {
            _logger.Info("Service Stopped");
            _timer.Stop();
        }

        private void StartCollecting()
        {
            _logger.Info("Collecting Urls");
            _collectorCore.CollectUrls();
            _logger.Info("Done Collecting Urls");
        }
    }
}
