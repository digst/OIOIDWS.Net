using System;
using System.IdentityModel.Selectors;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.AuthorizationService.Issuing;
using Digst.OioIdws.Rest.AuthorizationService.Storage;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Owin;

namespace Digst.OioIdws.Rest.AuthorizationService
{
    public class OioIdwsAuthorizationServiceMiddleware : OwinMiddleware
    {
        private readonly Settings _settings;
        private readonly ILogger _logger;
        private readonly AccessTokenIssuer _accessTokenIssuer;

        internal class Settings
        {
            public PathString AuthorizationPath { get; set; }
            public TimeSpan AccessTokenExpiration { get; set; }
            public SecurityTokenResolver ServiceTokenResolver { get; set; }
        }

        public OioIdwsAuthorizationServiceMiddleware(
            OwinMiddleware next, 
            IAppBuilder appBuilder,
            OioIdwsAuthorizationServiceOptions options, 
            ISecurityTokenStore securityTokenStore) 
            : base(next)
        {
            if (appBuilder == null)
            {
                throw new ArgumentNullException(nameof(appBuilder));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (securityTokenStore == null)
            {
                throw new ArgumentNullException(nameof(securityTokenStore));
            }

            _logger = appBuilder.CreateLogger<OioIdwsAuthorizationServiceMiddleware>();

            _settings = ValidateOptions(options);

            _accessTokenIssuer = new AccessTokenIssuer(options.AccessTokenGenerator , securityTokenStore, options.TokenValidator);

            //todo: log that we're started
        }

        private Settings ValidateOptions(OioIdwsAuthorizationServiceOptions options)
        {
            if (options.AccessTokenGenerator == null)
            {
                throw new ArgumentNullException(nameof(options.AccessTokenGenerator));
            }

            if (options.ServiceTokenResolver == null)
            {
                throw new ArgumentNullException(nameof(options.ServiceTokenResolver));
            }

            var settings = new Settings
            {
                AuthorizationPath = options.IssueAccessTokenEndpoint,
                ServiceTokenResolver = options.ServiceTokenResolver
            };

            if (options.AccessTokenExpiration > TimeSpan.FromHours(1))
            {
                _logger.WriteWarning($"AccessTokenExpiration has been set to an expiration of {options.AccessTokenExpiration}. It is advised to set it to one hour or less.");
            }

            settings.AccessTokenExpiration = options.AccessTokenExpiration;

            return settings;
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Path.Equals(_settings.AuthorizationPath) && context.Request.Method == "POST")
            {
                await _accessTokenIssuer.IssueAsync(context, _settings);
            }
            else
            {
                await Next.Invoke(context);
            }
        }
    }
}   