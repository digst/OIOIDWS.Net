using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Digst.OioIdws.Wsc.OioWsTrust
{
    public class CertificateUtil
    {
        /// <summary>
        /// Finds a certificate in the certificate store.
        /// </summary>
        /// <param name="storeName">Name of store</param>
        /// <param name="storeLocation">Location of store</param>
        /// <param name="x509FindType">Find type that <see cref="findValue"/> must match</param>
        /// <param name="findValue">Value corresponding to <see cref="x509FindType"/></param>
        /// <returns>Return a X509Certificate2 certificate or null if a certificate was not found.</returns>
        public static X509Certificate2 GetCertificate(StoreName storeName, StoreLocation storeLocation, X509FindType x509FindType, string findValue)
        {
            var store = new X509Store(storeName, storeLocation);

            store.Open(OpenFlags.ReadOnly);

            var certificate = store.Certificates.Find(x509FindType, findValue,
                true).OfType<X509Certificate2>().FirstOrDefault();

            if (certificate == null)
                throw new InvalidOperationException(
                    string.Format("Certificate with the following configuration was not found: {0}, {1}, {2}, {3}",
                        storeLocation, storeName, x509FindType,findValue));
            
            return certificate;
        }

        /// <summary>
        /// Gets a certificate based on a oioidws configuration.
        /// </summary>
        /// <param name="certificate">oioidws configuration of a certificate.</param>
        /// <returns></returns>
        public static X509Certificate2 GetCertificate(Certificate certificate)
        {
            return GetCertificate(certificate.StoreName, certificate.StoreLocation,
                certificate.X509FindType, certificate.FindValue);
        }
    }
}
