using Owin;

namespace Digst.OioIdws.Rest.AuthorizationService
{
    public static class OioIdwsAuthorizationServiceAppBuilderExtensions
    {
        /// <summary>
        /// Configures the OWIN pipeline to use OIO IDWS AuthorizationService
        /// </summary>
        /// <param name="app"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IAppBuilder UseOioIdwsAuthorizationService(this IAppBuilder app, OioIdwsAuthorizationServiceOptions options)
        {
            return app.Use<OioIdwsAuthorizationServiceMiddleware>(app, options);
        }
    }
}
