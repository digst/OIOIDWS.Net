using System;
using System.IdentityModel.Metadata;
using System.Threading;
using Digst.OioIdws.DotnetWscJavaWspExample.Service_References.HelloWorldProxy;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Wsc.OioWsTrust;
using log4net.Config;
using KeyType = Digst.OioIdws.OioWsTrust.KeyType;

namespace Digst.OioIdws.DotnetWscJavaWspExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setup Log4Net configuration by loading it from configuration file. 
            // log4net is not necessary and is only being used for demonstration.
            XmlConfigurator.Configure();

            // To ensure that the WSP is up and running.
            Thread.Sleep(1000);

            // Retrieve token
            ISecurityTokenServiceClient stsTokenService = 
                new LocalSecurityTokenServiceClient(
                    TokenServiceConfigurationFactory.CreateConfiguration(), null
                );
            var securityToken = stsTokenService.GetServiceToken("https://localhost:8443/HelloWorld/services/helloworld", KeyType.HolderOfKey);

            // Call WSP with token
            var client = new HelloWorldPortTypeClient();

            var channelWithIssuedToken = 
                client.ChannelFactory.CreateChannelWithIssuedToken(
                    securityToken
                );

            var helloWorldRequestJohn = new HelloWorldRequest("John");
            Console.WriteLine(
                channelWithIssuedToken.HelloWorld(helloWorldRequestJohn).response
            );

            var helloWorldRequestJane = new HelloWorldRequest("Jane");
            Console.WriteLine(
                channelWithIssuedToken.HelloWorld(helloWorldRequestJane).response
            );

            try
            {
                // third call will trigger a SOAPFault
                var helloWorldRequest = new HelloWorldRequest("");
                Console.WriteLine(
                    channelWithIssuedToken.HelloWorld(helloWorldRequest).response
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expected SOAPFault caught: " + ex.Message);
            }

            // Encrypted calls fails client side. However, encryption at message
            // level is not required and no further investigation has been 
            // putted into this issue yet.
            //
            // Console.WriteLine(channelWithIssuedToken.HelloEncryptAndSign("Schultz"));

            Console.WriteLine("Press <Enter> to stop the service.");
            Console.ReadLine();
        }
    }
}
