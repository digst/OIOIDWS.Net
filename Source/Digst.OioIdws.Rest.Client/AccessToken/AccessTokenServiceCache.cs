using System;
using System.Collections.Specialized;
using System.IdentityModel.Tokens;
using System.Runtime.Caching;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Digst.OioIdws.Rest.Client.AccessToken
{
    /// <summary>
    /// <see cref="IAccessTokenService"/>
    /// This implementation acts as a proxy to <see cref="AccessTokenService"/> and caches the token from the AS automatically according to the token expiration time.
    /// Access tokens are cached across instances of <see cref="OioIdwsClient"/>. Thus, even if one instance is created pr. request the access token has been cached.
    /// </summary>
    internal class AccessTokenServiceCache : IAccessTokenService
    {
        private readonly IAccessTokenService _tokenService;
        private static readonly MemoryCache TokenCache = new MemoryCache(typeof(AccessTokenServiceCache).FullName, new NameValueCollection{{"pollingInterval", "00:00:30"}});
        private readonly TimeSpan _cacheClockSkew = TimeSpan.FromMinutes(5);

        internal AccessTokenServiceCache(OioIdwsClientSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            _tokenService = new AccessTokenService(settings);
            _cacheClockSkew = settings.CacheClockSkew;
        }

        public async Task<AccessToken> GetTokenAsync(GenericXmlSecurityToken securityToken, CancellationToken cancellationToken)
        {
            string cacheKey;

            var tokenAsString = securityToken.TokenXml.InnerXml;
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(tokenAsString));

                // Format hash as a hexidecimal string.
                cacheKey = new SoapHexBinary(hash).ToString();
            }

            var accessToken = (AccessToken) TokenCache.Get(cacheKey);

            if (accessToken == null)
            {
                accessToken = await _tokenService.GetTokenAsync(securityToken, cancellationToken);
                TokenCache.Add(new CacheItem(cacheKey, accessToken),
                    new CacheItemPolicy {AbsoluteExpiration = DateTime.UtcNow + accessToken.ExpiresIn - _cacheClockSkew});
            }

            return accessToken;
        }
    }
}