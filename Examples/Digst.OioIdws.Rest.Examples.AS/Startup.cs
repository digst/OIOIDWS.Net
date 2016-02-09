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
                    new IssuerAudiences("2E7A061560FA2C5E141A634DC1767DACAEEC8D12", "test cert")
                        .Audience(new Uri("https://wsp.itcrew.dk")), 
                }),
                TrustedWspCertificateThumbprints = new[] { "dc35c0466ad606422adff717c9cb8e3274d8772e" },
            })
            .Use((ctx, next) =>
            {
                ctx.Response.Write("Well hello there. Guess you didn't hit any of the AS endpoints?");
                return Task.FromResult(0);
            });
        }
    }
}
