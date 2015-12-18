using System;
using System.Diagnostics;
using Microsoft.Owin.Hosting;

namespace Digst.OioIdws.Rest.Examples.AS
{
    class Program
    {
        static void Main(string[] args)
        {
            var endpoint = args.Length > 0 ? args[0] : "https://digst.oioidws.rest.as:10001";

            using (WebApp.Start<Startup>(endpoint))
            {
                Console.WriteLine($"The service is ready at {endpoint} (PID:{Process.GetCurrentProcess().Id})");
                Console.WriteLine("Press <Enter> to stop the service.");
                Console.ReadLine();
            }
        }
    }
}
