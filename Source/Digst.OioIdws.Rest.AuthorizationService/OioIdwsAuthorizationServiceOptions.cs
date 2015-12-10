using System;

namespace Digst.OioIdws.Rest.AuthorizationService
{
    public class OioIdwsAuthorizationServiceOptions
    {
        public OioIdwsAuthorizationServiceOptions()
        {
            AccessTokenExpiration = TimeSpan.FromSeconds(3600);
        }

        public string IssueAccessTokenEndpoint { get; set; }
        public TimeSpan AccessTokenExpiration { get; set; }
    }
}