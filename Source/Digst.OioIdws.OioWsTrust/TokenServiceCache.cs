using System;
using System.IdentityModel.Tokens;
using System.Runtime.Caching;

namespace Digst.OioIdws.OioWsTrust
{
    /// <summary>
    /// <see cref="ITokenService"/>
    /// This implementation acts as a proxy to <see cref="TokenService"/> and caches the token from the STS automatically according to the token expiration time.
    /// </summary>
    public class TokenServiceCache : ITokenService
    {
        private readonly ITokenService _tokenService;
        private static readonly MemoryCache TokenCache = new MemoryCache(typeof(TokenServiceCache).FullName);
        private const string WscTokenKey = "WscTokenKey";
        private readonly TimeSpan _cacheClockSkew;

        public TokenServiceCache(TokenServiceConfiguration config)
        {
            _tokenService = new TokenService(config);
            _cacheClockSkew = config.CacheClockSkew;
        }

        /// <summary>
        /// <see cref="ITokenService.GetToken()"/>
        /// Furtermore this implementation caches the token automatically according to the token expiration time.
        /// </summary>
        public SecurityToken GetToken()
        {
            return GetTokenInternal(null);
        }

        /// <summary>
        /// <see cref="ITokenService.GetTokenWithBootstrapToken"/>
        /// Furtermore this implementation caches the token automatically according to the token expiration time.
        /// </summary>
        public SecurityToken GetTokenWithBootstrapToken(SecurityToken bootstrapToken)
        {
            if (bootstrapToken == null) throw new ArgumentNullException(nameof(bootstrapToken));

            return GetTokenInternal(bootstrapToken);
        }

        private SecurityToken GetTokenInternal(SecurityToken bootstrapToken)
        {
            var cacheKey = bootstrapToken != null ? bootstrapToken.Id : WscTokenKey;

            var securityToken = (SecurityToken) TokenCache.Get(cacheKey);

            if (securityToken == null)
            {
                securityToken = _tokenService.GetToken();
                TokenCache.Add(new CacheItem(cacheKey, securityToken),
                    new CacheItemPolicy {AbsoluteExpiration = securityToken.ValidTo - _cacheClockSkew});
            }

            return securityToken;
        }
    }
}