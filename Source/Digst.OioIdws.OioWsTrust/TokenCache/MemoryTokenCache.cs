using System;
using System.Collections.Specialized;
using System.IdentityModel.Tokens;
using System.Runtime.Caching;
using Digst.OioIdws.Common.Logging;

namespace Digst.OioIdws.OioWsTrust.TokenCache
{
    /// <summary>
    /// This class implements <see cref="ITokenCache"/> with <see cref="MemoryCache"/> as backing cache
    /// </summary>
    public class MemoryTokenCache : ITokenCache
    {
        private static readonly MemoryCache TokenCache = new MemoryCache(typeof(MemoryTokenCache).FullName, new NameValueCollection { { "pollingInterval", "00:00:30" } });
        private readonly TimeSpan _timeSpan;

        /// <summary>
        /// Your run-of-the-mill constructor. This version takes sets the cache clock skew to 0
        /// </summary>
        public MemoryTokenCache() : this(TimeSpan.FromSeconds(0))
        {
        }

        /// <summary>
        /// Your run-of-the-mill constructor. This version takes a <see cref="TimeSpan"/> as input to set the cache clock skew
        /// </summary>
        /// <param name="clockSkew"><see cref="TimeSpan"/></param>
        public MemoryTokenCache(TimeSpan clockSkew)
        {
            _timeSpan = clockSkew;
        }

        /// <summary>
        /// Gets the <see cref="SecurityToken"/> associated with cacheKey. Returns null if miss
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns><see cref="SecurityToken"/> or null</returns>
        public SecurityToken GetToken(string cacheKey)
        {
            var securityToken = (SecurityToken)TokenCache.Get(cacheKey);

            Logger.Instance?.Trace(cacheKey + " was " + (securityToken != null ? "" : "not ") + "retrieved from cache");

            return securityToken;
        }

        /// <summary>
        /// Checks cache for item with cacheKey without loading item if found
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns><see cref="Boolean"/></returns>
        public bool IsInCache(string cacheKey)
        {
            var isFound = TokenCache.Contains(cacheKey);

            if (isFound)
            {
                Logger.Instance?.Trace($"Token was found in cache. Cache key: {cacheKey}");
            }
            else
            {
                Logger.Instance?.Trace($"Token was NOT found in cache. Cache key: {cacheKey}");
            }

            return isFound;
        }

        /// <summary>
        /// Adds the <see cref="SecurityToken"/> to cache with an absolute expiration of <see cref="SecurityToken.ValidTo"/> minus the configured clock skew
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="securityToken"><see cref="SecurityToken"/></param>
        public void PutToken(string cacheKey, SecurityToken securityToken)
        {
            // set expiration to 5 minutes before expiry;
            var expireAt = securityToken.ValidTo - TimeSpan.FromMinutes(5);
            if (expireAt < DateTime.UtcNow) return; // do not cache an almost expired token
            TokenCache.Add(cacheKey, securityToken, expireAt);
            Logger.Instance?.Trace($"Token added to cache. Cache key: {cacheKey}. Expiration: {expireAt:s}");
        }
    }
}
