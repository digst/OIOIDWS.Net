using System;
using System.Configuration;
using Digst.OioIdws.Common.Utils;

namespace Digst.OioIdws.OioWsTrust.InMemory
{
    /// <summary>
    /// Produces <see cref="InMemoryLocalTokenServiceConfiguration"/> instances.
    /// </summary>
    public class InMemoryLocalTokenServiceConfigurationFactory
    {
        /// <summary>
        /// Creates configuration from the configuration file
        /// </summary>
        public static InMemoryLocalTokenServiceConfiguration CreateConfiguration()
        {
            var thumbprint = ConfigurationManager.AppSettings["LocalTokenServiceSigningCertificateThumbprint"];
            var entityId = ConfigurationManager.AppSettings["LocalTokenServiceEntityId"];

            var tokenSigningCertificate = CertificateUtil.GetCertificate(thumbprint);

            return new InMemoryLocalTokenServiceConfiguration(tokenSigningCertificate, entityId, TimeSpan.FromMinutes(10));
        }
    }
}