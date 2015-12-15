using Owin;

namespace Digst.OioIdws.Rest.AuthorizationService
{
    public static class OioIdwsAuthorizationServiceAppBuilderExtensions
    {
        public static IAppBuilder UseOioIdwsAuthorizationService(this IAppBuilder app, OioIdwsAuthorizationServiceOptions options)
        {
            return app.UseOioIdwsAuthorizationService(options, new MemoryTokenStore());
        }

        public static IAppBuilder UseOioIdwsAuthorizationService(this IAppBuilder app, OioIdwsAuthorizationServiceOptions options, ITokenStore tokenStore)
        {
            return app.Use<OioIdwsAuthorizationServiceMiddleware>(app, options, new AccessTokenGenerator(), tokenStore);
        }
    }
}
