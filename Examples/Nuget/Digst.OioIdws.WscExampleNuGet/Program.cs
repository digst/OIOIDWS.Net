using System;
using System.Configuration;
using System.Threading;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Wsc.OioWsTrust;
using Digst.OioIdws.WscExampleNuGet.Service_References.HelloWorldProxy;
using log4net.Config;

namespace Digst.OioIdws.WscExampleNuGet
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setup Log4Net configuration by loading it from configuration file. 
            // log4net is not necessary and is only being used for demonstration.
            XmlConfigurator.Configure();

            // Read the identifier and URL of the service to invoke from config appsettings
            var wspIdentifier = ConfigurationManager.AppSettings["WspIdentifier"];
            var wspUrl = ConfigurationManager.AppSettings["WspUrl"];

            // To ensure that the WSP is up and running.
            Thread.Sleep(1000);

            // Retrieve token
            // Retrieve token
            var cfg = SecurityTokenServiceClientConfigurationSection.FromConfiguration();
            var stsTokenService = new CachedSecurityTokenServiceClient(new NemloginSecurityTokenServiceClient(cfg), null, null);
            var securityToken = stsTokenService.GetServiceToken(wspIdentifier, KeyType.HolderOfKey);

            // Call WSP with token
            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(securityToken);
            Console.WriteLine(channelWithIssuedToken.HelloNone("Schultz")); // Even if the protection level is set to 'None' Digst.OioIdws.Wsc ensures that the body is always at least signed.
            Console.WriteLine(channelWithIssuedToken.HelloSign("Schultz"));
            Console.WriteLine(channelWithIssuedToken.HelloEncryptAndSign("Schultz"));

            //Checking that SOAP faults can be read.SOAP faults are encrypted in Sign and EncryptAndSign mode if no special care is taken.
            try
            {
                channelWithIssuedToken.HelloSignError("Schultz");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }
    }
}
