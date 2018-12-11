using System;
using System.ServiceModel;
using Digst.OioIdws.Examples.Healthcare.ClientServer.Contracts;
using static System.Console;

namespace Digst.OioIdws.Examples.Healthcare.ClientServer.Backend
{
    class Program
    {
        static void Main(string[] args)
        {
            // Start the service host
            var host = new ServiceHost(typeof(ClientServerService), new Uri("http://localhost:9901"));
            host.AddServiceEndpoint(typeof(IClientServerService), new WSHttpBinding(), "");
            host.Open();

            WriteLine("In-process service is now running...\n");

            WriteLine("--------------------------------------------------------------------------------");
            WriteLine("Client/Server scenario: Example backend");
            WriteLine("--------------------------------------------------------------------------------");

            WriteLine("Press any key to exit the service");
            ReadKey();
        }
    }
}