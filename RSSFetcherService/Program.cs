using RSSFetcherService.Startup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace RSSFetcherService
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
                Bootstrapper.Resolve<RSSFetcherService>()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
