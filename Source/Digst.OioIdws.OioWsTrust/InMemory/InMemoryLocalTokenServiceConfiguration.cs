using System;
using System.Security.Cryptography.X509Certificates;

namespace Digst.OioIdws.OioWsTrust.InMemory
{
    /// <summary>
    /// Configuration for the <see cref="InMemoryLocalTokenService"/>
    /// </summary>
    public class InMemoryLocalTokenServiceConfiguration
    {
        public InMemoryLocalTokenServiceConfiguration(X509Certificate2 tokenSigningCertificate, string entityId, TimeSpan tokenValidDuration)
        {
            TokenSigningCertificate = tokenSigningCertificate;
            EntityId = entityId;
            TokenValidDuration = tokenValidDuration;
        }

        /// <summary>
        /// Gets the token signing certificate.
        /// </summary>
        public X509Certificate2 TokenSigningCertificate { get; }

        /// <summary>
        /// Gets the duration for which the issued tokens will be valid.
        /// Issued tokens will be valid from current date/time and until
        /// current date/time plus this duration.
        /// </summary>
        public TimeSpan TokenValidDuration { get; }

        /// <summary>
        /// Gets the entity ID of this token service.
        /// </summary>
        public string EntityId { get; }
    }
}