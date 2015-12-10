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

        public OioIdwsProviderAuthenticationMiddleware(OwinMiddleware next, OioIdwsProviderAuthenticationOptions options, ITokenProvider tokenProvider) : base(next)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (tokenProvider == null)
            {
                throw new ArgumentNullException(nameof(tokenProvider));
            }

            _tokenProvider = tokenProvider;
        }

        public override async Task Invoke(IOwinContext context)
        {
            AuthenticationHeaderValue authHeader;
            if(AuthenticationHeaderValue.TryParse(context.Request.Headers["Authorization"], out authHeader))
            {
                //todo: validate parameter?
                var token = await _tokenProvider.RetrieveTokenAsync(authHeader.Parameter);
            }

            await Next.Invoke(context);
        }
    }
}
