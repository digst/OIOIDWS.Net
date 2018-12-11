using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens;
using Digst.OioIdws.OioWsTrust.TokenCache;
using System.Diagnostics;
using System.IdentityModel.Metadata;
using System.IdentityModel.Protocols.WSTrust;
using System.Security.Cryptography.X509Certificates;

namespace Digst.OioIdws.OioWsTrust
{
    /// <summary>
    /// This class allows for injection of a token cache of type <see cref="ITokenCache"/> and an StsTokenService of type <see cref="ISecurityTokenServiceClient"/>
    /// </summary>
    public class CachedSecurityTokenServiceClient : ISecurityTokenServiceClient
    {
        private readonly ISecurityTokenServiceClient _stsTokenService;
        private readonly ITokenCache _bootstrapTokenCache;
        private readonly ITokenCache _identityTokenCache;
        private readonly ITokenCacheHitMissLogger _tokenCacheHitMissLogger;

        /// <summary>
        /// Just your everyday CachedStsTokenService constructor
        /// </summary>
        /// <param name="securityTokenService">The service for which the responses should be cached</param>
        /// <param name="bootstrapTokenCache">A cache which will hold cached bootstrap tokens. Used when authentication tokens are exchanged for bootstrap tokens.</param>
        /// <param name="identityTokenCache">A cache which will hold cached identity (service) tokens. Used when bootstrap tokens are exchanged for identity tokens.</param>
        public CachedSecurityTokenServiceClient(ISecurityTokenServiceClient securityTokenService, ITokenCache bootstrapTokenCache, ITokenCache identityTokenCache, ITokenCacheHitMissLogger tokenCacheHitMissLogger = null)
        {
            _stsTokenService = securityTokenService;
            _bootstrapTokenCache = bootstrapTokenCache;
            _identityTokenCache = identityTokenCache;
            _tokenCacheHitMissLogger = tokenCacheHitMissLogger;
        }

        /// <summary>
        /// Get a GenericXmlSecurityToken from cache or STS without a token as ActAs
        /// </summary>
        /// <returns>GenericXmlSecurityToken</returns>
        public SecurityToken GetServiceToken(string serviceIdentifier, KeyType keyType)
        {
            string cacheKey = $"{serviceIdentifier}:{keyType}";

            var secToken = TryGetFromCache(cacheKey, _identityTokenCache);
            if (secToken is null)
            {
                _tokenCacheHitMissLogger?.CacheMiss(cacheKey);
                secToken = _stsTokenService.GetServiceToken(serviceIdentifier, keyType);
                PutTokenInCache(cacheKey, secToken, _identityTokenCache);
            }
            else
            {
                _tokenCacheHitMissLogger?.CacheHit(cacheKey);
            }

            return secToken;
        }

        /// <summary>
        /// Get a GenericXmlSecurityToken from cache or STS with a token as ActAs
        /// </summary>
        /// <returns>GenericXmlSecurityToken</returns>
        public SecurityToken GetIdentityTokenFromBootstrapToken(SecurityToken bootstrapToken, string serviceIdentifier, KeyType keyType)
        {
            string cacheKey = $"{bootstrapToken.Id}:{serviceIdentifier}:{keyType}";

            var secToken = TryGetFromCache(cacheKey, _identityTokenCache);
            if (secToken is null)
            {
                _tokenCacheHitMissLogger?.CacheMiss(cacheKey);
                secToken = _stsTokenService.GetIdentityTokenFromBootstrapToken(bootstrapToken, serviceIdentifier, keyType);
                PutTokenInCache(cacheKey, secToken, _identityTokenCache);
            }
            else
            {
                _tokenCacheHitMissLogger?.CacheHit(cacheKey);
            }

            return secToken;
        }

        /// <summary>
        /// Get a GenericXmlSecurityToken from cache or STS with an authenticationToken as ActAs
        /// </summary>
        /// <returns>GenericXmlSecurityToken</returns>
        public SecurityToken GetBootstrapTokenFromAuthenticationToken(SecurityToken authenticationToken)
        {
            string cacheKey = authenticationToken.Id;

            var secToken = TryGetFromCache(cacheKey, _bootstrapTokenCache);
            if (secToken is null)
            {
                _tokenCacheHitMissLogger?.CacheMiss(cacheKey);
                secToken = _stsTokenService.GetBootstrapTokenFromAuthenticationToken(authenticationToken);
                PutTokenInCache(cacheKey, secToken, _bootstrapTokenCache);
            }
            else
            {
                _tokenCacheHitMissLogger?.CacheHit(cacheKey);
            }

            return secToken;
        }

        private SecurityToken TryGetFromCache(string cacheKey, ITokenCache cache)
        {
            return cache.IsInCache(cacheKey) ? cache.GetToken(cacheKey) : null;
        }

        private void PutTokenInCache(string cacheKey, SecurityToken securityToken, ITokenCache cache)
        {
            cache.PutToken(cacheKey, securityToken);
        }
    }
}
