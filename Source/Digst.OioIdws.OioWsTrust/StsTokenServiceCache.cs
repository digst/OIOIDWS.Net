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
    public class StsTokenServiceCache : StsTokenServiceBase
    {
        private readonly StsTokenServiceBase _stsTokenService;
        private static readonly MemoryCache TokenCache = new MemoryCache(typeof(StsTokenServiceCache).FullName, new NameValueCollection { { "pollingInterval", "00:00:30" } });
        private readonly TimeSpan _cacheClockSkew;
        private readonly string _wspEndpointId;

        /// <summary>
        /// Initializes a new instance of the <see cref="StsTokenServiceCache"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public StsTokenServiceCache(StsTokenServiceConfiguration config)
        {
            _stsTokenService = new StsTokenService(config);
            _cacheClockSkew = config.CacheClockSkew;
            _wspEndpointId = config.WspEndpointId;
        }

        /// <summary>
        /// Gets a token from the service
        /// </summary>
        /// <param name="stsAuthenticationCase">The STS authentication case.</param>
        /// <param name="authenticationToken">The authentication token (bootstrap or local -token).</param>
        /// <returns></returns>
        protected internal override SecurityToken GetToken(StsAuthenticationCase stsAuthenticationCase, SecurityToken authenticationToken)
        {
            // Generate a cache key. If an authentication token has been provided then prefix with the ID of that token; otherwise we just use the endpoint ID
            var cacheKey = authenticationToken != null ? authenticationToken.Id + _wspEndpointId : _wspEndpointId;

            var securityToken = (SecurityToken)TokenCache.Get(cacheKey);

            if (securityToken == null)
            {
                // cache miss
                securityToken = _stsTokenService.GetToken(stsAuthenticationCase, authenticationToken);
                TokenCache.Add(
                    new CacheItem(cacheKey, securityToken), 
                    new CacheItemPolicy
                    {
                        AbsoluteExpiration = securityToken.ValidTo - _cacheClockSkew
                    });
            }

            return securityToken;
        }

    }
}