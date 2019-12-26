using System.ServiceProcess;
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

        public RSSCollectorService(
            ICollectorCore collectorCore,
            ILoggerService logger
        )
        {
            InitializeComponent();

            _collectorCore = collectorCore;
            _logger = logger;
            _timer = new Timer();
        }

        protected override void OnStart(string[] args)
        {
            _logger.Debug("Service Started");

            _timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            _timer.Interval = 216000000;
            _timer.Enabled = true;
            _timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _collectorCore.CollectUrls();
        }

        protected override void OnStop()
        {
            _logger.Debug("Service Stopped");
            _timer.Stop();
        }
    }
}
