using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using Digst.OioIdws.Common.Logging;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.OioWsTrust.ProtocolChannel;

namespace Digst.OioIdws.Healthcare.Wsc
{

    /// <summary>
    /// A client for standard WS-Trust security token service
    /// </summary>
    /// <seealso cref="Digst.OioIdws.OioWsTrust.IOioSecurityTokenServiceClient" />
    public class StandardSecurityTokenServiceClient : IOioSecurityTokenServiceClient
    {
        private readonly ISecurityTokenServiceClientConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardSecurityTokenServiceClient"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public StandardSecurityTokenServiceClient(ISecurityTokenServiceClientConfiguration config)
        {
            _config = config;
        }


        /// <summary>
        /// Gets the bootstrap token from authentication token.
        /// </summary>
        /// <param name="authenticationToken">The authentication token.</param>
        /// <returns></returns>
        public SecurityToken GetBootstrapTokenFromAuthenticationToken(SecurityToken authenticationToken)
        {
            return GetTokenInternal(_config.BootstrapTokenFromAuthenticationTokenUrl, authenticationToken, KeyType.HolderOfKey, _config.StsIdentifier, null);
        }


        /// <summary>
        /// Gets the identity token from bootstrap token.
        /// </summary>
        /// <param name="bootstrapToken">The bootstrap token.</param>
        /// <param name="serviceEntityId">The service entity identifier.</param>
        /// <param name="claims">The claims.</param>
        /// <param name="keyType">Type of the key.</param>
        /// <returns></returns>
        public SecurityToken GetIdentityTokenFromBootstrapToken(SecurityToken bootstrapToken, string serviceEntityId, RequestClaimCollection claims, KeyType keyType)
        {
            Logger.Instance.Trace($"Get IDT from BST: {bootstrapToken} {claims.ToString()}");
            return GetTokenInternal(_config.IdentityTokenFromBootstrapTokenUrl, bootstrapToken, keyType, serviceEntityId, claims);
        }


        /// <summary>
        /// Gets a service token.
        /// </summary>
        /// <param name="serviceEntityId">The service entity identifier.</param>
        /// <param name="claims">The claims.</param>
        /// <returns></returns>
        public SecurityToken GetServiceToken(string serviceEntityId, RequestClaimCollection claims)
        {
            return GetTokenInternal(_config.ServiceTokenUrl, null, KeyType.HolderOfKey, serviceEntityId, claims);
        }



        private SecurityToken GetTokenInternal(Uri stsUrl, SecurityToken actAs, KeyType keyType, string appliesTo, RequestClaimCollection claims = null)
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
                    //Logger.Instance.Warning($"RequestToken send timeout set to {_config.SendTimeout.Value}");
                    stsBinding.SendTimeout = _config.SendTimeout.Value;
                }
                stsBinding.Elements.Add(new OioWsTrustBindingElement(_config.StsCertificate, new LocalWsTrustMessageTransformer(stsUrl.ToString())));
                stsBinding.Elements.Add(new TextMessageEncodingBindingElement(MessageVersion.Soap11WSAddressing10, Encoding.UTF8));
                // ManualAddressing must be true in order to make sure that wsa header elements are not altered in the HttpsTransportChannel which happens after xml elements have been digitally signed.
                stsBinding.Elements.Add(new HttpsTransportBindingElement() { ManualAddressing = true });

                // Setup channel factory and apply client credentials
                var factory =
                    new WSTrustChannelFactory(stsBinding, new EndpointAddress(stsUrl))
                    {
                        TrustVersion = TrustVersion.WSTrust13,
                        Credentials = { ClientCertificate = { Certificate = _config.WscCertificate } }
                    };

                // Create token request
                // UseKey and KeyType are only set in order for WCF to know which proof token to use when signing the request sent from WSC to WSP. Hence, UseKey and KeyType are actually sent to the NemLogin-STS but not used for anything by the STS.
                var requestSecurityToken = new RequestSecurityToken
                {
                    RequestType = RequestTypes.Issue,
                    AppliesTo = appliesTo != null ? new EndpointReference(appliesTo) : null,
                    // TokenType is optional according to [NEMLOGIN-STSRULES]. If specified it must contain the value http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0 which is the only type NemLogin STS supports.
                    // We specify it in case that NemLogin STS supports other token types in the future.
                    // Currently if TokenType is not specified ... then TokenType is also not specified in the RequestSecurityTokenResponse (RSTR). According to spec it should always be specified in the RSTR. Specifying TokenType in the RST triggers the TokenType to be specified in the RSTR.
                    TokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0",
                    SignatureAlgorithm = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256",
                };

                if (claims != null)
                {
                    requestSecurityToken.Claims.Dialect = claims.Dialect;
                    foreach (var claim in claims)
                    {
                        requestSecurityToken.Claims.Add(claim);
                    }
                }


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
                    requestSecurityToken.Lifetime = new Lifetime(null,
                        currentTimeUtc.Add(_config.TokenLifeTime.Value));
                }

                // ActAs is only set if an ActAs token (typically AUT or BST) is supplied.
                if (actAs != null)
                {
                    // First check validity before using it.
                    if (actAs.ValidTo < currentTimeUtc)
                        throw new ArgumentException(
                            $"ActAs token life time has expired. Please renew before trying again. ActAs token ID: {actAs.Id}, Valid to: {actAs.ValidTo}, Current time: {currentTimeUtc}");
                    requestSecurityToken.ActAs = new SecurityTokenElement(actAs);
                }

                requestSecurityToken.AppliesTo = appliesTo != null ? new EndpointReference(appliesTo) {} : null;



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