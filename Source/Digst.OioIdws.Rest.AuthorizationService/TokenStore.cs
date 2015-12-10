using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Digst.OioIdws.Rest.AuthorizationService
{
    public class MemoryTokenStore : ITokenStore
    {
        private readonly ConcurrentDictionary<string, SamlToken> _store = new ConcurrentDictionary<string, SamlToken>(); 

        public Task StoreTokenAsync(string accessToken, SamlToken samlToken)
        {
            _store.AddOrUpdate(accessToken, samlToken, (key, existingToken) => samlToken);
            return Task.FromResult(0);
        }
    }
}