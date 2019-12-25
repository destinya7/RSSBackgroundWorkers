using RSSCollectorService.Startup;
using System.ServiceProcess;

namespace RSSCollectorService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            Bootstrapper.Bootstrap();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                Bootstrapper.Resolve<RSSCollectorService>()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
