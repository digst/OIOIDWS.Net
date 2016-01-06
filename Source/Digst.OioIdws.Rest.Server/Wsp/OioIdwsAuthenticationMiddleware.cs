using System;
using Digst.OioIdws.Rest.Server.AuthorizationServer;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security.Infrastructure;
using Owin;

namespace Digst.OioIdws.Rest.Server.Wsp
{
    public class OioIdwsAuthenticationMiddleware : AuthenticationMiddleware<OioIdwsAuthenticationOptions>
    {
        private readonly ILogger _logger;
        private readonly ISecurityTokenStore _securityTokenStore;

        public OioIdwsAuthenticationMiddleware(
            OwinMiddleware next, 
            IAppBuilder app,
            OioIdwsAuthenticationOptions options) : base(next, options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.TokenRetrievalMethod == TokenRetrievalMethod.InMemory)
            {
                object tmp;
                if (!app.Properties.TryGetValue(OioIdwsAuthorizationServiceMiddleware.SecurityTokenStoreKey, out tmp) ||
                    (_securityTokenStore = (tmp as ISecurityTokenStore)) == null)
                {
                    throw new ArgumentException($"When option '{nameof(options.TokenRetrievalMethod)}' is set to '{TokenRetrievalMethod.InMemory}', the {nameof(OioIdwsAuthorizationServiceMiddleware)} must be configured as well.");
                }
            }
            else
            {
                if (options.AccessTokenRetrievalEndpoint == null)
                {
                    throw new ArgumentException($"The '{nameof(options.AccessTokenRetrievalEndpoint)}' option must be provided");
                }
            }

            if (options.IdentityBuilder == null)
            {
                options.IdentityBuilder = new IdentityBuilder();
            }

            _logger = app.CreateLogger<OioIdwsAuthenticationMiddleware>();
        }

        protected override AuthenticationHandler<OioIdwsAuthenticationOptions> CreateHandler()
        {
            return new OioIdwsAuthenticationHandler(_logger, _securityTokenStore);
        }
    }
}