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
            if (string.IsNullOrEmpty(context.Request.ContentType))
            {
                context.SetFailed(AuthenticationErrorCodes.InvalidRequest, "No content type was specified");
                return;
            }
            var ct = new System.Net.Mime.ContentType(context.Request.ContentType);

            string validContentType = "application/x-www-form-urlencoded";

            if (!ct.MediaType.Equals(validContentType, StringComparison.InvariantCultureIgnoreCase))
            {
                context.SetFailed(AuthenticationErrorCodes.InvalidRequest, $"Content type '{validContentType}' is required.");
                return;
            }

            var form = await context.Request.ReadFormAsync();
            var tokenValue = form["saml-token"];
            
            if (string.IsNullOrEmpty(tokenValue))
            {
                context.SetFailed(AuthenticationErrorCodes.InvalidRequest, "saml-token was missing");
                return;
            }
            
            var samlTokenValidation = await _tokenValidator.ValidateTokenAsync(tokenValue, context.ClientCertificate, context.Options);

            if (!samlTokenValidation.Success)
            {
                _logger.WriteInformation($"Issuing token was denied: {samlTokenValidation.ErrorDescription}");
                context.SetFailed(AuthenticationErrorCodes.InvalidToken, samlTokenValidation.ErrorDescription);
                return;
            }

            var storedToken = new OioIdwsToken
            {
                CertificateThumbprint = samlTokenValidation.AccessTokenType == AccessTokenType.HolderOfKey 
                    ? context.ClientCertificate?.Thumbprint?.ToLowerInvariant()
                    : null,
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
            await WriteAccessTokenAsync(context.Response, accessToken, samlTokenValidation.AccessTokenType, context.Options.AccessTokenExpiration);
            _logger.WriteInformation($"Token {accessToken} was issued");

            context.RequestCompleted();
        }

        private async Task WriteAccessTokenAsync(IOwinResponse response, string accessToken, AccessTokenType accessTokenType, TimeSpan accessTokenExpiration)
        {
            response.ContentType = "application/json; charset=UTF-8";

            //todo: type either bearer/holder-of-key
            var tokenObj = new JObject(
                new JProperty("access_token", accessToken),
                new JProperty("token_type", accessTokenType == AccessTokenType.Bearer ? "bearer" : "holder-of-key"),
                new JProperty("expires_in", (int) accessTokenExpiration.TotalSeconds));

            await response.WriteAsync(tokenObj.ToString());
        }
    }
}
