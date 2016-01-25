using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;

namespace Digst.OioIdws.Rest.Server.Wsp
{
    /// <summary>
    /// Caches tokens in memory up to the point where they are set to expire
    /// </summary>
    public class InMemoryTokenCache : ITokenCache
    {
        private readonly ConcurrentDictionary<string, OioIdwsToken> _cache = new ConcurrentDictionary<string, OioIdwsToken>();

        public InMemoryTokenCache()
        {
            var timer = new Timer(Cleanup, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }

        private void Cleanup(object state)
        {
            foreach (var item in _cache)
            {
                if (item.Value.ExpiresUtc < DateTimeOffset.UtcNow)
                {
                    OioIdwsToken deletedValue;
                    _cache.TryRemove(item.Key, out deletedValue);
                }
            }
        }

        public Task StoreAsync(string accessToken, OioIdwsToken token)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            _cache.TryAdd(accessToken, token);
            return Task.FromResult(0);
        }

        public Task<OioIdwsToken> RetrieveAsync(string accessToken)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            OioIdwsToken token;
            _cache.TryGetValue(accessToken, out token);
            return Task.FromResult(token);
        }
    }
}