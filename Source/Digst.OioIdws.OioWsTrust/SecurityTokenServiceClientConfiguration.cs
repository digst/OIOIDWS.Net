using System;
using System.Security.Cryptography.X509Certificates;

namespace Digst.OioIdws.OioWsTrust
{
    public class SecurityTokenServiceClientConfiguration : ISecurityTokenServiceClientConfiguration
    {
        public string StsIdentifier { get; set; }

        public Uri BootstrapTokenFromAuthenticationTokenUrl { get; set; }

        public Uri IdentityTokenFromBootstrapTokenUrl { get; set; }

        public Uri ServiceTokenUrl { get; set; }

        public X509Certificate2 StsCertificate { get; set; }

        public TimeSpan? TokenLifeTime { get; set; }

        public TimeSpan? SendTimeout { get; set; }

        public string WscIdentifier { get; set; }

        public X509Certificate2 WscCertificate { get; set; }

        public TimeSpan CacheClockSkew { get; set; }
    }
}