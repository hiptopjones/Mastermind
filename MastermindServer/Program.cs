using Nancy;
using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastermindServer
{
    class Program
    {
        static void Main(string[] args)
        {
            HostConfiguration hostConfigs = new HostConfiguration();
            hostConfigs.UrlReservations.CreateAutomatically = true;

            CustomBootstrapper bootstrapper = new CustomBootstrapper();

            using (var host = new NancyHost(bootstrapper, hostConfigs, new Uri("http://localhost:1234")))
            {
                host.Start();
                Console.WriteLine("Running on http://localhost:1234");
                Console.ReadLine();
            }
        }
    }
}
