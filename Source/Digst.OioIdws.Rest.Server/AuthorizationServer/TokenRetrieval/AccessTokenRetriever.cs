using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Server.AuthorizationServer.Issuing;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer.TokenRetrieval
{
    internal class AccessTokenRetriever
    {
        private readonly ISecurityTokenStore _securityTokenStore;
        private readonly ILogger _logger;

        public AccessTokenRetriever(ISecurityTokenStore securityTokenStore, ILogger logger)
        {
            if (securityTokenStore == null)
            {
                throw new ArgumentNullException(nameof(securityTokenStore));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _securityTokenStore = securityTokenStore;
            _logger = logger;
        }

        public async Task RetrieveAsync(OioIdwsMatchEndpointContext context)
        {
            var clientCertificate = context.ClientCertificate();

            if (clientCertificate?.Thumbprint == null ||
                context.Options.TrustedWspCertificateThumbprints == null ||
                !context.Options.TrustedWspCertificateThumbprints
                .Any(x => clientCertificate.Thumbprint.Equals(x, StringComparison.OrdinalIgnoreCase)))
            {
                RequestFailed(context, 401, "No trusted client certificate was provided");
                return;
            }

            context.Response.ContentType = "application/json; charset=UTF-8";

            if (!context.Request.QueryString.HasValue)
            {
                RequestFailed(context, 400, "access token must be given as the query string");
                return;
            }

            var accessToken = context.Request.QueryString.Value;

            _logger.WriteEntry(Log.ProcessingToken(accessToken));
            
            AuthenticationProperties tokenProperties;

            try
            {
                tokenProperties = context.Options.TokenDataFormat.Unprotect(accessToken);
            }
            catch (Exception ex)
            {
                RequestFailed(context, 401, "Token could not be read", ex);
                return;
            }

            if (tokenProperties == null)
            {
                RequestFailed(context, 401, "Token could not be unprotected");
                return;
            }

            //check the Expires that was stored inside the protected access token, saving a round trip to the SecurityTokenStore if the token is expired
            if (tokenProperties.ExpiresUtc.GetValueOrDefault() + context.Options.MaxClockSkew < context.Options.SystemClock.UtcNow)
            {
                TokenExpired(context, accessToken);
                return;
            }

            var tokenValue = tokenProperties.Value();

            _logger.WriteEntry(Log.AttemptRetrieveInformationForAccessToken(tokenValue));
            var token = await _securityTokenStore.RetrieveTokenAsync(tokenValue);

            if (token == null)
            {
                _logger.WriteEntry(Log.CouldNotRetrieveTokenInformationFromStore());
                context.Response.StatusCode = 404;
                return;
            }
            
            //we might as well check expiry against that stored along with the token information
            if (token.ExpiresUtc + context.Options.MaxClockSkew < context.Options.SystemClock.UtcNow)
            {
                TokenExpired(context, accessToken);
                return;
            }

            var serializer = new JsonSerializer();
            using (var writer = new StreamWriter(context.Response.Body))
            {
                serializer.Serialize(writer, token);
            }

            _logger.WriteEntry(Log.ProcessingTokenCompleted(accessToken));

            context.RequestCompleted();
        }

        private void TokenExpired(OioIdwsMatchEndpointContext context, string accessToken)
        {
            var reason = "The token has expired";
            _logger.WriteEntry(Log.TokenExpired(accessToken));
            context.Response.StatusCode = 401;
            context.Response.Write(new JObject(
                    new JProperty("error", reason),
                    new JProperty("expired", 1)).ToString());
            context.RequestCompleted();
        }

        private void RequestFailed(OioIdwsMatchEndpointContext context, int statusCode, string error, Exception ex = null, string accessToken = null)
        {
            _logger.WriteEntry(Log.RetrieveAccessTokenRequestFailed(error, accessToken, ex));
            context.Response.StatusCode = statusCode;
            context.Response.Write(new JObject(
                    new JProperty("error", error)).ToString());
            context.RequestCompleted();
        }
    }
}
