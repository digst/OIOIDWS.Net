using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;

namespace Digst.OioIdws.Rest.ProviderAuthentication
{
    public static class OioIdwsProviderAuthenticationAppBuilderExtensions
    {
        public static IAppBuilder OioIdwsProviderAuthentication(this IAppBuilder app, OioIdwsProviderAuthenticationOptions options)
        {
            return app.Use<OioIdwsProviderAuthenticationMiddleware>(options);
        }
    }
}
