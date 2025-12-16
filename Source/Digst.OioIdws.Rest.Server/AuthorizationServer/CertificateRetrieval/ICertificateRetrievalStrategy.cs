using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer.CertificateRetrieval
{
    /// <summary>
    ///     Defines a strategy for retrieving a client certificate
    ///     from the current OIOIDWS endpoint context.
    /// </summary>
    public interface ICertificateRetrievalStrategy
    {
        /// <summary>
        ///     Retrieves a client certificate from the given context.
        /// </summary>
        /// <param name="context">The OIOIDWS endpoint context.</param>
        /// <returns>
        ///     A <see cref="X509Certificate2" /> if found; otherwise, <c>null</c>.
        /// </returns>
        X509Certificate2 GetCertificate(OioIdwsMatchEndpointContext context);
    }
}