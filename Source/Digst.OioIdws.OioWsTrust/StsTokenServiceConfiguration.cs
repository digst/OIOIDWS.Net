using System;
using System.Security.Cryptography.X509Certificates;

namespace Digst.OioIdws.OioWsTrust
{
    public class StsTokenServiceConfiguration
    {
        public StsTokenServiceConfiguration()
        {
            CacheClockSkew = TimeSpan.FromSeconds(60);
        }

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
        /// All values above 480 minutes (8 hours) will result in a token life time of 8 hours from STS.
        /// All values below 1 minute will result in a token life time of 8 hours from STS.
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

        /// <summary>
        /// This is used to determine how long before the token actually expires ... the token should be removed from the cache.
        /// E.g. if token will expire in 100 seconds and <see cref="CacheClockSkew"/> is set to 10 seconds ... then the token will be removed from the cache after 90 seconds.
        /// If not set ... the default value is 60 seconds.
        /// This configuration setting is only used in conjunction with <see cref="StsTokenServiceCache"/>
        /// </summary>
        public TimeSpan CacheClockSkew { get; set; }
    }
}
