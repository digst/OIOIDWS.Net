using System;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.AuthorizationService.Issuing;
using Digst.OioIdws.Rest.AuthorizationService.Retrieval;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace Digst.OioIdws.Rest.AuthorizationService
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

            //var cert = context.Get<X509Certificate2>("ssl.ClientCertificate");
            //X509CertificateValidator.ChainTrust.Validate(cert);

            try
            {
                //todo require SSL?
                if (Context.Request.Path.Equals(Options.AccessTokenIssuerPath) && Context.Request.Method == "POST")
                {
                    _logger.WriteVerbose("Invoking access token issuer");
                    await _accessTokenIssuer.IssueAsync(Context, Options);
                    return true;
                }

                if (Context.Request.Path.Equals(Options.AccessTokenRetrievalPath) && Context.Request.Method == "GET")
                {
                    //todo trust/validate WSP?
                    _logger.WriteVerbose("Invoking access token retrieval");
                    await _accessTokenRetriever.RetrieveAsync(Context);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError("Unhandled exception", ex);
                //todo: decide, return error response?
            }

            return false;
        }

        protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            return Task.FromResult<AuthenticationTicket>(null);
        }
    }
}