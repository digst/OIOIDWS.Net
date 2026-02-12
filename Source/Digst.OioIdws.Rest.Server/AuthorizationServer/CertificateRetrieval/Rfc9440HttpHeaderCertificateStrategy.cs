using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Owin;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer.CertificateRetrieval
{

    /// <summary>
    /// Retrieves the client certificate from the HTTP header "Client-Cert" only.
    /// See https://www.rfc-editor.org/rfc/rfc9440 for header specification.
    /// </summary>
    public class Rfc9440HttpHeaderCertificateStrategy : ICertificateRetrievalStrategy
    {
        private readonly string _headerName;
        private const string DefaultHeaderName = "Client-Cert";

        /// <summary>
        /// Creates new strategy for certificate retrieval from HTTP headers.
        /// </summary>
        /// <param name="headerName">Configured HTTP header name.</param>
        public Rfc9440HttpHeaderCertificateStrategy(string headerName=DefaultHeaderName)
        {
            _headerName = string.IsNullOrWhiteSpace(headerName) ? DefaultHeaderName : headerName;
        }

        /// <inheritdoc />
        public X509Certificate2 GetCertificate(OioIdwsMatchEndpointContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            
            return GetCertificate(context.OwinContext);
        }
        
        /// <inheritdoc />
        public X509Certificate2 GetCertificate(IOwinContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!context.Request.Headers.ContainsKey(_headerName))
                throw new InvalidOperationException($"No HTTP header field {_headerName} in HTTP headers.");
            
            string headerCertificate = context.Request.Headers[_headerName];
            if(string.IsNullOrWhiteSpace(headerCertificate))
                throw new InvalidOperationException($"Certificate found in HTTP header field {_headerName}.");
            
            try
            {
                byte[] rawCert = Convert.FromBase64String(headerCertificate);
                return new X509Certificate2(rawCert);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException($"Invalid certificate in header {_headerName}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse certificate in header {_headerName}", ex);
            }
        }
    }
}
