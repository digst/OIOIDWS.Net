using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;

namespace Digst.OioIdws.Rest.AuthorizationService.Storage
{
    public class MemorySecurityTokenStore : ISecurityTokenStore
    {
        private readonly ConcurrentDictionary<string, OioIdwsToken> _store = new ConcurrentDictionary<string, OioIdwsToken>(); 

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
            //todo better handling when token doesn't exist
            //todo handle token expired?
            OioIdwsToken oioIdwsToken;
            _store.TryGetValue(accessToken, out oioIdwsToken);
            return Task.FromResult(oioIdwsToken);
        }
    }
}