using Owin;

namespace Digst.OioIdws.Rest.ProviderAuthentication
{
    public static class OioIdwsProviderAuthenticationAppBuilderExtensions
    {
        public static IAppBuilder OioIdwsProviderAuthentication(this IAppBuilder app, OioIdwsProviderAuthenticationOptions options, IPrincipalBuilder principalBuilder)
        {
            return app.OioIdwsProviderAuthentication(options);
        }
        public static IAppBuilder OioIdwsProviderAuthentication(this IAppBuilder app, OioIdwsProviderAuthenticationOptions options)
        {
            return app.Use<OioIdwsProviderAuthenticationMiddleware>(options, new TokenProvider(), new PrincipalBuilder());
        }
    }
}
