using Digst.OioIdws.Rest.AuthorizationService.Issuing;
using Digst.OioIdws.Rest.AuthorizationService.Storage;
using Owin;

namespace Digst.OioIdws.Rest.AuthorizationService
{
    public static class OioIdwsAuthorizationServiceAppBuilderExtensions
    {
        public static IAppBuilder UseOioIdwsAuthorizationService(this IAppBuilder app, OioIdwsAuthorizationServiceOptions options)
        {
            return app.UseOioIdwsAuthorizationService(options, new MemorySecurityTokenStore());
        }

        public static IAppBuilder UseOioIdwsAuthorizationService(this IAppBuilder app, OioIdwsAuthorizationServiceOptions options, ISecurityTokenStore securityTokenStore)
        {
            return app.Use<OioIdwsAuthorizationServiceMiddleware>(app, options, securityTokenStore);
        }
    }
}
