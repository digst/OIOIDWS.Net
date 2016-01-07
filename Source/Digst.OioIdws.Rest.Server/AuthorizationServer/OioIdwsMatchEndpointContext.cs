using System.Security.Cryptography.X509Certificates;
using Microsoft.Owin;
using Microsoft.Owin.Security.Provider;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer
{
    public class OioIdwsMatchEndpointContext : EndpointContext<OioIdwsAuthorizationServiceOptions>
    {
        public OioIdwsMatchEndpointContext(IOwinContext context, OioIdwsAuthorizationServiceOptions options) : base(context, options)
        {

        }

        public bool IsAccessTokenIssueEndpoint { get; set; }
        public bool IsAccessTokenRetrievalEndpoint { get; set; }
        public X509Certificate2 ClientCertificate { get; set; }
    }
}
