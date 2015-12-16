using System;
using System.Security.Cryptography.X509Certificates;

namespace Digst.OioIdws.OioWsTrust
{
    public class TokenIssuingRequestConfiguration
    {
        /// <summary>
        /// Endpoint address of STS. E.g. https://SecureTokenService.test-nemlog-in.dk/SecurityTokenService.svc
        /// </summary>
        public string StsEndpointAddress { get; set; }

        /// <summary>
        /// Endpoint ID of WSP. E.g. https://saml.nnit001.dmz.inttest
        /// </summary>
        public string WspEndpointId { get; set; }

        /// <summary>
        /// Token life time can be specified in minutes. Default life time is chossen by STS if nothing is specified (8 hours according to the specification at the time of this writing).
        /// If specified, according to specification the STS is not obligated to honor this range and may return a token with a shorter life time in RSTR.
        /// All values above 480 minutes (8 hours) will result in a token life time of 8 hours.
        /// </summary>
        public int? TokenLifeTimeInMinutes { get; set; }

        public TimeSpan? SendTimeout { get; set; }

        /// <summary>
        /// Represents the client certificate including the private key. This should be either a MOCES, FOCES or VOCES certificate.
        /// </summary>
        public X509Certificate2 ClientCertificate { get; set; }

        /// <summary>
        /// Represents the STS certificate containing only the public key. This should be a FOCES certificate.
        /// </summary>
        public X509Certificate2 StsCertificate { get; set; }
    }
}
