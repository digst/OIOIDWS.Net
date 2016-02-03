using System;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;
using Digst.OioIdws.Rest.Server.AuthorizationServer;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace Digst.OioIdws.Rest.Server.Wsp
{
    public class OioIdwsAuthenticationHandler : AuthenticationHandler<OioIdwsAuthenticationOptions>
    {
        private readonly ILogger _logger;
        private string _errorCode;
        private string _errorDescription;
        private AccessTokenType _accessTokenType; 

        public OioIdwsAuthenticationHandler(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;
        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            try
            {
                AuthenticationHeaderValue authHeader;
                if (AuthenticationHeaderValue.TryParse(Context.Request.Headers["Authorization"], out authHeader))
                {
                    var requestAccessTokenType = AccessTokenTypeParser.FromString(authHeader.Scheme);

                    if (!requestAccessTokenType.HasValue)
                    {
                        _logger.WriteVerbose($"Ignoring unhandled authorization scheme '{authHeader.Scheme}'");
                        return null;
                    }

                    var accessToken = authHeader.Parameter;

                    _logger.WriteEntry(Log.ProcessingToken(accessToken));
                    
                    //The token provider validates that the token is known and not expired.
                    var tokenRetrievalResult = await Options.TokenProvider.RetrieveTokenAsync(accessToken);
                    
                    if (tokenRetrievalResult.Success)
                    {
                        var token = tokenRetrievalResult.Result;

                        if (requestAccessTokenType != token.Type)
                        {
                            StoreAuthenticationFailed(AuthenticationErrorCodes.InvalidToken, "Authentication scheme was not valid", token.Type);
                            _logger.WriteEntry(Log.InvalidTokenType(authHeader.Scheme));
                            return null;
                        }

                        if (token.Type == AccessTokenType.HolderOfKey)
                        {
                            var cert = Context.Get<X509Certificate2>("ssl.ClientCertificate");

                            if (cert?.Thumbprint == null || !cert.Thumbprint.Equals(token.CertificateThumbprint, StringComparison.OrdinalIgnoreCase))
                            {
                                StoreAuthenticationFailed(AuthenticationErrorCodes.InvalidToken, "A valid certificate must be presented when presenting a holder-of-key token", requestAccessTokenType.Value);
                                _logger.WriteEntry(Log.HolderOfKeyNoCertificatePresented(accessToken, cert?.Thumbprint));
                                return null;
                            }
                        }

                        var identity = await Options.IdentityBuilder.BuildIdentityAsync(token);
                        _logger.WriteEntry(Log.TokenValidatedAndRequestAuthenticated(accessToken));
                        return new AuthenticationTicket(identity, new AuthenticationProperties());
                    }

                    if (tokenRetrievalResult.Expired)
                    {
                        _logger.WriteEntry(Log.TokenExpired(accessToken));
                        StoreAuthenticationFailed(AuthenticationErrorCodes.InvalidToken, "Token was expired", AccessTokenTypeParser.FromString(authHeader.Scheme) ?? AccessTokenType.Bearer);
                    }
                    else
                    {
                        _logger.WriteEntry(Log.TokenWasNotRetrievedFromAuthorizationServer(accessToken));
                        StoreAuthenticationFailed(AuthenticationErrorCodes.InvalidToken, "Token information could not be retrieved from the Authorization Server. The access token might be unknown or expired", requestAccessTokenType.Value);
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError("Unhandled exception", ex);
            }

            return null;
        }

        /// <summary>
        /// Stores authentication information for this request inside this handler instance. Handlers are created per request,
        /// and if a challenge is found necessary, these values are used for generated proper challenge
        /// </summary>
        /// <param name="error"></param>
        /// <param name="errorDescription"></param>
        /// <param name="type"></param>
        private void StoreAuthenticationFailed(string error, string errorDescription, AccessTokenType type)
        {
            _errorCode = error;
            _errorDescription = errorDescription;
            _accessTokenType = type;
        }

        protected override Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode == 401 && !string.IsNullOrEmpty(_errorCode))
            {
                Context.Response.SetAuthenticationFailed(_accessTokenType, _errorCode, _errorDescription);
            }

            var challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);

            string scope;

            if (challenge != null && challenge.Properties.Dictionary.TryGetValue(AuthenticationErrorCodes.InsufficentScope, out scope))
            {
                Context.Response.SetAuthenticationFailed(_accessTokenType, AuthenticationErrorCodes.InsufficentScope, requiredScope: scope);
            }

            return Task.FromResult(0);
        }
    }
}