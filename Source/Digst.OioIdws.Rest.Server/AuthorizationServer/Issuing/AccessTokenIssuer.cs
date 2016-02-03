using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Newtonsoft.Json.Linq;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer.Issuing
{
    internal class AccessTokenIssuer
    {
        private readonly IKeyGenerator _keyGenerator;
        private readonly ISecurityTokenStore _securityTokenStore;
        private readonly ITokenValidator _tokenValidator;
        private readonly ILogger _logger;

        public AccessTokenIssuer(
            IKeyGenerator keyGenerator, 
            ISecurityTokenStore securityTokenStore, 
            ITokenValidator tokenValidator, 
            ILogger logger)
        {
            if (keyGenerator == null)
            {
                throw new ArgumentNullException(nameof(keyGenerator));
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
            _keyGenerator = keyGenerator;
            _securityTokenStore = securityTokenStore;
            _tokenValidator = tokenValidator;
            _logger = logger;
        }

        public async Task IssueAsync(OioIdwsMatchEndpointContext context)
        {
            if (string.IsNullOrEmpty(context.Request.ContentType))
            {
                SetInvalidRequest(context, "No content type was specified");
                return;
            }

            var ct = new System.Net.Mime.ContentType(context.Request.ContentType);

            var validContentType = "application/x-www-form-urlencoded";

            if (!ct.MediaType.Equals(validContentType, StringComparison.InvariantCultureIgnoreCase))
            {
                SetInvalidRequest(context, $"Content type '{validContentType}' is required.");
                return;
            }

            var form = await context.Request.ReadFormAsync();
            var tokenValueBase64 = form["saml-token"];
            
            if (string.IsNullOrEmpty(tokenValueBase64))
            {
                SetInvalidRequest(context, "saml-token was missing");
                return;
            }

            string tokenValue;

            try
            {
                var bytes = Convert.FromBase64String(tokenValueBase64);
                using (var stream = new MemoryStream(bytes))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        tokenValue = await reader.ReadToEndAsync();
                    }
                }
            }
            catch (Exception)
            {
                SetInvalidRequest(context, "saml-token must be in base64");
                return;
            }

            var clientCertificate = context.ClientCertificate();
            
            _logger.WriteEntry(Log.StartingTokenValidation());
            var samlTokenValidation = await _tokenValidator.ValidateTokenAsync(tokenValue, clientCertificate, context.Options);

            if (!samlTokenValidation.Success)
            {
                _logger.WriteEntry(Log.IssuingTokenDenied(samlTokenValidation.ErrorDescription, samlTokenValidation.ValidationException));
                
                // Scheme is mandatory and Holder-Of-Key is currently the only supportede scheme at NemLog-in STS. Hence, we specify Holder-Of-Key.
                context.Response.SetAuthenticationFailed(AccessTokenType.HolderOfKey, AuthenticationErrorCodes.InvalidToken, samlTokenValidation.ErrorDescription);
                context.RequestCompleted();
                return;
            }

            _logger.WriteEntry(Log.TokenValidationCompleted());

            var expiresIn = context.Options.AccessTokenExpiration;

            int requestedExpiration;
            if (int.TryParse(form["should-expire-in"], out requestedExpiration))
            {
                var tmp = TimeSpan.FromSeconds(requestedExpiration);

                if (tmp < expiresIn)
                {
                    //if the client wants a lower expiration, that's ok. Never to increase it.
                    expiresIn = tmp;
                }
            }

            var storedToken = new OioIdwsToken
            {
                CertificateThumbprint = samlTokenValidation.AccessTokenType == AccessTokenType.HolderOfKey 
                    ? clientCertificate?.Thumbprint?.ToLowerInvariant()
                    : null,
                Type = samlTokenValidation.AccessTokenType,
                ExpiresUtc = context.Options.SystemClock.UtcNow + expiresIn, 
                Claims = samlTokenValidation.ClaimsIdentity.Claims.Select(x => new OioIdwsClaim
                {
                    Type = x.Type,
                    Value = x.Value,
                    Issuer = x.Issuer,
                    ValueType = x.ValueType
                }).ToList(),
            };

            var accessToken = await GenerateAccessTokenAsync(context, storedToken);

            await WriteAccessTokenAsync(context.Response, accessToken, samlTokenValidation.AccessTokenType, expiresIn);
            _logger.WriteEntry(Log.TokenIssuedWithExpiration(accessToken, expiresIn));

            context.RequestCompleted();
        }

        private async Task<string> GenerateAccessTokenAsync(OioIdwsMatchEndpointContext context, OioIdwsToken storedToken)
        {
            var uniqueKey = _keyGenerator.GenerateUniqueKey();
            _logger.WriteEntry(Log.NewUniqueKeyGenerated(uniqueKey));

            await _securityTokenStore.StoreTokenAsync(uniqueKey, storedToken);
            _logger.WriteVerbose("Token information was committed to the Token Store");

            //store the Expiry time directly in the protected access token, allowing the Authorization Server to quickly validate the token when asked to retrieve information
            var properties = new AuthenticationProperties
            {
                ExpiresUtc = storedToken.ExpiresUtc
            };
            properties.Value(uniqueKey);

            var accessToken = context.Options.TokenDataFormat.Protect(properties);
            return accessToken;
        }

        private void SetInvalidRequest(OioIdwsMatchEndpointContext context, string message)
        {
            _logger.WriteWarning($"Returned invalid request: {message}");
            context.RequestCompleted();
        }

        private async Task WriteAccessTokenAsync(IOwinResponse response, string accessToken, AccessTokenType accessTokenType, TimeSpan expiresIn)
        {
            response.ContentType = "application/json; charset=UTF-8";

            var tokenObj = new JObject(
                new JProperty("access_token", accessToken),
                new JProperty("token_type", AccessTokenTypeParser.ToString(accessTokenType)),
                new JProperty("expires_in", (int) expiresIn.TotalSeconds));

            await response.WriteAsync(tokenObj.ToString());
        }
    }
}
