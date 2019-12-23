using RSSFetcherService.Core;
using System.ServiceProcess;

namespace RSSFetcherService
{
    public partial class RSSFetcherService : ServiceBase
    {
        private IFetcherCore _fetcherCore;

        public RSSFetcherService(IFetcherCore _fetcherCore)
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }
    }
}
