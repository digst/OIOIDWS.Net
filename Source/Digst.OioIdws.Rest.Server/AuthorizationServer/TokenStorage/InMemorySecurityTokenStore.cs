using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage
{
    /// <summary>
    /// In memory implementation of a <see cref="ISecurityTokenStore"/>. 
    /// Useful as long as the AuthorizationService is not going to scale across multiple servers.
    /// Expired access token will be automatically clean up.
    /// </summary>
    public class InMemorySecurityTokenStore : ISecurityTokenStore
    {
        private readonly ConcurrentDictionary<string, OioIdwsToken> _store = new ConcurrentDictionary<string, OioIdwsToken>();

        public InMemorySecurityTokenStore()
        {
            var timer = new Timer(Cleanup, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }
        
        private void Cleanup(object state)
        {
            foreach (var item in _store)
            {
                if (item.Value.ValidUntilUtc < DateTime.UtcNow)
                {
                    OioIdwsToken deletedValue;
                    _store.TryRemove(item.Key, out deletedValue);
                }
            }
        }

        public Task StoreTokenAsync(string accessToken, OioIdwsToken oioIdwsToken)
        {
            if (oioIdwsToken == null)
            {
                throw new ArgumentNullException(nameof(oioIdwsToken));
            }
            if (String.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentException("Argument is null or empty", nameof(accessToken));
            }

            var alreadyAdded = !_store.TryAdd(accessToken, oioIdwsToken);
            if (alreadyAdded)
            {
                throw new InvalidOperationException($"The access token '{accessToken}' has already been stored and it's not allowed to rewrite stored access tokens");
            }
            return Task.FromResult(0);
        }

        public Task<OioIdwsToken> RetrieveTokenAsync(string accessToken)
        {
            OioIdwsToken oioIdwsToken;
            _store.TryGetValue(accessToken, out oioIdwsToken);

            if (oioIdwsToken?.ValidUntilUtc > DateTime.UtcNow)
            {
                return Task.FromResult(oioIdwsToken);
            }

            return null;
        }
    }
}