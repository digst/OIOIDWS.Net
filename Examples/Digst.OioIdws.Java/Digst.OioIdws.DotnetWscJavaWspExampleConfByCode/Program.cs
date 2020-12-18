using System;
using System.Threading;

using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Wsc.OioWsTrust;
using Digst.OioIdws.DotnetWscJavaWspExampleConfByCode.HelloWorldProxy;

using log4net.Config;

using Channels = System.ServiceModel.Channels;
using SecurityTokens = System.ServiceModel.Security.Tokens;
using X509Certificates = System.Security.Cryptography.X509Certificates;

namespace Digst.OioIdws.DotnetWscJavaWspExampleConfByCode
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setup Log4Net configuration by loading it from configuration file
            // log4net is not necessary and is only being used for demonstration
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
            var hostname = "https://localhost:8443/HelloWorld/services/helloworld";
            var customBinding = new Channels.CustomBinding();
            var endpointAddress = new System.ServiceModel.EndpointAddress(
                new Uri(hostname),
                System.ServiceModel.EndpointIdentity.CreateDnsIdentity(
                    //"WSP (wsp.oioidws-net.dk TEST) (funktionscertifikat)"
                    "eID JAVA test (funktionscertifikat)"
                ),
                new Channels.AddressHeader[] { }
            );

            var asymmetric =
                new Channels.AsymmetricSecurityBindingElement
                (
                    new SecurityTokens.X509SecurityTokenParameters(
                        SecurityTokens.X509KeyIdentifierClauseType.Any,
                        SecurityTokens.SecurityTokenInclusionMode.AlwaysToInitiator
                    ),
                    new Soap.StrCustomization.CustomizedIssuedSecurityTokenParameters(
                        "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0"
                    )
                    {
                        UseStrTransform = true
                    }
                )
                {
                    AllowSerializedSigningTokenOnReply = true,
                    ProtectTokens = true
                };
            asymmetric.SetKeyDerivation(false);
            var messageEncoding =
                new Channels.TextMessageEncodingBindingElement
                {
                    MessageVersion =
                        Channels.MessageVersion.Soap12WSAddressing10
                };
            var transport =
                (hostname.ToLower().StartsWith("https://"))
                    ? new Channels.HttpsTransportBindingElement()
                    : new Channels.HttpTransportBindingElement();

            customBinding.Elements.Add(asymmetric);
            customBinding.Elements.Add(messageEncoding);
            customBinding.Elements.Add(transport);

            System.ServiceModel.ChannelFactory<HelloWorldPortType> factory =
                new System.ServiceModel.ChannelFactory<HelloWorldPortType>(
                    customBinding, endpointAddress
                );
            factory.Credentials.UseIdentityConfiguration = true;
            factory.Credentials.ServiceCertificate.SetScopedCertificate(
                X509Certificates.StoreLocation.LocalMachine,
                X509Certificates.StoreName.My,
                X509Certificates.X509FindType.FindByThumbprint,
                //"ca30025a4981147505b8d7a59052ac40c7033688",
                "85398FCF737FB76F554C6F2422CC39D3A35EC26F",
                new Uri(hostname)
            );
            factory.Endpoint.Behaviors.Add(
                new Soap.Behaviors.SoapClientBehavior()
            );

            var channelWithIssuedToken =
                factory.CreateChannelWithIssuedToken(securityToken);

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
