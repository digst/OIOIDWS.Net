using System;
using System.IdentityModel.Tokens;
using System.Runtime.Caching;
using Digst.OioIdws.OioWsTrust.Utils;

namespace Digst.OioIdws.OioWsTrust
{
    /// <summary>
    /// <see cref="ITokenService"/>
    /// This implementation acts as a proxy to <see cref="TokenService"/> and caches the token from the STS automatically according to the token expiration time.
    /// </summary>
    public class TokenServiceCache : ITokenService
    {
        private readonly ITokenService _tokenService;
        private readonly MemoryCache _tokenCache = MemoryCache.Default;
        private const string TokenKey = "TokenKey";
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
            var securityToken = (SecurityToken)_tokenCache.Get(TokenKey);
            
            if (securityToken == null)
            {
                 securityToken = _tokenService.GetToken();
                _tokenCache.Add(new CacheItem(TokenKey, securityToken), new CacheItemPolicy{AbsoluteExpiration = securityToken.ValidTo - _cacheClockSkew });
            }

            return securityToken;
        }
    }
}