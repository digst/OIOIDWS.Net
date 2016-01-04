using Microsoft.Owin.Extensions;
using Owin;

namespace Digst.OioIdws.Rest.Authentication
{
    public static class OioIdwsAuthenticationAppBuilderExtensions
    {
        public static IAppBuilder OioIdwsAuthentication(this IAppBuilder app, OioIdwsAuthenticationOptions options)
        {
            return app.OioIdwsAuthentication(options, PipelineStage.Authenticate);
        }
        public static IAppBuilder OioIdwsAuthentication(this IAppBuilder app, OioIdwsAuthenticationOptions options, PipelineStage stage)
        {
            return app.Use<OioIdwsAuthenticationMiddleware>(app, options)
                .UseStageMarker(stage);
        }
    }
}
