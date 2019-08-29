﻿using System;
using System.Runtime.Remoting.Messaging;
using Digst.OioIdws.Rest.Server.Wsp;
using Microsoft.Owin.Logging;
using Owin;

namespace Digst.OioIdws.Rest.Examples.Server
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
            .UseOioIdwsAuthentication(new OioIdwsAuthenticationOptions
            {
                TokenProvider = new RestTokenProvider(new Uri("https://digst.oioidws.rest.as:10001/accesstoken"), CertificateUtil.GetCertificate("1F0830937C74B0567D6B05C07B6155059D9B10C7"))
                    
            })
            .Use<MyService>();
        }
    }
}
