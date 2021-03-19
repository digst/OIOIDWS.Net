using System;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Server.AuthorizationServer;
using Digst.OioIdws.Rest.Server.Wsp;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Owin;

namespace Digst.OioIdws.Rest.Examples.ServerAndASCombinedNuget
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.SetLoggerFactory(new ConsoleLoggerFactory());

            app.Use(async (ctx, next) =>
            {
                //For correlating logs
                CallContext.LogicalSetData("correlationIdentifier", ctx.Environment["owin.RequestId"]);
                await next();
                CallContext.LogicalSetData("correlationIdentifier", null);
            })
            .UseOioIdwsAuthentication(new OioIdwsAuthenticationOptions())
            .UseOioIdwsAuthorizationService(new OioIdwsAuthorizationServiceOptions
            {
                AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                IssuerAudiences = () => Task.FromResult(new[]
                {
                    new IssuerAudiences("78b20a6d9000f29011063ba7b60ff66074076f48", "test cert")
                        .Audience(new Uri("https://wsp.oioidws-net.dk")),
                }),
            })
            .Use<MyService>();
        }
    }
}
