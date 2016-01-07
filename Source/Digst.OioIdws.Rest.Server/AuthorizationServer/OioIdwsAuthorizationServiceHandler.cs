﻿using System;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Server.AuthorizationServer.Issuing;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenRetrieval;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer
{
    public class OioIdwsAuthorizationServiceHandler : AuthenticationHandler<OioIdwsAuthorizationServiceOptions>
    {
        private readonly ILogger _logger;

        private AccessTokenIssuer _accessTokenIssuer;
        private AccessTokenRetriever _accessTokenRetriever;

        public OioIdwsAuthorizationServiceHandler(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;
        }

        protected override Task InitializeCoreAsync()
        {
            _accessTokenIssuer = new AccessTokenIssuer(Options.AccessTokenGenerator, Options.SecurityTokenStore, Options.TokenValidator, _logger);
            _accessTokenRetriever = new AccessTokenRetriever(Options.SecurityTokenStore, _logger);
            return Task.FromResult(0);
        }

        public override async Task<bool> InvokeAsync()
        {
            //todo: send challenge in the correct security callback..

            var cert = Context.Get<X509Certificate2>("ssl.ClientCertificate");

            if (cert != null)
            {
                try
                {
                    Options.CertificateValidator.Validate(cert);
                }
                catch (SecurityTokenValidationException ex)
                {
                    _logger.WriteError($"Validating client certificate with thumbprint '{cert.Thumbprint} failed", ex);
                    return false;
                }
            }

            if (!string.Equals(Request.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            {
                _logger.WriteWarning("Authorization Server ignoring request because it's not https");
                return false;
            }

            var matchRequestContext = new OioIdwsMatchEndpointContext(Context, Options)
            {
                ClientCertificate = cert
            };

            if (Context.Request.Path.Equals(Options.AccessTokenIssuerPath) && Context.Request.Method == "POST")
            {
                matchRequestContext.IsAccessTokenIssueEndpoint = true;
            }

            if (Context.Request.Path.Equals(Options.AccessTokenRetrievalPath) && Context.Request.Method == "GET")
            {
                matchRequestContext.IsAccessTokenRetrievalEndpoint = true;
            }

            try
            {
                if (matchRequestContext.IsAccessTokenIssueEndpoint)
                {
                    _logger.WriteVerbose("Invoking access token issuer");
                    await _accessTokenIssuer.IssueAsync(matchRequestContext);
                }

                if (matchRequestContext.IsAccessTokenRetrievalEndpoint)
                {
                    //todo trust/validate WSP?
                    _logger.WriteVerbose("Invoking access token retrieval");
                    await _accessTokenRetriever.RetrieveAsync(matchRequestContext);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError("Unhandled exception", ex);
                throw;
            }

            return matchRequestContext.IsRequestCompleted;
        }

        protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            return Task.FromResult<AuthenticationTicket>(null);
        }
    }
}