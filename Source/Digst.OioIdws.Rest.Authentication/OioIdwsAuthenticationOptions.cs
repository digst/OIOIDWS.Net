using System;
using Microsoft.Owin.Security;

namespace Digst.OioIdws.Rest.Authentication
{
    public class OioIdwsAuthenticationOptions : AuthenticationOptions
    {
        public OioIdwsAuthenticationOptions() : base("OioIdws")
        {
            TokenProvider = new TokenProvider();
        }

        public Uri AccessTokenRetrievalEndpoint { get; set; }
        public IIdentityBuilder IdentityBuilder { get; set; }

        /// <summary>
        /// Never intended to be replaced. It's only here to allow for internal testing
        /// </summary>
        internal ITokenProvider TokenProvider { get; set; }
    }
}
