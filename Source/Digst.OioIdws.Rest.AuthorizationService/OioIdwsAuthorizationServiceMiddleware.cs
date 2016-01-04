using System;
using Digst.OioIdws.Rest.AuthorizationService.Storage;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security.Infrastructure;
using Owin;

namespace Digst.OioIdws.Rest.AuthorizationService
{
    /// <summary>
    /// The middlware that acts as OIO IDWS AuthorizationService
    /// </summary>
    public class OioIdwsAuthorizationServiceMiddleware : AuthenticationMiddleware<OioIdwsAuthorizationServiceOptions>
    {
        private readonly ILogger _logger;

        public OioIdwsAuthorizationServiceMiddleware(
            OwinMiddleware next, 
            IAppBuilder app,
            OioIdwsAuthorizationServiceOptions options) 
            : base(next, options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.SecurityTokenStore == null)
            {
                options.SecurityTokenStore = new InMemorySecurityTokenStore();
            }

            if (options.ServiceTokenResolver == null)
            {
                throw new ArgumentException($"The '{nameof(options.ServiceTokenResolver)}' options must be set");
            }

            if (!options.AccessTokenIssuerPath.HasValue)
            {
                throw new ArgumentException($"The '{nameof(options.AccessTokenIssuerPath)}' options must be set");
            }

            if (!options.AccessTokenRetrievalPath.HasValue)
            {
                throw new ArgumentException($"The '{nameof(options.AccessTokenRetrievalPath)}' options must be set");
            }

            if (options.IssuerAudiences == null)
            {
                throw new ArgumentException($"The '{nameof(options.IssuerAudiences)}' option must be set");
            }

            _logger = app.CreateLogger<OioIdwsAuthorizationServiceMiddleware>();

            if (options.AccessTokenExpiration > TimeSpan.FromHours(1))
            {
                _logger.WriteWarning($"AccessTokenExpiration has been set to an expiration of {options.AccessTokenExpiration}. It is adviced to set it to one hour or less.");
            }
        }

        protected override AuthenticationHandler<OioIdwsAuthorizationServiceOptions> CreateHandler()
        {
            return new OioIdwsAuthorizationServiceHandler(_logger);
        }
    }
}   