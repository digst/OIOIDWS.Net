using System;
using System.Configuration;
using System.Threading;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Soap.StrCustomization;
using Digst.OioIdws.Wsc.OioWsTrust;
using Digst.OioIdws.WscExampleConfByCode.Connected_Services.HelloWorldProxy;
using log4net.Config;
using Channels = System.ServiceModel.Channels;
using SecurityTokens = System.ServiceModel.Security.Tokens;
using X509Certificates = System.Security.Cryptography.X509Certificates;

namespace Digst.OioIdws.WscExampleConfByCode
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setup Log4Net configuration by loading it from configuration file
            // log4net is not necessary and is only being used for demonstration
            XmlConfigurator.Configure();

            // Read the identifier and URL of the service to invoke from config appsettings
            var wspIdentifier = ConfigurationManager.AppSettings["WspIdentifier"];
            var wspUrl = ConfigurationManager.AppSettings["WspUrl"];

            // To ensure that the WSP is up and running.
            Thread.Sleep(1000);

            // Retrieve token
            var cfg = SecurityTokenServiceClientConfigurationSection.FromConfiguration();
            var stsTokenService = new CachedSecurityTokenServiceClient(new NemloginSecurityTokenServiceClient(cfg), null, null);
            var securityToken = stsTokenService.GetServiceToken(wspIdentifier, KeyType.HolderOfKey);

            // Call WSP with token
            var hostname = wspUrl;
            var customBinding = new Channels.CustomBinding();
            var endpointAddress = new System.ServiceModel.EndpointAddress(
                new Uri(hostname),
                System.ServiceModel.EndpointIdentity.CreateDnsIdentity(
                    "wsp.oioidws-net.dk TEST (funktionscertifikat)"
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
                    new CustomizedIssuedSecurityTokenParameters(
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

            System.ServiceModel.ChannelFactory<IHelloWorld> factory =
                new System.ServiceModel.ChannelFactory<IHelloWorld>(
                    customBinding, endpointAddress
                );
            factory.Credentials.UseIdentityConfiguration = true;
            factory.Credentials.ServiceCertificate.SetScopedCertificate(
                X509Certificates.StoreLocation.LocalMachine,
                X509Certificates.StoreName.My,
                X509Certificates.X509FindType.FindByThumbprint,
                "1F0830937C74B0567D6B05C07B6155059D9B10C7",
                new Uri(hostname)
            );
            factory.Endpoint.Behaviors.Add(
                new Soap.Behaviors.SoapClientBehavior()
            );

            var channelWithIssuedToken =
                factory.CreateChannelWithIssuedToken(securityToken);

            Console.WriteLine(channelWithIssuedToken.HelloNone("Schultz"));
            Console.WriteLine(channelWithIssuedToken.HelloSign("Schultz"));
            Console.WriteLine(channelWithIssuedToken.HelloEncryptAndSign("Schultz"));

            // Checking that SOAP faults can be read. SOAP faults are encrypted
            // in Sign and EncryptAndSign mode if no special care is taken.
            try
            {
                channelWithIssuedToken.HelloSignError("Schultz");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Checking that SOAP faults can be read when only being signed.
            // SOAP faults are only signed if special care is taken.
            try
            {
                channelWithIssuedToken.HelloSignErrorNotEncrypted("Schultz");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}