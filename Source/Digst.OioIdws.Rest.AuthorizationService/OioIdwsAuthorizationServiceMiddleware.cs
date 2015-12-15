using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Owin;

namespace Digst.OioIdws.Rest.AuthorizationService
{
    public class OioIdwsAuthorizationServiceMiddleware : OwinMiddleware
    {
        private readonly IAccessTokenGenerator _accessTokenGenerator;
        private readonly ITokenStore _tokenStore;
        private readonly Settings _settings;
        private readonly ILogger _logger;

        class Settings
        {
            public PathString AuthorizationPath { get; set; }
            public TimeSpan AccessTokenExpiration { get; set; }
        }

        public OioIdwsAuthorizationServiceMiddleware(
            OwinMiddleware next, 
            IAppBuilder appBuilder,
            OioIdwsAuthorizationServiceOptions options, 
            IAccessTokenGenerator accessTokenGenerator,
            ITokenStore tokenStore) 
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
            if (accessTokenGenerator == null)
            {
                throw new ArgumentNullException(nameof(accessTokenGenerator));
            }
            if (tokenStore == null)
            {
                throw new ArgumentNullException(nameof(tokenStore));
            }

            _logger = appBuilder.CreateLogger<OioIdwsAuthorizationServiceMiddleware>();

            _accessTokenGenerator = accessTokenGenerator;
            _tokenStore = tokenStore;

            _settings = ValidateOptions(options);
        }

        private Settings ValidateOptions(OioIdwsAuthorizationServiceOptions options)
        {
            var settings = new Settings();

            try
            {
                settings.AuthorizationPath = new PathString(options.IssueAccessTokenEndpoint);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("'IssueAccessTokenEndpoint' must be set to a valid relative url where the endpoint will be hosted", ex);
            }

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
                await new AccessTokenIssuer().IssueAsync(context, _settings.AccessTokenExpiration, _accessTokenGenerator, _tokenStore);
            }
            else
            {
                await Next.Invoke(context);
            }
        }
    }
}   