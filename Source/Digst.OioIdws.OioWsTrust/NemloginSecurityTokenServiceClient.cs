using System;
using System.IdentityModel.Metadata;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using Digst.OioIdws.Common.Logging;
using Digst.OioIdws.Common.Utils;
using Digst.OioIdws.OioWsTrust.ProtocolChannel;

namespace Digst.OioIdws.OioWsTrust
{
    /// <summary>
    /// An implementation of a Security Token Service (STS) client which is customized to act
    /// against the NemLog-in STS.
    /// </summary>
    public class NemloginSecurityTokenServiceClient : ISecurityTokenServiceClient
    {
        private readonly SecurityTokenServiceClientConfiguration _config;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public NemloginSecurityTokenServiceClient(SecurityTokenServiceClientConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }


        /// <summary>
        /// <see cref="ISecurityTokenServiceClient.GetServiceToken"/>
        /// </summary>
        /// <param name="serviceIdentifier"></param>
        /// <param name="keyType"></param>
        public SecurityToken GetServiceToken(string serviceIdentifier, KeyType keyType)
        {
            return GetTokenInternal(null, serviceIdentifier, keyType);
        }


        /// <inheritdoc />
        public SecurityToken GetBootstrapTokenFromAuthenticationToken(SecurityToken authenticationToken)
        {
            return GetTokenInternal(authenticationToken, _config.WscIdentifier , KeyType.HolderOfKey);
        }


        /// <summary>
        /// <see cref="ISecurityTokenServiceClient.GetIdentityTokenFromBootstrapToken"/>
        /// </summary>
        public SecurityToken GetIdentityTokenFromBootstrapToken(SecurityToken bootstrapToken, string serviceIdentifier, KeyType keyType)
        {
            if (bootstrapToken == null) throw new ArgumentNullException(nameof(bootstrapToken));
            if (serviceIdentifier == null) throw new ArgumentNullException(nameof(serviceIdentifier));

            return GetTokenInternal(bootstrapToken, serviceIdentifier, keyType);
        }

        private SecurityToken GetTokenInternal(SecurityToken bootstrapToken, string serviceIdentifier, KeyType keyType)
        {
            Logger.Instance.Trace(
                $@"RequestToken called with the client certificate: {_config.WscCertificate.SubjectName.Name} ({
                        _config.WscCertificate.Thumbprint
                    })");
            Logger.Instance.Trace(
                $@"RequestToken called with the STS certificate: {_config.StsCertificate.SubjectName.Name} ({
                        _config.StsCertificate.Thumbprint
                    })");

            try
            {
                // Create custom binding
                var stsBinding = new CustomBinding();
                if (_config.SendTimeout.HasValue)
                {
                    Logger.Instance.Warning($"RequestToken send timeout set to {_config.SendTimeout.Value}");
                    stsBinding.SendTimeout = _config.SendTimeout.Value;
                }
                stsBinding.Elements.Add(new OioWsTrustBindingElement(_config.StsCertificate, new NemLoginWsTrustMessageTransformer()));
                stsBinding.Elements.Add(new TextMessageEncodingBindingElement(MessageVersion.Soap11WSAddressing10,
                    Encoding.UTF8));
                // ManualAddressing must be true in order to make sure that wsa header elements are not altered in the HttpsTransportChannel which happens after xml elements have been digitally signed.
                stsBinding.Elements.Add(new HttpsTransportBindingElement() { ManualAddressing = true });

                // Setup channel factory and apply client credentials
                var factory = new WSTrustChannelFactory(stsBinding, new EndpointAddress(_config.ServiceTokenUrl));
                factory.TrustVersion = TrustVersion.WSTrust13;
                factory.Credentials.ClientCertificate.Certificate = _config.WscCertificate;

                // Create token request
                // UseKey and KeyType are only set in order for WCF to know which proof token to use when signing the request sent from WSC to WSP. Hence, UseKey and KeyType are actually sent to the NemLogin-STS but not used for anything by the STS.
                var requestSecurityToken = new RequestSecurityToken
                {
                    RequestType = RequestTypes.Issue,
                    AppliesTo = new EndpointReference(serviceIdentifier),

                    // TokenType is optional according to [NEMLOGIN-STSRULES]. If specified it must contain the value http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0 which is the only type NemLogin STS supports.
                    // We specify it in case that NemLogin STS supports other token types in the future.
                    // Currently if TokenType is not specified ... then TokenType is also not specified in the RequestSecurityTokenResponse (RSTR). According to spec it should always be specified in the RSTR. Specifying TokenType in the RST triggers the TokenType to be specified in the RSTR.
                    TokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0",
                    UseKey =
                        new UseKey(new X509SecurityToken(_config
                            .WscCertificate)), // The UseKey must be set in order for WCF to know which certificate must be used for signing the request from WSC to WSP. Thus, the usekey is actually the same as the proof key in the holder-of-key scenario.
                    KeyType =
                        KeyTypes
                            .Asymmetric // The KeyType must be set to Asymmetric in order for WCF to know that it must use the UseKey as proof token.
                };

                requestSecurityToken.SignatureAlgorithm = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
                
                // Lifetime is only specified if it has been configured. Should result in a default life time (8 hours) on issued token if not specified. If specified, STS is not obligated to honor this range and may return a token with a shorter life time in RSTR.
                var currentTimeUtc = DateTime.UtcNow;
                if (_config.TokenLifeTime.HasValue)
                {
                    requestSecurityToken.Lifetime = new Lifetime(null,
                        currentTimeUtc.Add(_config.TokenLifeTime.Value));
                }

                // ActAs is only set if a bootstrap token is supplied.
                if (bootstrapToken != null)
                {
                    // First check validity before using it.
                    if (bootstrapToken.ValidTo < currentTimeUtc)
                        throw new ArgumentException(
                            $"Bootstrap token life time has expired. Please renew before trying again. Bootstrap token ID: {bootstrapToken.Id}, Valid to: {bootstrapToken.ValidTo}, Current time: {currentTimeUtc}");
                    requestSecurityToken.ActAs = new SecurityTokenElement(bootstrapToken);
                }

                // Request token and return
                var wsTrustChannelContract = factory.CreateChannel();
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
    }
}