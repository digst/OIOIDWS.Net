using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Rest.Client.AccessToken;
using Digst.OioIdws.Rest.Common;
using Newtonsoft.Json.Linq;

namespace Digst.OioIdws.Rest.Client
{
    public class OioIdwsClient
    {
        private readonly SecurityToken _bootstrapToken;
        private readonly IStsTokenService _stsTokenService;
        private readonly IAccessTokenService _accessTokenService;
        public OioIdwsClientSettings Settings { get; }

        /// <summary>
        /// Used in the bootstrap case scenario.
        /// One instance of <see cref="OioIdwsClient"/> must be created for each user.
        /// </summary>
        /// <param name="settings"></param>
        public OioIdwsClient(OioIdwsClientSettings settings, SecurityToken bootstrapToken) : this (settings)
        {
            if (bootstrapToken == null) throw new ArgumentNullException(nameof(bootstrapToken));
            _bootstrapToken = bootstrapToken;
        }

        /// <summary>
        /// Used in the signature case scenario
        /// </summary>
        /// <param name="settings"></param>
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
                _accessTokenService = new AccessTokenServiceCache(Settings);
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
            return new OioIdwsRequestHandler(this);
        }

        public GenericXmlSecurityToken GetSecurityToken()
        {
            if (_bootstrapToken != null)
            {
                return (GenericXmlSecurityToken)_stsTokenService.GetTokenWithBootstrapToken(_bootstrapToken);
            }
            else
            {
                return (GenericXmlSecurityToken)_stsTokenService.GetToken();
            }
        }

        public async Task<AccessToken.AccessToken> GetAccessTokenAsync(
            GenericXmlSecurityToken securityToken,
            CancellationToken cancellationToken)
        {
            return await _accessTokenService.GetTokenAsync(securityToken, cancellationToken);
        }
    }
}
