using System;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Server.AuthorizationServer.Issuing;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Owin;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer
{
    /// <summary>
    /// The middlware that acts as OIO IDWS AuthorizationService
    /// </summary>
    public class OioIdwsAuthorizationServiceMiddleware : AuthenticationMiddleware<OioIdwsAuthorizationServiceOptions>
    {
        public const string TokenRetrievalKey = "OioIdwsAuthorizationServiceMiddleware.TokenRetrievalKey";

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

            if (options.IssuerAudiences == null)
            {
                throw new ArgumentException($"The '{nameof(options.IssuerAudiences)}' option must be set");
            }

            _logger = app.CreateLogger<OioIdwsAuthorizationServiceMiddleware>();

            if (options.AccessTokenExpiration > TimeSpan.FromHours(1))
            {
                _logger.WriteWarning($"AccessTokenExpiration has been set to an expiration of {options.AccessTokenExpiration}. It is adviced to set it to one hour or less.");
            }

            if (options.TokenDataFormat == null)
            {
                var dataProtecter = app.CreateDataProtector(
                typeof(OioIdwsAuthorizationServiceMiddleware).FullName,
                Options.AuthenticationType, "v1");

                options.TokenDataFormat = new PropertiesDataFormat(dataProtecter);
            }

            app.Properties[TokenRetrievalKey] = (Func<string, Task<OioIdwsToken>>) (async accessToken =>
            {
                var props = Options.TokenDataFormat.Unprotect(accessToken);

                if (props == null)
                {
                    throw new InvalidOperationException("Token could not be read");
                }

                if (props.ExpiresUtc.GetValueOrDefault() + Options.MaxClockSkew < Options.SystemClock.UtcNow)
                {
                    throw new AccessTokenExpiredException();
                }

                var value = props.Value();

                var tokenInformation = await Options.SecurityTokenStore.RetrieveTokenAsync(value);

                if (tokenInformation != null && tokenInformation.ExpiresUtc + options.MaxClockSkew < Options.SystemClock.UtcNow)
                {
                    throw new AccessTokenExpiredException();
                }

                return tokenInformation;
            });
        }

        protected override AuthenticationHandler<OioIdwsAuthorizationServiceOptions> CreateHandler()
        {
            return new OioIdwsAuthorizationServiceHandler(_logger);
        }
    }
}   