using System;
using System.Linq;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.AuthorizationService.Storage;
using Digst.OioIdws.Rest.Common;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Newtonsoft.Json.Linq;

namespace Digst.OioIdws.Rest.AuthorizationService.Issuing
{
    internal class AccessTokenIssuer
    {
        private readonly IAccessTokenGenerator _accessTokenGenerator;
        private readonly ISecurityTokenStore _securityTokenStore;
        private readonly ITokenValidator _tokenValidator;
        private readonly ILogger _logger;

        public AccessTokenIssuer(
            IAccessTokenGenerator accessTokenGenerator, 
            ISecurityTokenStore securityTokenStore, 
            ITokenValidator tokenValidator, 
            ILogger logger)
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
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            _accessTokenGenerator = accessTokenGenerator;
            _securityTokenStore = securityTokenStore;
            _tokenValidator = tokenValidator;
            _logger = logger;
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
                context.SetAuthenticationFailed(AuthenticationErrorCodes.InvalidRequest, "saml-token was missing");
                return;
            }
            
            var samlTokenValidation = await _tokenValidator.ValidateTokenAsync(tokenValue, null, settings); //todo get cert

            if (!samlTokenValidation.Success)
            {
                _logger.WriteInformation($"Issuing token was denied - {samlTokenValidation.ErrorCode}: {samlTokenValidation.ErrorDescription}");
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
            };

            var accessToken = _accessTokenGenerator.GenerateAccesstoken();
            await _securityTokenStore.StoreTokenAsync(accessToken, storedToken);
            await WriteAccessTokenAsync(context, accessToken, settings.AccessTokenExpiration);
            _logger.WriteInformation($"Token {accessToken} was issued");
        }

        private async Task WriteAccessTokenAsync(IOwinContext context, string accessToken, TimeSpan accessTokenExpiration)
        {
            context.Response.ContentType = "application/json; charset=UTF-8";

            //todo: type either bearer/holder-of-key
            var tokenObj = new JObject(
                new JProperty("access_token", accessToken),
                new JProperty("token_type", "bearer"),
                new JProperty("expires_in", (int) accessTokenExpiration.TotalSeconds));

            await context.Response.WriteAsync(tokenObj.ToString());
        }
    }
}
