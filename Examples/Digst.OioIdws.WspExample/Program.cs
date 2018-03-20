using System;
using System.ServiceModel;

namespace Digst.OioIdws.WspExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create the ServiceHost.
            // HTTP: const string httpLocalhostHelloworld = "http://Digst.OioIdws.Wsp:9090/HelloWorld";
            const string httpLocalhostHelloworld = "https://Digst.OioIdws.Wsp:9090/HelloWorld";
            using (var host = new ServiceHost(typeof(HelloWorld), new Uri(httpLocalhostHelloworld)))
            {
                host.Open();

                // Ensure WSP only uses TLS 1.2 to communicate with WSC.
                // 
                // Note: As this can't be enforced by code/configuration, you
                // MUST use a tool like "IIS Crypto" (free) where you can choose 
                // the PCI 3.1 Template and unmark TLS 1.1) to enforce this on
                // an Operating System level.
                // 
                // Source: https://www.nartac.com/Products/IISCrypto

                Console.WriteLine("The service is ready at {0}", httpLocalhostHelloworld);
                Console.WriteLine("Press <Enter> to stop the service.");
                Console.ReadLine();

                // Close the ServiceHost.
                host.Close();
            }
        }
    }
}