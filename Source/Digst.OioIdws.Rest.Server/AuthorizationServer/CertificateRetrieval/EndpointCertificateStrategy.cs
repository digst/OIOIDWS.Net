using System.Security.Cryptography.X509Certificates;
using Microsoft.Owin;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer.CertificateRetrieval
{
    /// <summary>
    ///     Retrieves the client certificate directly from the TLS connection.
    /// </summary>
    public class EndpointCertificateStrategy : ICertificateRetrievalStrategy
    {
        /// <inheritdoc />
        public X509Certificate2 GetCertificate(IOwinContext context)
        {
            if (context == null)
                return null;
            
            return context.Get<X509Certificate2>("ssl.ClientCertificate");
        }
    }
}