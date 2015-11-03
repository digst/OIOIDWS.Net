using System;
using System.Threading;
using Digst.OioIdws.Wsc;
using Digst.OioIdws.Wsc.OioWsTrust;
using Digst.OioIdws.WscExample.HelloWorldProxy;
using log4net.Config;

namespace Digst.OioIdws.WscExample
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
            HelloWorldClient client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(securityToken);
            Console.WriteLine(channelWithIssuedToken.HelloNone("Schultz")); // Even if the protection level is set to 'None' Digst.OioIdws.Wsc ensures that the body is always at least signed.
            Console.WriteLine(channelWithIssuedToken.HelloSign("Schultz")); 

            // Encrypted calls fails client side. However, encryption at message level is not required and no further investigation has been putted into this issue yet.
            //Console.WriteLine(channelWithIssuedToken.HelloEncryptAndSign("Schultz")); 

            Console.ReadKey();
        }
    }
}
        