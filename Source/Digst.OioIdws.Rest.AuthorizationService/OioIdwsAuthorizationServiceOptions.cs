using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using Digst.OioIdws.Rest.AuthorizationService.Issuing;
using Digst.OioIdws.Rest.AuthorizationService.Storage;
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

        public PathString IssueAccessTokenEndpoint { get; set; }
        public TimeSpan AccessTokenExpiration { get; set; }
        public SecurityTokenResolver ServiceTokenResolver { get; set; }
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