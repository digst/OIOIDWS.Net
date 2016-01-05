using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Server.Issuing;
using Digst.OioIdws.Rest.Server.TokenStorage;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Digst.OioIdws.Rest.Server
{
    public class OioIdwsAuthorizationServiceOptions : AuthenticationOptions
    {
        public OioIdwsAuthorizationServiceOptions() : base("OioIdwsAuthorizationService")
        {
            AccessTokenExpiration = TimeSpan.FromSeconds(3600);
            AccessTokenGenerator = new AccessTokenGenerator();
            TokenValidator = new TokenValidator();
            ServiceTokenResolver = new X509CertificateStoreTokenResolver();
        }

        /// <summary>
        /// Path on the server where access tokens are being issued
        /// </summary>
        public PathString AccessTokenIssuerPath { get; set; }
        /// <summary>
        /// expires_in for access token being issued. Defaults to one hour, which is the highest expiration time adviced by the specification
        /// </summary>
        public TimeSpan AccessTokenExpiration { get; set; }
        /// <summary>
        /// Path on the server where user information is retrieved by serving it an access token
        /// </summary>
        public PathString AccessTokenRetrievalPath { get; set; }
        /// <summary>
        /// Used when decrypting and validating the Security Token. Defaults to <see cref="X509CertificateStoreTokenResolver"/> using store 'Local Machine/My'
        /// </summary>
        public SecurityTokenResolver ServiceTokenResolver { get; set; }
        /// <summary>
        /// Register the issuers and corresponding audiences that are allowed per issuer. To allow for dynamic changes, the func is invoked on each token validation.
        /// </summary>
        public Func<Task<IssuerAudiences[]>> IssuerAudiences { get; set; }
        /// <summary>
        /// For storing security tokens. Default to <see cref="InMemorySecurityTokenStore"/> which should be sufficient when not running the AuthorizationService in a distributed fashion
        /// </summary>
        public ISecurityTokenStore SecurityTokenStore { get; set; }
        /// <summary>
        /// Never intended to be replaced. It's only here to allow for internal testing
        /// </summary>
        internal IAccessTokenGenerator AccessTokenGenerator { get; set; }
        /// <summary>
        /// Never intended to be replaced. It's only here to allow for internal testing
        /// </summary>
        internal ITokenValidator TokenValidator { get; set; }
    }
}