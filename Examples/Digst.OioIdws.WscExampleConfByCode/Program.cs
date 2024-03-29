﻿using System;
using System.Threading;

using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Wsc.OioWsTrust;
using Digst.OioIdws.WscExampleConfByCode.HelloWorldProxy;

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

            // To ensure that the WSP is up and running.
            Thread.Sleep(1000);

            // Retrieve token
            IStsTokenService stsTokenService =
                new StsTokenServiceCache(
                    TokenServiceConfigurationFactory.CreateConfiguration()
                );
            var securityToken = stsTokenService.GetToken();

            // Call WSP with token
            var hostname = "https://Digst.OioIdws.Wsp:9090/HelloWorld";
            var customBinding = new Channels.CustomBinding();
            var endpointAddress = new System.ServiceModel.EndpointAddress(
                new Uri(hostname),
                System.ServiceModel.EndpointIdentity.CreateDnsIdentity(
                    "OIOIDWS.NET WSP - Test"
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

            System.ServiceModel.ChannelFactory<IHelloWorld> factory =
                new System.ServiceModel.ChannelFactory<IHelloWorld>(
                    customBinding, endpointAddress
                );
            factory.Credentials.UseIdentityConfiguration = true;
            factory.Credentials.ServiceCertificate.SetScopedCertificate(
                X509Certificates.StoreLocation.LocalMachine,
                X509Certificates.StoreName.My,
                X509Certificates.X509FindType.FindByThumbprint,
                "d738a7d146f07e02c16cf28dac11e742e4ce9582",
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