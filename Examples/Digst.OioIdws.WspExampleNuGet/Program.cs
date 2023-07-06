using System;
using System.ServiceModel;

namespace Digst.OioIdws.WspExampleNuGet
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create the ServiceHost.
            const string httpLocalhostHelloworld = "https://Digst.OioIdws.Wsp:9091/HelloWorld";
            using (var host = new ServiceHost(typeof(HelloWorld), new Uri(httpLocalhostHelloworld)))
            {
                host.Open();

                Console.WriteLine("The service is ready at {0}", httpLocalhostHelloworld);
                Console.WriteLine("Press <Enter> to stop the service.");
                Console.ReadLine();

                // Close the ServiceHost.
                host.Close();
            }
        }
    }
}
