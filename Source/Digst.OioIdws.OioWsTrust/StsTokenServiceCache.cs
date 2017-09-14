using System;
using System.Collections.Specialized;
using System.IdentityModel.Tokens;
using System.Runtime.Caching;

namespace Digst.OioIdws.OioWsTrust
{
    /// <summary>
    /// <see cref="IStsTokenService"/>
    /// This implementation acts as a proxy to <see cref="StsTokenService"/> and caches the token from the STS automatically according to the token expiration time.
    /// </summary>
    public class StsTokenServiceCache : IStsTokenService
    {
        private readonly IStsTokenService _stsTokenService;
        private static readonly MemoryCache TokenCache = new MemoryCache(typeof(StsTokenServiceCache).FullName, new NameValueCollection { { "pollingInterval", "00:00:30" } });
        private readonly TimeSpan _cacheClockSkew;
        private readonly string _wspEndpointId;

        public StsTokenServiceCache(StsTokenServiceConfiguration config)
        {
            _stsTokenService = new StsTokenService(config);
            _cacheClockSkew = config.CacheClockSkew;
            _wspEndpointId = config.WspEndpointId;
        }

        /// <summary>
        /// <see cref="IStsTokenService.GetToken()"/>
        /// Furtermore this implementation caches the token automatically according to the token expiration time.
        /// </summary>
        public SecurityToken GetToken()
        {
            return GetTokenInternal(null);
        }

        /// <summary>
        /// <see cref="IStsTokenService.GetTokenWithBootstrapToken"/>
        /// Furtermore this implementation caches the token automatically according to the token expiration time.
        /// </summary>
        public SecurityToken GetTokenWithBootstrapToken(SecurityToken bootstrapToken)
        {
            if (bootstrapToken == null) throw new ArgumentNullException(nameof(bootstrapToken));

            return GetTokenInternal(bootstrapToken);
        }

        private SecurityToken GetTokenInternal(SecurityToken bootstrapToken)
        {
            var cacheKey = bootstrapToken != null ? bootstrapToken.Id + _wspEndpointId : _wspEndpointId;

            var securityToken = (SecurityToken) TokenCache.Get(cacheKey);

            if (securityToken == null)
            {
                if (bootstrapToken == null)
                {
                    securityToken = _stsTokenService.GetToken();
                }
                else
                {
                    securityToken = _stsTokenService.GetTokenWithBootstrapToken(bootstrapToken);
                }
                TokenCache.Add(new CacheItem(cacheKey, securityToken),
                    new CacheItemPolicy {AbsoluteExpiration = securityToken.ValidTo - _cacheClockSkew});
            }

            return securityToken;
        }
    }
}