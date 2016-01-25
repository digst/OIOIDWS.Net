using System;
using System.Text;
using Digst.OioIdws.Rest.Common;
using Microsoft.Owin;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer
{
    internal static class OwinContextExtensions
    {
        public static void SetAuthenticationFailed(
            this IOwinResponse response,
            AccessTokenType type,
            string error, 
            string errorDescription = null, 
            string requiredScope = null)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            if (error == AuthenticationErrorCodes.InvalidRequest)
            {
                response.StatusCode = 400;
            }

            if (error == AuthenticationErrorCodes.InvalidToken)
            {
                response.StatusCode = 401;
            }

            if (error == AuthenticationErrorCodes.InsufficentScope)
            {
                response.StatusCode = 403;
            }

            response.OnSendingHeaders(rsp =>
            {
                var sb = new StringBuilder($@"{AccessTokenTypeParser.ToString(type)} error=""{error}""");

                if (!string.IsNullOrEmpty(errorDescription))
                {
                    sb.Append($@",error_description=""{errorDescription}""");
                }

                if (!string.IsNullOrEmpty(requiredScope))
                {
                    sb.Append($@",scope=""{requiredScope}""");
                }

                ((IOwinResponse) rsp).Headers.Set(
                    "WWW-Authenticate", 
                    sb.ToString()
                );
            }, response);
        }
    }
}