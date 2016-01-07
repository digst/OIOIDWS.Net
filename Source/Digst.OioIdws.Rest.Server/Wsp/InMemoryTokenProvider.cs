using System;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Server.AuthorizationServer;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;
using Owin;

namespace Digst.OioIdws.Rest.Server.Wsp
{
    public class InMemoryTokenProvider : ITokenProvider
    {
        private ISecurityTokenStore _securityTokenStore;

        public void Initialize(IAppBuilder app, OioIdwsAuthenticationOptions options)
        {
            object tmp;
            if (!app.Properties.TryGetValue(OioIdwsAuthorizationServiceMiddleware.SecurityTokenStoreKey, out tmp) ||
                (_securityTokenStore = (tmp as ISecurityTokenStore)) == null)
            {
                throw new ArgumentException($"When option '{nameof(options.TokenProvider)}' is set to '{nameof(InMemoryTokenProvider)}', the {nameof(OioIdwsAuthorizationServiceMiddleware)} must be configured as well.");
            }
        }

        public async Task<OioIdwsToken> RetrieveTokenAsync(string accessToken)
        {
            return await _securityTokenStore.RetrieveTokenAsync(accessToken);
        }
    }
}