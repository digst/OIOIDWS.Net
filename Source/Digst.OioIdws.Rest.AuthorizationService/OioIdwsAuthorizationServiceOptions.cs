using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.AuthorizationService.Issuing;
using Microsoft.Owin;

namespace Digst.OioIdws.Rest.AuthorizationService
{
    public class OioIdwsAuthorizationServiceOptions
    {
        public OioIdwsAuthorizationServiceOptions()
        {
            AccessTokenExpiration = TimeSpan.FromSeconds(3600);
            AccessTokenGenerator = new AccessTokenGenerator();
            TokenValidator = new TokenValidator();
            ServiceTokenResolver = new X509CertificateStoreTokenResolver();
        }

        public PathString AccessTokenIssuerPath { get; set; }
        public TimeSpan AccessTokenExpiration { get; set; }
        public SecurityTokenResolver ServiceTokenResolver { get; set; }
        public PathString AccessTokenRetrievalPath { get; set; }
        /// <summary>
        /// Register the issuers and corresponding audiences that are allowed. To allow for dynamic changes, the func is invoked on each token validation.
        /// </summary>
        public Func<Task<IssuerAudiences[]>> IssuerAudiences { get; set; }

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