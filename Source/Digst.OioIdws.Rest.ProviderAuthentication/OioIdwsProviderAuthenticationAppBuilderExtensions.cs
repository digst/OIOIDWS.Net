using Owin;

namespace Digst.OioIdws.Rest.ProviderAuthentication
{
    public static class OioIdwsProviderAuthenticationAppBuilderExtensions
    {
        public static IAppBuilder OioIdwsProviderAuthentication(this IAppBuilder app, OioIdwsProviderAuthenticationOptions options, ITokenProvider tokenProvider)
        {
            return app.Use<OioIdwsProviderAuthenticationMiddleware>(options, tokenProvider, new PrincipalBuilder());
        }
    }
}
