using System;

namespace Digst.OioIdws.Rest.ProviderAuthentication
{
    public class OioIdwsProviderAuthenticationOptions
    {
        public Uri AccessTokenRetrievalEndpoint { get; set; }

        /// <summary>
        /// Never intended to be replaced. It's only here to allow for internal testing
        /// </summary>
        internal ITokenProvider TokenProvider { get; set; }
    }
}
