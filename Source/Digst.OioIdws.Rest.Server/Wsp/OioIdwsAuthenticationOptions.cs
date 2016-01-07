using System;
using Digst.OioIdws.Rest.Server.AuthorizationServer;
using Microsoft.Owin.Security;

namespace Digst.OioIdws.Rest.Server.Wsp
{
    public class OioIdwsAuthenticationOptions : AuthenticationOptions
    {
        public OioIdwsAuthenticationOptions() : base("UseOioIdwsAuthentication")
        {
            
        }

        /// <summary>
        /// Used when building an identity during authentication. Defaults to <see cref="Wsp.IdentityBuilder"/>.
        /// </summary>
        public IIdentityBuilder IdentityBuilder { get; set; }

        /// <summary>
        /// Provider for retrieving token information from an access token. Defaults to a <see cref="InMemoryTokenProvider"/>
        /// </summary>
        public ITokenProvider TokenProvider { get; set; }
    }
}
