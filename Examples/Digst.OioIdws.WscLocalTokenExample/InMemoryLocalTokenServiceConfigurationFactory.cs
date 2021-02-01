using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using Digst.OioIdws.Wsc.OioWsTrust;

namespace Digst.OioIdws.WscLocalTokenExample
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

            var tokenSigningCertificate = CertificateUtil.GetCertificate(StoreName.My, StoreLocation.LocalMachine, X509FindType.FindByThumbprint, thumbprint);

            return new InMemoryLocalTokenServiceConfiguration(tokenSigningCertificate, entityId, TimeSpan.FromMinutes(10));
        }
    }
}