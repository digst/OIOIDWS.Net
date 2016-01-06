using System;
using Microsoft.Owin;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer
{
    internal static class OwinContextExtensions
    {
        public static void SetAuthenticationFailed(this IOwinContext context, string error, string errorDescription)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }
            if (errorDescription == null)
            {
                throw new ArgumentNullException(nameof(errorDescription));
            }

            context.Response.StatusCode = 401;
            context.Response.OnSendingHeaders(ctx =>
            {
                ((IOwinContext) ctx).Response.Headers.Add("WWW-Authenticate", new[]
                {
                    $@"Bearer error=""{error}"",error_description=""{errorDescription}""",
                });
            }, context);

        }
    }
}