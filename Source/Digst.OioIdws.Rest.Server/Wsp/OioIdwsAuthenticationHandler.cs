using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace Digst.OioIdws.Rest.Server.Wsp
{
    public class OioIdwsAuthenticationHandler : AuthenticationHandler<OioIdwsAuthenticationOptions>
    {
        private readonly ILogger _logger;
        private readonly ISecurityTokenStore _securityTokenStore;

        public OioIdwsAuthenticationHandler(ILogger logger, ISecurityTokenStore securityTokenStore)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;
            _securityTokenStore = securityTokenStore;
        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            try
            {
                //todo: check scheme + cert
                AuthenticationHeaderValue authHeader;
                if (AuthenticationHeaderValue.TryParse(Context.Request.Headers["Authorization"], out authHeader)) //&& authHeader.Scheme == "Bearer")
                {
                    OioIdwsToken token;

                    if (Options.TokenRetrievalMethod == TokenRetrievalMethod.InMemory)
                    {
                        token = await _securityTokenStore.RetrieveTokenAsync(authHeader.Parameter);
                    }
                    else
                    {
                        //todo: caching
                        token = await Options.TokenProvider.RetrieveTokenAsync(authHeader.Parameter, Options.AccessTokenRetrievalEndpoint);
                    }
                    
                    //todo: validate token
                    var identity = await Options.IdentityBuilder.BuildIdentityAsync(token);
                    return new AuthenticationTicket(identity, new AuthenticationProperties());
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError("Unhandled exception", ex);
            }

            return null;
        }
    }
}