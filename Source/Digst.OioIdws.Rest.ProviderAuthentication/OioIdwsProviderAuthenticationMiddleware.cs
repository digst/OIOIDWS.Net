using System;
using System.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Digst.OioIdws.Rest.ProviderAuthentication
{
    public class OioIdwsProviderAuthenticationMiddleware : OwinMiddleware
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IPrincipalBuilder _principalBuilder;

        public OioIdwsProviderAuthenticationMiddleware(OwinMiddleware next, OioIdwsProviderAuthenticationOptions options, ITokenProvider tokenProvider, IPrincipalBuilder principalBuilder) : base(next)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (tokenProvider == null)
            {
                throw new ArgumentNullException(nameof(tokenProvider));
            }
            if (principalBuilder == null)
            {
                throw new ArgumentNullException(nameof(principalBuilder));
            }

            _tokenProvider = tokenProvider;
            _principalBuilder = principalBuilder;
        }

        public override async Task Invoke(IOwinContext context)
        {
            AuthenticationHeaderValue authHeader;
            if(AuthenticationHeaderValue.TryParse(context.Request.Headers["Authorization"], out authHeader) && authHeader.Scheme == "Bearer")
            {
                var token = await _tokenProvider.RetrieveTokenAsync(authHeader.Parameter);
                //todo: validate token
                context.Request.User = await _principalBuilder.BuildPrincipalAsync(token);
            }

            await Next.Invoke(context);
        }
    }
}
