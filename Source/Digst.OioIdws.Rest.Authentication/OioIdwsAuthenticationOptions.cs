using System;
using Microsoft.Owin.Security;

namespace Digst.OioIdws.Rest.Authentication
{
    public class OioIdwsAuthenticationOptions : AuthenticationOptions
    {
        public OioIdwsAuthenticationOptions() : base("OioIdwsAuthentication")
        {
            TokenProvider = new TokenProvider();
        }

        /// <summary>
        /// Path on the AuthorizationService server where token information can be resolved by giving an acess token
        /// </summary>
        public Uri AccessTokenRetrievalEndpoint { get; set; }
        /// <summary>
        /// Used when building an identity during authentication. Defaults to <see cref="Digst.OioIdws.Rest.Authentication.IdentityBuilder"/>.
        /// </summary>
        public IIdentityBuilder IdentityBuilder { get; set; }

        /// <summary>
        /// Never intended to be replaced. It's only here to allow for internal testing
        /// </summary>
        internal ITokenProvider TokenProvider { get; set; }
    }
}
