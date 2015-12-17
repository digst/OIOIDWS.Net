using System;
using System.IdentityModel.Selectors;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.AuthorizationService.Issuing;
using Digst.OioIdws.Rest.AuthorizationService.Retrieval;
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
        private readonly AccessTokenRetriever _accessTokenRetriever;

        internal class Settings
        {
            public PathString AccessTokenIssuerPath { get; set; }
            public TimeSpan AccessTokenExpiration { get; set; }
            public SecurityTokenResolver ServiceTokenResolver { get; set; }
            public PathString AccessTokenRetrievalPath { get; set; }
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

            _accessTokenIssuer = new AccessTokenIssuer(options.AccessTokenGenerator, securityTokenStore, options.TokenValidator);
            _accessTokenRetriever = new AccessTokenRetriever(securityTokenStore);

            //todo: log that we're started
        }

        private Settings ValidateOptions(OioIdwsAuthorizationServiceOptions options)
        {
            if (options.ServiceTokenResolver == null)
            {
                throw new ArgumentNullException(nameof(options.ServiceTokenResolver));
            }

            if (!options.AccessTokenIssuerPath.HasValue)
            {
                throw new ArgumentException("AccessTokenIssuerPath must be set to a valid path", nameof(options.AccessTokenIssuerPath));
            }

            if (!options.AccessTokenRetrievalPath.HasValue)
            {
                throw new ArgumentException("AccessTokenRetrievalPath must be set to a valid path", nameof(options.AccessTokenRetrievalPath));
            }

            var settings = new Settings
            {
                AccessTokenIssuerPath = options.AccessTokenIssuerPath,
                ServiceTokenResolver = options.ServiceTokenResolver,
                AccessTokenRetrievalPath = options.AccessTokenRetrievalPath
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
            //todo require SSL?
            if (context.Request.Path.Equals(_settings.AccessTokenIssuerPath) && context.Request.Method == "POST")
            {
                await _accessTokenIssuer.IssueAsync(context, _settings);
            }
            else if (context.Request.Path.Equals(_settings.AccessTokenRetrievalPath) && context.Request.Method == "GET")
            {
                //todo trust/validate WSP?
                await _accessTokenRetriever.RetrieveAsync(context, _settings);
            }
            else
            {
                await Next.Invoke(context);
            }
        }
    }
}   