using System;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Server.AuthorizationServer;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Owin;

namespace Digst.OioIdws.Rest.Examples.AS
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.SetLoggerFactory(new ConsoleLoggerFactory());

            app.Use(async (ctx, next) =>
            {
                //Example for correlating logs, track source address
                CallContext.LogicalSetData("correlationIdentifier", ctx.Environment["owin.RequestId"]);
                CallContext.LogicalSetData("sourceInfo", $"{ctx.Request.RemoteIpAddress}:{ctx.Request.RemotePort}");
                await next();
                CallContext.FreeNamedDataSlot("correlationIdentifier");
                CallContext.FreeNamedDataSlot("sourceInfo");
            })
            .UseErrorPage()
            .UseOioIdwsAuthorizationService(new OioIdwsAuthorizationServiceOptions
            {
                AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                AccessTokenRetrievalPath = new PathString("/accesstoken"),
                IssuerAudiences = () => Task.FromResult(new []
                {
                    new IssuerAudiences("fcb5edc9fb09cf39716c09c35fdc883bd48add8d", "test cert")
                        .Audience(new Uri("https://wsp.oioidws-net.dk")), 
                }),
                TrustedWspCertificateThumbprints = new[] { "1F0830937C74B0567D6B05C07B6155059D9B10C7" },
            })
            .Use((ctx, next) =>
            {
                ctx.Response.Write("Well hello there. Guess you didn't hit any of the AS endpoints?");
                return Task.FromResult(0);
            });
        }
    }
}
