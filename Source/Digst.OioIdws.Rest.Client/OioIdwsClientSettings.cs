using System;
using System.Security.Cryptography.X509Certificates;

namespace Digst.OioIdws.Rest.Client
{
    public class OioIdwsClientSettings
    {
        /// <summary>
        /// Represents the client certificate including the private key. This should be either a MOCES, FOCES or VOCES certificate.
        /// </summary>
        public X509Certificate2 ClientCertificate { get; set; }
        public Uri AudienceUri { get; set; }
        /// <summary>
        /// Settings for the Security Token Service
        /// </summary>
        public OioIdwsStsSettings SecurityTokenService { get; set; }

        public Uri AccessTokenIssuerEndpoint { get; set; }
    }
}