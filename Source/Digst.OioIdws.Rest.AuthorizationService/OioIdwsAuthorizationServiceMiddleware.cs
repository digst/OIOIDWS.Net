using System;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Digst.OioIdws.Rest.AuthorizationService
{
    public class OioIdwsAuthorizationServiceMiddleware : OwinMiddleware
    {
        private readonly IAccessTokenGenerator _accessTokenGenerator;
        private readonly ITokenStore _tokenStore;
        private readonly Settings _settings;
        class Settings
        {
            public PathString AuthorizationPath { get; set; }
            public TimeSpan AccessTokenExpiration { get; set; }
        }

        public OioIdwsAuthorizationServiceMiddleware(
            OwinMiddleware next, 
            OioIdwsAuthorizationServiceOptions options, 
            IAccessTokenGenerator accessTokenGenerator,
            ITokenStore tokenStore) 
            : base(next)
        {
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

            _accessTokenGenerator = accessTokenGenerator;
            _tokenStore = tokenStore;

            _settings = ValidateOptions(options);
        }

        private static Settings ValidateOptions(OioIdwsAuthorizationServiceOptions options)
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

            if (options.AccessTokenExpiration.TotalSeconds > 3600)
            {
                //todo: log warning that expiration *should* be lower
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