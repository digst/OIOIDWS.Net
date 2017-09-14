using System;
using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Rest.Client.AccessToken;

namespace Digst.OioIdws.Rest.Client
{
    /// <summary>
    /// Class which represents a client/WSC in the OIOIDWS.Net REST scenario.
    /// It can be used to retrieve STS tokens and access tokens in a custom middleware implementation.
    /// It also gives middleware implementation <see cref="OioIdwsRequestHandler"/> that supports <see cref="HttpClient"/>.
    /// </summary>
    public class OioIdwsClient
    {
        private readonly IStsTokenService _stsTokenService;
        private readonly IAccessTokenService _accessTokenService;
        
        /// <summary>
        /// The settings that is used for communicating with STS and AS.
        /// </summary>
        public OioIdwsClientSettings Settings { get; }

        /// <summary>
        /// The bootstrap token that the client is initialized with.
        /// </summary>
        public SecurityToken BootstrapToken { get; } 


        /// <summary>
        /// Used in the bootstrap case scenario.
        /// One instance of <see cref="OioIdwsClient"/> must be created for each user.
        /// </summary>
        public OioIdwsClient(OioIdwsClientSettings settings, SecurityToken bootstrapToken) : this (settings)
        {
            if (bootstrapToken == null) throw new ArgumentNullException(nameof(bootstrapToken));
            BootstrapToken = bootstrapToken;
        }

        /// <summary>
        /// Used in the signature case scenario
        /// </summary>
        public OioIdwsClient(OioIdwsClientSettings settings)
        {
            Settings = settings;
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (settings.ClientCertificate == null)
            {
                throw new ArgumentNullException(nameof(settings.ClientCertificate));
            }

            if (!settings.ClientCertificate.HasPrivateKey)
            {
                throw new ArgumentException("You must have access to the private key of the ClientCertificate", nameof(settings.ClientCertificate));
            }

            if (settings.SecurityTokenService == null)
            {
                throw new ArgumentNullException(nameof(settings.SecurityTokenService));
            }

            if (settings.SecurityTokenService.Certificate == null)
            {
                throw new ArgumentNullException(nameof(settings.SecurityTokenService.Certificate), "Certificate for the SecurityTokenService must be set");
            }

            var tokenServiceConfiguration = new StsTokenServiceConfiguration
            {
                ClientCertificate = Settings.ClientCertificate,
                StsCertificate = Settings.SecurityTokenService.Certificate,
                StsEndpointAddress = Settings.SecurityTokenService.EndpointAddress.ToString(),
                TokenLifeTimeInMinutes = (int?) Settings.SecurityTokenService.TokenLifeTime.GetValueOrDefault().TotalMinutes,
                SendTimeout = Settings.SecurityTokenService.SendTimeout,
                WspEndpointId = Settings.AudienceUri.ToString()
            };

            if (settings.SecurityTokenService.CacheClockSkew.HasValue)
            {
                tokenServiceConfiguration.CacheClockSkew = settings.SecurityTokenService.CacheClockSkew.Value;
            }

            if (settings.SecurityTokenService.UseTokenCache)
            {
                _stsTokenService = new StsTokenServiceCache(tokenServiceConfiguration);
            }
            else
            {
                _stsTokenService = new StsTokenService(tokenServiceConfiguration);
            }

            if (settings.UseTokenCache)
            {
                _accessTokenService = new AccessTokenServiceCache(this);
            }
            else
            {
                _accessTokenService = new AccessTokenService(Settings);
            }
        }

        /// <summary>
        /// Creates a handler that takes care of issuing tokens and renewing tokens when expiring. 
        /// It can be used inside a HttpClient or similar that supports it.
        /// The handler is not thread-safe, but you can create multiple instances
        /// </summary>
        /// <returns></returns>
        public HttpMessageHandler CreateMessageHandler()
        {
            return new OioIdwsRequestHandler(this, (_accessTokenService as AccessTokenServiceCache)?.GetCachedAccessTokenIfNotExpired());
        }

        /// <summary>
        /// Retrives a STS token based on <see cref="Settings"/>
        /// </summary>
        /// <returns></returns>
        public GenericXmlSecurityToken GetSecurityToken()
        {
            if (BootstrapToken != null)
            {
                return (GenericXmlSecurityToken)_stsTokenService.GetTokenWithBootstrapToken(BootstrapToken);
            }
            else
            {
                return (GenericXmlSecurityToken)_stsTokenService.GetToken();
            }
        }

        /// <summary>
        /// Retrives an access token based on <see cref="Settings"/>
        /// </summary>
        /// <returns></returns>
        public async Task<AccessToken.AccessToken> GetAccessTokenAsync(
            GenericXmlSecurityToken securityToken,
            CancellationToken cancellationToken)
        {
            return await _accessTokenService.GetTokenAsync(securityToken, cancellationToken);
        }
    }
}
