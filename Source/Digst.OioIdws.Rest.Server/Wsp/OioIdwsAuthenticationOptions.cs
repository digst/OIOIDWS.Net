using Digst.OioIdws.Rest.Server.AuthorizationServer.CertificateRetrieval;
using Microsoft.Owin.Security;

namespace Digst.OioIdws.Rest.Server.Wsp
{
    public class OioIdwsAuthenticationOptions : AuthenticationOptions
    {
        public OioIdwsAuthenticationOptions() : base("UseOioIdwsAuthentication")
        {
            CertificateRetrievalStrategy = null; 
            CertificateStrategyType = null; 
            CertificateStrategyValue = null;
        }

        /// <summary>
        /// Used when building an identity during authentication. Defaults to <see cref="Wsp.IdentityBuilder"/>.
        /// </summary>
        public IIdentityBuilder IdentityBuilder { get; set; }

        /// <summary>
        /// Provider for retrieving token information from an access token. Defaults to a <see cref="InMemoryTokenProvider"/>
        /// </summary>
        public ITokenProvider TokenProvider { get; set; }
        
        /// <summary>
        /// Explicit certificate retrieval strategy. If set, this overrides all other strategy configuration.
        /// </summary>
        public ICertificateRetrievalStrategy CertificateRetrievalStrategy { get; set; }

        /// <summary>
        /// Built-in certificate strategy type to use when no explicit strategy is provided.
        /// </summary>
        public CertificateStrategyType? CertificateStrategyType { get; set; }

        /// <summary>
        /// Optional value used by some strategies (e.g. header name).
        /// </summary>
        public string CertificateStrategyValue { get; set; }

    }
}
