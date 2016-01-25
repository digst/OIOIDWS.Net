using System;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Server.AuthorizationServer;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;
using Microsoft.Owin.Logging;
using Owin;

namespace Digst.OioIdws.Rest.Server.Wsp
{
    public class InMemoryTokenProvider : ITokenProvider
    {
        private Func<string, Task<OioIdwsToken>> _tokenRetrieval;

        public void Initialize(IAppBuilder app, OioIdwsAuthenticationOptions options, ILogger logger)
        {
            object tmp;
            if (!app.Properties.TryGetValue(OioIdwsAuthorizationServiceMiddleware.TokenRetrievalKey, out tmp) ||
                (_tokenRetrieval = (tmp as Func<string, Task<OioIdwsToken>>)) == null)
            {
                throw new ArgumentException($"When option '{nameof(options.TokenProvider)}' is set to '{nameof(InMemoryTokenProvider)}', the {nameof(OioIdwsAuthorizationServiceMiddleware)} must be configured as well.");
            }
        }

        public async Task<RetrieveTokenResult> RetrieveTokenAsync(string accessToken)
        {
            try
            {
                var token = await _tokenRetrieval(accessToken);
                return new RetrieveTokenResult(token);
            }
            catch (AccessTokenExpiredException)
            {
                return RetrieveTokenResult.AsExpired();
            }
        }
    }
}