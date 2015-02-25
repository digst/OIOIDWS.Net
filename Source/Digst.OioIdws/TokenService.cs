using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using Digst.OioIdws.BindingElements;
using Digst.OioIdws.Configurations;
using Digst.OioIdws.Logging;

namespace Digst.OioIdws
{
    /// <summary>
    /// <see cref="ITokenService"/>
    /// </summary>
    public class TokenService : ITokenService
    {
        /// <summary>
        /// <see cref="ITokenService.GetToken()"/>
        /// </summary>
        public SecurityToken GetToken()
        {
            // Retrieve Configuration
            var config =
                (OioIdwsConfiguration)System.Configuration.ConfigurationManager.GetSection("oioIdwsConfiguration");

            return GetToken(config);
        }

        /// <summary>
        /// <see cref="ITokenService.GetToken(OioIdwsConfiguration)"/>
        /// </summary>
        public SecurityToken GetToken(OioIdwsConfiguration config)
        {
            // Check input arguments
            if (config == null) throw new ArgumentNullException("config");
            if (string.IsNullOrEmpty(config.WspEndpointID)) throw new ArgumentException("WspEndpointID");
            if (string.IsNullOrEmpty(config.StsEndpointAddress)) throw new ArgumentException("StsEndpointAddress");
            // X509FindType cannot be tested below because default value is FindByThumbprint
            if (config.ClientCertificate == null || config.ClientCertificate.StoreLocation == 0 || config.ClientCertificate.StoreName == 0 || string.IsNullOrEmpty(config.ClientCertificate.FindValue)) throw new ArgumentException("ClientCertificate");
            if (config.StsCertificate == null || config.StsCertificate.StoreLocation == 0 || config.StsCertificate.StoreName == 0 || string.IsNullOrEmpty(config.StsCertificate.FindValue)) throw new ArgumentException("StsCertificate");

            if (config.DebugMode)
                Logger.Instance.Warning("GetToken called in debug mode. Send timeout has been set to 24 hours in order to make debugging easier!");

            var clientCertificate = config.ClientCertificate;
            var stsCertificate = config.StsCertificate;
            Logger.Instance.Debug(
                string.Format(
                    "GetToken called with the client certificate: storeLocation=\"{0}\" storeName=\"{1}\" x509FindType=\"{2}\" findValue=\"{3}\"",
                    clientCertificate.StoreLocation, clientCertificate.StoreName, clientCertificate.X509FindType,
                    clientCertificate.FindValue));
            Logger.Instance.Debug(
                string.Format(
                    "GetToken called with the STS certificate: storeLocation=\"{0}\" storeName=\"{1}\" x509FindType=\"{2}\" findValue=\"{3}\"",
                    stsCertificate.StoreLocation, stsCertificate.StoreName, stsCertificate.X509FindType,
                    stsCertificate.FindValue));
            
            try
            {
                // Create custom binding
                var stsBinding = new CustomBinding();
                if (config.DebugMode)
                    stsBinding.SendTimeout = new TimeSpan(1, 0, 0, 0); // Gives you more time to debug.
                stsBinding.Elements.Add(new SignatureCaseBindingElement(GetStsCertificate(config)));
                stsBinding.Elements.Add(new TextMessageEncodingBindingElement(MessageVersion.Soap11WSAddressing10,
                    Encoding.UTF8));
                // ManualAddressing must be true in order to make sure that wsa header elements are not altered in the HttpsTransportChannel which happens after xml elements have been digitally signed.
                stsBinding.Elements.Add(new HttpsTransportBindingElement() {ManualAddressing = true});

                // Setup channel factory and apply client credentials
                var factory = new WSTrustChannelFactory(stsBinding, new EndpointAddress(config.StsEndpointAddress));
                factory.TrustVersion = TrustVersion.WSTrust13;
                factory.Credentials.ClientCertificate.SetCertificate(clientCertificate.StoreLocation,
                    clientCertificate.StoreName, clientCertificate.X509FindType,
                    clientCertificate.FindValue);

                // Create token request
                var requestSecurityToken = new RequestSecurityToken
                {
                    RequestType = RequestTypes.Issue,
                    AppliesTo = new EndpointReference(config.WspEndpointID),
                    // TokenType is optional according to [NEMLOGIN-STSRULES]. If specified it must contain the value http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0 which is the only type NemLogin STS supports.
                    // We specify it in case that NemLogin STS supports other token types in the future.
                    // Currently if TokenType is not specified ... then TokenType is also not specified in the RequestSecurityTokenResponse (RSTR). According to spec it should always be specified in the RSTR. Specifying TokenType in the RST triggers the TokenType to be specified in the RSTR.
                    TokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0"
                };
                // Lifetime is only specified if it has been configured. Should result in a default life time (8 hours) on issued token if not specified. If specified, STS is not obligated to honor this range and may return a token with a shorter life time in RSTR.
                if (config.TokenLifeTimeInMinutes.HasValue)
                {
                    requestSecurityToken.Lifetime = new Lifetime(null,
                        DateTime.UtcNow.AddMinutes(config.TokenLifeTimeInMinutes.Value));
                }

                // Request token and return
                IWSTrustChannelContract wsTrustChannelContract = factory.CreateChannel();
                var securityToken = wsTrustChannelContract.Issue(requestSecurityToken);

                return securityToken;
            }
            // Log all errors and rethrow
            catch (Exception e)
            {
                Logger.Instance.Error("Error occured while requesting token. See exception details!", e);
                throw;
            }
        }

        private static X509Certificate2 GetStsCertificate(OioIdwsConfiguration oioIdwsConfiguration)
        {
            // Retrieve configuration
            var stsCertificateConfiguration = oioIdwsConfiguration.StsCertificate;

            var store = new X509Store(stsCertificateConfiguration.StoreName, stsCertificateConfiguration.StoreLocation);

            store.Open(OpenFlags.ReadOnly);

            var cert =
                store.Certificates.Find(stsCertificateConfiguration.X509FindType, stsCertificateConfiguration.FindValue,
                    true).OfType<X509Certificate2>().FirstOrDefault();

            if (cert == null)
                throw new InvalidOperationException("STS certificate with the following configuration was not found: " + stsCertificateConfiguration);

            return cert;
        }
    }
}
