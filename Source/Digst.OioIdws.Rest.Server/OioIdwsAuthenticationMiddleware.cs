using System;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security.Infrastructure;
using Owin;

namespace Digst.OioIdws.Rest.Server
{
    public class OioIdwsAuthenticationMiddleware : AuthenticationMiddleware<OioIdwsAuthenticationOptions>
    {
        private readonly ILogger _logger;

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

            if (options.AccessTokenRetrievalEndpoint == null)
            {
                throw new ArgumentException($"The '{nameof(options.AccessTokenRetrievalEndpoint)}' option must be provided");
            }

            if (options.IdentityBuilder == null)
            {
                options.IdentityBuilder = new IdentityBuilder();
            }

            _logger = app.CreateLogger<OioIdwsAuthenticationMiddleware>();
        }

        protected override AuthenticationHandler<OioIdwsAuthenticationOptions> CreateHandler()
        {
            return new OioIdwsAuthenticationHandler(_logger);
        }
    }
}