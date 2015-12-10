using System;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;
using Microsoft.Owin;

namespace Digst.OioIdws.Rest.AuthorizationService
{
    public class AccessTokenIssuer
    {
        public async Task IssueAsync(IOwinContext context, TimeSpan accessTokenExpiration, IAccessTokenGenerator accessTokenGenerator, ITokenStore tokenStore)
        {
            //todo: should we check Request content-type?
            var form = await context.Request.ReadFormAsync();
            var tokenValue = form["saml-token"];
            var samlToken = new SamlToken
            {
                TokenValue = tokenValue
            };

            //todo: validate/parse token

            if (string.IsNullOrEmpty(tokenValue))
            {
                context.SetAuthenticationFailed(AuthenticationErrorCodes.InvalidRequest,
                    AuthenticationErrorCodes.Descriptions.SamlTokenMissing);
                return;
            }

            var accessToken = accessTokenGenerator.GenerateAccesstoken();
            await tokenStore.StoreTokenAsync(accessToken, samlToken);
            await WriteAccessTokenAsync(context, accessToken, accessTokenExpiration);
        }

        private async Task WriteAccessTokenAsync(IOwinContext context, string accessToken, TimeSpan accessTokenExpiration)
        {
            var tokenJson =
                $@"
                {{
                    ""access_token"": ""{accessToken}"",
                    ""token_type"": ""Bearer"",
                    ""expires_in"": ""{(int)accessTokenExpiration.TotalSeconds}""
                }}";

            context.Response.ContentType = "application/json; charset=UTF-8";
            await context.Response.WriteAsync(tokenJson);
        }
    }
}
