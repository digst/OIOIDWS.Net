using System;
using System.Threading;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Wsc.OioWsTrust;
using Digst.OioIdws.DotnetWscJavaWspExample.HelloWorldProxy;
using log4net.Config;

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
            IStsTokenService stsTokenService = 
                new StsTokenServiceCache(
                    TokenServiceConfigurationFactory.CreateConfiguration()
                );
            var securityToken = stsTokenService.GetToken();

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
