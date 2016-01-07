using System;
using System.Linq;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Newtonsoft.Json.Linq;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer.Issuing
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

        public async Task IssueAsync(OioIdwsMatchEndpointContext context)
        {
            //todo: should we check Request content-type?
            var form = await context.Request.ReadFormAsync();
            var tokenValue = form["saml-token"];
            
            if (string.IsNullOrEmpty(tokenValue))
            {
                context.Response.SetAuthenticationFailed(AuthenticationErrorCodes.InvalidRequest, "saml-token was missing");
                context.RequestCompleted();
                return;
            }
            
            var samlTokenValidation = await _tokenValidator.ValidateTokenAsync(tokenValue, null, context.Options); //todo get cert

            if (!samlTokenValidation.Success)
            {
                _logger.WriteInformation($"Issuing token was denied - {samlTokenValidation.ErrorCode}: {samlTokenValidation.ErrorDescription}");
                context.Response.SetAuthenticationFailed(samlTokenValidation.ErrorCode, samlTokenValidation.ErrorDescription);
                return;
            }

            var storedToken = new OioIdwsToken
            {
                CertificateThumbprint = "", //todo get cert
                Type = samlTokenValidation.AccessTokenType,
                ValidUntilUtc = DateTime.UtcNow + context.Options.AccessTokenExpiration, //todo add time skew?
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
            await WriteAccessTokenAsync(context.Response, accessToken, context.Options.AccessTokenExpiration);
            _logger.WriteInformation($"Token {accessToken} was issued");

            context.RequestCompleted();
        }

        private async Task WriteAccessTokenAsync(IOwinResponse response, string accessToken, TimeSpan accessTokenExpiration)
        {
            response.ContentType = "application/json; charset=UTF-8";

            //todo: type either bearer/holder-of-key
            var tokenObj = new JObject(
                new JProperty("access_token", accessToken),
                new JProperty("token_type", "bearer"),
                new JProperty("expires_in", (int) accessTokenExpiration.TotalSeconds));

            await response.WriteAsync(tokenObj.ToString());
        }
    }
}
