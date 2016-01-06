using System;
using Digst.OioIdws.Rest.Server.AuthorizationServer;
using Microsoft.Owin.Security;

namespace Digst.OioIdws.Rest.Server.Wsp
{
    public class OioIdwsAuthenticationOptions : AuthenticationOptions
    {
        public OioIdwsAuthenticationOptions() : base("UseOioIdwsAuthentication")
        {
            TokenProvider = new TokenProvider();
            TokenRetrievalMethod = TokenRetrievalMethod.InMemory;
        }

        /// <summary>
        /// Determines what method is used for retrieving token information. 
        /// If <see cref="Wsp.TokenRetrievalMethod.WebService"/> is used, <see cref="AccessTokenRetrievalEndpoint"/> must be set to the endpoint where tokens can be retrieved.
        /// If <see cref="Wsp.TokenRetrievalMethod.InMemory"/> is used, <see cref="OioIdwsAuthorizationServiceMiddleware"/> must be configured in OWIN pipeline for direct access.
        /// Defaults to <see cref="Wsp.TokenRetrievalMethod.InMemory"/>
        /// </summary>
        public TokenRetrievalMethod TokenRetrievalMethod { get; set; }
        /// <summary>
        /// Path on the AuthorizationService server where token information can be resolved by giving an access token
        /// </summary>
        public Uri AccessTokenRetrievalEndpoint { get; set; }
        /// <summary>
        /// Used when building an identity during authentication. Defaults to <see cref="Wsp.IdentityBuilder"/>.
        /// </summary>
        public IIdentityBuilder IdentityBuilder { get; set; }

        /// <summary>
        /// Never intended to be replaced. It's only here to allow for internal testing
        /// </summary>
        internal ITokenProvider TokenProvider { get; set; }
    }
}
