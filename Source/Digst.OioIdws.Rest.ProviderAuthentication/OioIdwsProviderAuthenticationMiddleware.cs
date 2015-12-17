using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Digst.OioIdws.Rest.ProviderAuthentication
{
    public class OioIdwsProviderAuthenticationMiddleware : OwinMiddleware
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IPrincipalBuilder _principalBuilder;
        private readonly Settings _settings;

        internal class Settings
        {
            public Uri AccessTokenRetrievalEndpoint { get; set; }
        }

        public OioIdwsProviderAuthenticationMiddleware(OwinMiddleware next, OioIdwsProviderAuthenticationOptions options, IPrincipalBuilder principalBuilder) : base(next)
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

            _settings = ValidateOptions(options);
        }

        private Settings ValidateOptions(OioIdwsProviderAuthenticationOptions options)
        {
            return new Settings
            {
                AccessTokenRetrievalEndpoint = options.AccessTokenRetrievalEndpoint,
            };
        }

        public override async Task Invoke(IOwinContext context)
        {
            AuthenticationHeaderValue authHeader;
            if(AuthenticationHeaderValue.TryParse(context.Request.Headers["Authorization"], out authHeader) && authHeader.Scheme == "Bearer")
            {
                var token = await _tokenProvider.RetrieveTokenAsync(authHeader.Parameter, _settings);
                //todo: validate token
                context.Request.User = await _principalBuilder.BuildPrincipalAsync(token);
            }

            await Next.Invoke(context);
        }
    }
}