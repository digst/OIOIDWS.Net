using Digst.OioIdws.Rest.Server.AuthorizationServer.CertificateRetrieval;
using Microsoft.Owin.Security;

namespace Digst.OioIdws.Rest.Server.Wsp
{
    /// <inheritdoc />
    public class OioIdwsAuthenticationOptions : AuthenticationOptions
    {
        private ICertificateRetrievalStrategy _strategy;

        /// <inheritdoc />
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
        
        /// <summary>
        /// Explicit certificate retrieval strategy. If set, this overrides all other strategy configuration.
        /// </summary>
        public ICertificateRetrievalStrategy CertificateRetrievalStrategy {
            get
            {
                return _strategy ?? CertificateStrategyFactory.Create(this);
            }
            set
            {
                _strategy = value;
            }
            
        }

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
