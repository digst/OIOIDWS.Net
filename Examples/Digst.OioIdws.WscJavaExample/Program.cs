using System;
using System.Threading;
using Digst.OioIdws.Wsc.OioWsTrust;
using Digst.OioIdws.WscJavaExample.HelloWorldProxy;
using log4net.Config;

namespace Digst.OioIdws.WscJavaExample
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
            ITokenService tokenService = new TokenService();
            var securityToken = tokenService.GetToken();

            // Call WSP with token
            var client = new HelloWorldPortTypeClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(securityToken);
            var helloWorldRequest = new HelloWorldRequest("Schultz");
            Console.WriteLine(channelWithIssuedToken.HelloWorld(helloWorldRequest));

            // Encrypted calls fails client side. However, encryption at message level is not required and no further investigation has been putted into this issue yet.
            // Console.WriteLine(channelWithIssuedToken.HelloEncryptAndSign("Schultz")); 

            Console.ReadKey();
        }
    }
}
