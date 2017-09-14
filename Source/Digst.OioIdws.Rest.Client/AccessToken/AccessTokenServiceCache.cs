using System;
using System.Collections.Specialized;
using System.IdentityModel.Tokens;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace Digst.OioIdws.Rest.Client.AccessToken
{
    /// <summary>
    /// <see cref="IAccessTokenService"/>
    /// This implementation acts as a proxy to <see cref="AccessTokenService"/> and caches the token from the AS automatically according to the token expiration time.
    /// Access tokens are cached across instances of <see cref="OioIdwsClient"/>. Thus, even if one instance is created pr. request the access token has been cached.
    /// </summary>
    public class AccessTokenServiceCache : IAccessTokenService
    {
        private readonly OioIdwsClient _client;
        private readonly IAccessTokenService _tokenService;
        private static readonly MemoryCache TokenCache = new MemoryCache(typeof(AccessTokenServiceCache).FullName, new NameValueCollection{{"pollingInterval", "00:00:30"}});
        private readonly TimeSpan _cacheClockSkew;
        
        public AccessTokenServiceCache(OioIdwsClient client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            _client = client;
            _tokenService = new AccessTokenService(_client.Settings);
            _cacheClockSkew = _client.Settings.CacheClockSkew;
        }

        /// <summary>
        /// CacheKey could not be created at construction time because <see cref="OioIdwsClient.BootstrapToken"/> is initialized at this point in time.
        /// </summary>
        private string GetCacheKey()
        {
            return _client.BootstrapToken != null ? _client.BootstrapToken.Id + _client.Settings.AudienceUri : _client.Settings.AudienceUri.ToString();
        }

        async Task<AccessToken> IAccessTokenService.GetTokenAsync(GenericXmlSecurityToken securityToken, CancellationToken cancellationToken)
        {
            var accessToken = GetCachedAccessTokenIfNotExpired();

            if (accessToken == null)
            {
                accessToken = await _tokenService.GetTokenAsync(securityToken, cancellationToken);
                TokenCache.Add(new CacheItem(GetCacheKey(), accessToken),
                    new CacheItemPolicy {AbsoluteExpiration = DateTime.UtcNow + accessToken.ExpiresIn - _cacheClockSkew});
            }

            return accessToken;
        }

        /// <summary>
        /// Returns a cached token if one valid access token already exists.
        /// </summary>
        public AccessToken GetCachedAccessTokenIfNotExpired()
        {
            var cacheKey = GetCacheKey();
            var accessToken = (AccessToken)TokenCache.Get(cacheKey);

            return accessToken;
        }
    }
}