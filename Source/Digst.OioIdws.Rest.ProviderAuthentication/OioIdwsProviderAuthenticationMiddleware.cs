using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Owin;

namespace Digst.OioIdws.Rest.ProviderAuthentication
{
    public class OioIdwsProviderAuthenticationMiddleware : OwinMiddleware
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IPrincipalBuilder _principalBuilder;
        private readonly Settings _settings;
        private readonly ILogger _logger;

        internal class Settings
        {
            public Uri AccessTokenRetrievalEndpoint { get; set; }
        }

        public OioIdwsProviderAuthenticationMiddleware(
            OwinMiddleware next, 
            IAppBuilder app,
            OioIdwsProviderAuthenticationOptions options, 
            IPrincipalBuilder principalBuilder) : base(next)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (principalBuilder == null)
            {
                throw new ArgumentNullException(nameof(principalBuilder));
            }

            _tokenProvider = options.TokenProvider;
            _principalBuilder = principalBuilder;

            _logger = app.CreateLogger<OioIdwsProviderAuthenticationMiddleware>();

            _settings = ValidateOptions(options);


        }

        private Settings ValidateOptions(OioIdwsProviderAuthenticationOptions options)
        {
            if (options.AccessTokenRetrievalEndpoint == null)
            {
                throw new ArgumentNullException(nameof(options.AccessTokenRetrievalEndpoint));
            }
            
            return new Settings
            {
                AccessTokenRetrievalEndpoint = options.AccessTokenRetrievalEndpoint,
            };
        }

        public override async Task Invoke(IOwinContext context)
        {
            try
            {
                //todo: check scheme + cert
                AuthenticationHeaderValue authHeader;
                if (AuthenticationHeaderValue.TryParse(context.Request.Headers["Authorization"], out authHeader)) //&& authHeader.Scheme == "Bearer")
                {
                    var token = await _tokenProvider.RetrieveTokenAsync(authHeader.Parameter, _settings);
                    //todo: validate token
                    context.Request.User = await _principalBuilder.BuildPrincipalAsync(token);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError("Unhandled exception", ex);
            }

            await Next.Invoke(context);
        }
    }
}