using System;
using Digst.OioIdws.Rest.ProviderAuthentication;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Logging;
using Owin;

namespace Digst.OioIdws.Rest.Examples.WSP
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.SetLoggerFactory(new ConsoleLoggerFactory());

            app
                .UseErrorPage()
                .OioIdwsProviderAuthentication(new OioIdwsProviderAuthenticationOptions
                {
                    AccessTokenRetrievalEndpoint = new Uri("https://digst.oioidws.rest.as:10001/accesstoken")
                })
                .Use<MyService>();
        }
    }
}
