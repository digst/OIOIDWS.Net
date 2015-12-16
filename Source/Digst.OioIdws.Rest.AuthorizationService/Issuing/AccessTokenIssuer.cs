using System;
using System.Linq;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.AuthorizationService.Storage;
using Digst.OioIdws.Rest.Common;
using Microsoft.Owin;

namespace Digst.OioIdws.Rest.AuthorizationService.Issuing
{
    internal class AccessTokenIssuer
    {
        private readonly IAccessTokenGenerator _accessTokenGenerator;
        private readonly ISecurityTokenStore _securityTokenStore;
        private readonly ITokenValidator _tokenValidator;

        public AccessTokenIssuer(
            IAccessTokenGenerator accessTokenGenerator, 
            ISecurityTokenStore securityTokenStore,
            ITokenValidator tokenValidator)
        {
            if (accessTokenGenerator == null)
            {
                throw new ArgumentNullException(nameof(accessTokenGenerator));
            }
            if (securityTokenStore == null)
            {
                throw new ArgumentNullException(nameof(securityTokenStore));
            }
            if (tokenValidator == null)
            {
                throw new ArgumentNullException(nameof(tokenValidator));
            }
            _accessTokenGenerator = accessTokenGenerator;
            _securityTokenStore = securityTokenStore;
            _tokenValidator = tokenValidator;
        }

        public async Task IssueAsync(
            IOwinContext context, 
            OioIdwsAuthorizationServiceMiddleware.Settings settings)
        {
            //todo: should we check Request content-type?
            var form = await context.Request.ReadFormAsync();
            var tokenValue = form["saml-token"];

            if (string.IsNullOrEmpty(tokenValue))
            {
                context.SetAuthenticationFailed(AuthenticationErrorCodes.InvalidRequest,
                    AuthenticationErrorCodes.Descriptions.SamlTokenMissing);
                return;
            }
            
            var samlTokenValidation = _tokenValidator.ValidateToken(tokenValue, null, settings); //todo get cert

            if (!samlTokenValidation.Success)
            {
                context.SetAuthenticationFailed(samlTokenValidation.ErrorCode, samlTokenValidation.ErrorDescription);
                return;
            }

            var storedToken = new OioIdwsToken
            {
                CertificateThumbprint = "", //todo get cert
                Type = samlTokenValidation.AccessTokenType,
                ValidUntilUtc = DateTime.UtcNow + settings.AccessTokenExpiration, //todo add time skew?
                Claims = samlTokenValidation.ClaimsIdentity.Claims.Select(x => new OioIdwsClaim
                {
                    Type = x.Type,
                    Value = x.Value,
                    Issuer = x.Issuer,
                    ValueType = x.ValueType
                }).ToList(),
                RoleType = samlTokenValidation.ClaimsIdentity.RoleClaimType,
                NameType = samlTokenValidation.ClaimsIdentity.NameClaimType,
                AuthenticationType = samlTokenValidation.ClaimsIdentity.AuthenticationType
            };

            var accessToken = _accessTokenGenerator.GenerateAccesstoken();
            await _securityTokenStore.StoreTokenAsync(accessToken, storedToken);
            await WriteAccessTokenAsync(context, accessToken, settings.AccessTokenExpiration);
        }

        private async Task WriteAccessTokenAsync(IOwinContext context, string accessToken, TimeSpan accessTokenExpiration)
        {
            //todo: type either bearer/holder-of-key
            var tokenJson =
                $@"
                {{
                    ""access_token"": ""{accessToken}"",
                    ""token_type"": ""bearer"",
                    ""expires_in"": ""{(int)accessTokenExpiration.TotalSeconds}""
                }}";

            context.Response.ContentType = "application/json; charset=UTF-8";
            await context.Response.WriteAsync(tokenJson);
        }
    }
}
