using System.ServiceProcess;
using RSSBackgroundWorkerBusiness.Repositories;
using RSSBackgroundWorkerBusiness.DAL;

namespace RSSCollectorService
{
    public partial class RSSCollectorService : ServiceBase
    {
        public RSSCollectorService()
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
