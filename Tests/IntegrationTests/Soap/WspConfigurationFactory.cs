using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;
using Digst.OioIdws.Soap.Behaviors;
using Digst.OioIdws.Soap.StrCustomization;

namespace DK.Gov.Oio.Idws.IntegrationTests.Soap
{
    public class WspConfigurationFactory
    {
        private const string TokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0";

        public static ChannelFactory<T> CreateChannelFactory<T>(WspConfiguration wspConfiguration)
        {
            var customBinding = new CustomBinding();
            WithAsymmetricSecurity(customBinding);
            WithMessageEncoding(customBinding);
            WithTransport(customBinding, wspConfiguration);
            
            var factory = new ChannelFactory<T>(customBinding, wspConfiguration.EndpointAddress);
            factory.Credentials.UseIdentityConfiguration = true;
            // Equivalent to SetScopedCertificate, but enables certificate for WSP from outside CAPI
            factory.Credentials.ServiceCertificate.ScopedCertificates[new Uri(wspConfiguration.Hostname)] = wspConfiguration.Certificate;
            factory.Endpoint.Behaviors.Add(new SoapClientBehavior());
            return factory;
        }

        private static void WithAsymmetricSecurity(CustomBinding customBinding)
        {
            var securityTokenParameters = new X509SecurityTokenParameters(X509KeyIdentifierClauseType.Any, SecurityTokenInclusionMode.AlwaysToInitiator);
            var tokenParameters = new CustomizedIssuedSecurityTokenParameters(TokenType)
            {
                UseStrTransform = true
            };
            var asymmetric = new AsymmetricSecurityBindingElement(securityTokenParameters, tokenParameters)
            {
                AllowSerializedSigningTokenOnReply = true,
                ProtectTokens = true
            };
            asymmetric.SetKeyDerivation(false);
            customBinding.Elements.Add(asymmetric);
        }
        
        private static void WithMessageEncoding(CustomBinding customBinding)
        {
            var messageEncoding = new TextMessageEncodingBindingElement
            {
                MessageVersion = MessageVersion.Soap12WSAddressing10
            };
            customBinding.Elements.Add(messageEncoding);
        }

        private static void WithTransport(CustomBinding customBinding, WspConfiguration wspConfiguration)
        {
            var transport = wspConfiguration.Hostname.ToLower().StartsWith("https://")
                ? new HttpsTransportBindingElement()
                : new HttpTransportBindingElement();
            customBinding.Elements.Add(transport);
        }
    }
}