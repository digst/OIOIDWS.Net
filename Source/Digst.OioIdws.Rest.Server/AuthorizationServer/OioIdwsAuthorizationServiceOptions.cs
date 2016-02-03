using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Server.AuthorizationServer.Issuing;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;
using Digst.OioIdws.Rest.Server.Wsp;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer
{
    public class OioIdwsAuthorizationServiceOptions : AuthenticationOptions
    {
        public OioIdwsAuthorizationServiceOptions() : base("OioIdwsAuthorizationService")
        {
            AccessTokenExpiration = TimeSpan.FromSeconds(3600);
            KeyGenerator = new KeyGenerator();
            TokenValidator = new TokenValidator();
            ServiceTokenResolver = new X509CertificateStoreTokenResolver();
            CertificateValidator = X509CertificateValidator.ChainTrust;
            MaxClockSkew = TimeSpan.FromMinutes(5);
            SystemClock = new SystemClock();
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
        /// Path on the server where user information is retrieved by serving it an access token. This is required if the <see cref="OioIdwsAuthenticationOptions.TokenProvider"/> is set to <see cref="RestTokenProvider"/> on the <see cref="OioIdwsAuthenticationMiddleware"/>
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
        /// Used for validating client certificates
        /// </summary>
        public X509CertificateValidator CertificateValidator { get; set; }
        /// <summary>
        /// Never intended to be replaced. It's only here to allow for internal testing
        /// </summary>
        internal IKeyGenerator KeyGenerator { get; set; }
        /// <summary>
        /// Never intended to be replaced. It's only here to allow for internal testing
        /// </summary>
        internal ITokenValidator TokenValidator { get; set; }
        /// <summary>
        /// Used during token validation (access token issuing) and when token information is accessed
        /// </summary>
        public TimeSpan MaxClockSkew { get; set; }
        /// <summary>
        /// The data format used to protect the information contained in the access token. 
        /// If not provided by the application the default data protection provider depends on the host server. 
        /// The SystemWeb host on IIS will use ASP.NET machine key data protection, and HttpListener and other self-hosted
        /// servers will use DPAPI data protection.
        /// </summary>
        public ISecureDataFormat<AuthenticationProperties> TokenDataFormat { get; set; }
        /// <summary>
        /// When the <see cref="AccessTokenRetrievalPath"/> is used, the WSP must provide a client certificate from this trusted list
        /// </summary>
        public IEnumerable<string> TrustedWspCertificateThumbprints { get; set; }
        /// <summary>
        /// Used to know what the current clock time is when calculating or validating token expiration. When not assigned default is based on
        /// DateTimeOffset.UtcNow. This is typically needed only for unit testing.
        /// </summary>
        public ISystemClock SystemClock { get; set; }
    }
}