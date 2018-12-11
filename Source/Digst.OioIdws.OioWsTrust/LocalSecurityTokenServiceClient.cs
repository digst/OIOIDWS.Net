using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using Digst.OioIdws.Common.Logging;
using Digst.OioIdws.OioWsTrust.Bindings;
using Digst.OioIdws.OioWsTrust.ProtocolChannel;

namespace Digst.OioIdws.OioWsTrust
{
    /// <summary>
    /// Client for a standards based (i.e. not NemLog-in) Security Token Service (STS). 
    /// </summary>
    public class LocalSecurityTokenServiceClient : ISecurityTokenServiceClient
    {
        private readonly RequestSecurityToken _templateRequest;
        private readonly SecurityTokenServiceClientConfiguration _config;

        /// <summary>
        /// Creates a new instance of the <see cref="LocalSecurityTokenServiceClient"/> class
        /// </summary>
        /// <param name="config"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public LocalSecurityTokenServiceClient(SecurityTokenServiceClientConfiguration config, RequestSecurityToken templateRequest)
        {
            // Check input arguments
            if (config == null) throw new ArgumentNullException(nameof(config));
            // X509FindType cannot be tested below because default value is FindByThumbprint
            if (config.WscCertificate == null) throw new ArgumentException("ClientCertificate");
            if (config.StsCertificate == null) throw new ArgumentException("StsCertificate");
            _templateRequest = templateRequest;

            _config = config;
        }


        /// <summary>
        /// <see cref="ISecurityTokenServiceClient.GetServiceToken"/>
        /// </summary>
        /// <param name="serviceIdentifier"></param>
        /// <param name="keyType"></param>
        public SecurityToken GetServiceToken(string serviceIdentifier, KeyType keyType)
        {
            return GetTokenInternal(null, serviceIdentifier, keyType, _config.ServiceTokenUrl);
        }


        /// <inheritdoc />
        public SecurityToken GetBootstrapTokenFromAuthenticationToken(SecurityToken authenticationToken)
        {
            return GetTokenInternal(authenticationToken, _config.WscIdentifier, KeyType.HolderOfKey, _config.BootstrapTokenFromAuthenticationTokenUrl);
        }


        /// <summary>
        /// <see cref="ISecurityTokenServiceClient.GetIdentityTokenFromBootstrapToken"/>
        /// </summary>
        public SecurityToken GetIdentityTokenFromBootstrapToken(SecurityToken bootstrapToken, string serviceIdentifier,
            KeyType keyType)
        {
            if (bootstrapToken == null) throw new ArgumentNullException(nameof(bootstrapToken));
            if (serviceIdentifier == null) throw new ArgumentNullException(nameof(serviceIdentifier));

            return GetTokenInternal(bootstrapToken, serviceIdentifier, keyType,
                _config.IdentityTokenFromBootstrapTokenUrl);
        }




        private SecurityToken GetTokenInternal(SecurityToken actAs, string serviceIdentifier, KeyType keyType, Uri serviceUri)
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
                var stsBinding = new OioWsTrustBinding();
                if (_config.SendTimeout.HasValue)
                {
                    Logger.Instance.Warning($"RequestToken send timeout set to {_config.SendTimeout.Value}");
                    stsBinding.SendTimeout = _config.SendTimeout.Value;
                }

                // Setup channel factory and apply client credentials
                var factory = new WSTrustChannelFactory(stsBinding, new EndpointAddress(serviceUri));
                factory.TrustVersion = TrustVersion.WSTrust13;
                factory.Credentials.ClientCertificate.Certificate = _config.WscCertificate;

                // Create token request
                // UseKey and KeyType are only set in order for WCF to know which proof token to use when signing the request sent from WSC to WSP. Hence, UseKey and KeyType are actually sent to the NemLogin-STS but not used for anything by the STS.
                var requestSecurityToken = new RequestSecurityToken
                {
                    RequestType = RequestTypes.Issue,
                    AppliesTo = _templateRequest.AppliesTo,
                    Claims = { Dialect = _templateRequest.Claims.Dialect },
                    // TokenType is optional according to [NEMLOGIN-STSRULES]. If specified it must contain the value http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0 which is the only type NemLogin STS supports.
                    // We specify it in case that NemLogin STS supports other token types in the future.
                    // Currently if TokenType is not specified ... then TokenType is also not specified in the RequestSecurityTokenResponse (RSTR). According to spec it should always be specified in the RSTR. Specifying TokenType in the RST triggers the TokenType to be specified in the RSTR.
                    TokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0",

                };

                foreach (var claim in _templateRequest.Claims)
                {
                    requestSecurityToken.Claims.Add(claim);
                }

                requestSecurityToken.SignatureAlgorithm = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";

                if (keyType == KeyType.Bearer)
                {
                    requestSecurityToken.KeyType = KeyTypes.Bearer;
                    requestSecurityToken.UseKey = null;
                }
                else if (keyType == KeyType.HolderOfKey)
                {
                    // Proof key indicated - request a holder-of-key token using the proof key
                    requestSecurityToken.KeyType = KeyTypes.Asymmetric;
                    requestSecurityToken.UseKey = new UseKey(new X509SecurityToken(_config.WscCertificate));
                }
                else throw new ArgumentException("Unknown/unsupported key type", nameof(keyType));


                // Lifetime is only specified if it has been configured. Should result in a default life time (8 hours) on issued token if not specified. If specified, STS is not obligated to honor this range and may return a token with a shorter life time in RSTR.
                var currentTimeUtc = DateTime.UtcNow;
                if (_config.TokenLifeTime.HasValue)
                {
                    requestSecurityToken.Lifetime = new Lifetime(null, currentTimeUtc.Add(_config.TokenLifeTime.Value));
                }

                // ActAs is only set if a bootstrap token is supplied.
                if (actAs != null)
                {
                    // First check validity before using it.
                    if (actAs.ValidTo < currentTimeUtc)
                        throw new ArgumentException(
                            $"Bootstrap token life time has expired. Please renew before trying again. Bootstrap token ID: {actAs.Id}, Valid to: {actAs.ValidTo}, Current time: {currentTimeUtc}");
                    requestSecurityToken.ActAs = new SecurityTokenElement(actAs);
                }

                // Request token and return
                var channel = factory.CreateChannel();
                var securityToken = channel.Issue(requestSecurityToken);

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