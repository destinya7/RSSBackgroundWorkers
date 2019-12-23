using System.ServiceProcess;
using RSSBackgroundWorkerBusiness.Repositories;
using RSSBackgroundWorkerBusiness.DAL;

namespace RSSCollectorService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
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
