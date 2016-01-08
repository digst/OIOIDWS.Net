using System;
using Microsoft.Owin;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer
{
    internal static class OwinContextExtensions
    {
        public static void SetAuthenticationFailed(this IOwinResponse response, string error, string errorDescription)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }
            if (errorDescription == null)
            {
                throw new ArgumentNullException(nameof(errorDescription));
            }

            response.StatusCode = 401;
            response.OnSendingHeaders(rsp =>
            {
                ((IOwinResponse) rsp).Headers.Set(
                    "WWW-Authenticate", 
                    $@"Bearer error=""{error}"",error_description=""{errorDescription}"""
                );
            }, response);

        }
    }
}