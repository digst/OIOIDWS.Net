using Microsoft.Owin.Extensions;
using Owin;

namespace Digst.OioIdws.Rest.Server
{
    public static class OioIdwsAuthenticationAppBuilderExtensions
    {
        /// <summary>
        /// Configures the OWIN pipeline to use OIO IDWS service authentication. Defaults to run at the <see cref="PipelineStage.Authenticate"/> stage
        /// </summary>
        /// <param name="app"></param>
        /// <param name="options"></param>
        /// <param name="stage"></param>
        /// <returns></returns>
        public static IAppBuilder UseOioIdwsAuthentication(this IAppBuilder app, OioIdwsAuthenticationOptions options, PipelineStage stage = PipelineStage.Authenticate)
        {
            return app
                .Use<OioIdwsAuthenticationMiddleware>(app, options)
                .UseStageMarker(stage);
        }
    }
}
