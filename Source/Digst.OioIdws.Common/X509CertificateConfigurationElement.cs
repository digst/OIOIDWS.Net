using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Digst.OioIdws.Common
{
    public class X509CertificateConfigurationElement : ConfigurationElement
    {
        /// <summary>Gets or sets a string that specifies the value to search for in the X.509 certificate store.</summary>
        /// <returns>The value to search for in the X.509 certificate store.</returns>
        [ConfigurationProperty("findValue", DefaultValue = "")]
        [StringValidator(MinLength = 0)]
        public string FindValue
        {
            get => (string)this["findValue"];
            set
            {
                if (string.IsNullOrEmpty(value))
                    value = string.Empty;
                this["findValue"] = (object)value;
            }
        }

        /// <summary>Gets or sets a value that specifies the location of the certificate store that the service can use to validate the client's certificate.</summary>
        /// <returns>A <see cref="T:System.Security.Cryptography.X509Certificates.StoreLocation" />. The default value is <see cref="F:System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine" />.</returns>
        [ConfigurationProperty("storeLocation", DefaultValue = StoreLocation.LocalMachine)]
        public StoreLocation StoreLocation
        {
            get => (StoreLocation)this["storeLocation"];
            set => this["storeLocation"] = (object)value;
        }

        /// <summary>Gets or sets the name of the X.509 certificate store to open.</summary>
        /// <returns>A <see cref="T:System.Security.Cryptography.X509Certificates.StoreName" /> that contains the name of the X.509 certificate store to open.</returns>
        [ConfigurationProperty("storeName", DefaultValue = StoreName.My)]
        public StoreName StoreName
        {
            get => (StoreName)this["storeName"];
            set => this["storeName"] = (object)value;
        }

        /// <summary>Gets or sets the type of X.509 search to be executed.</summary>
        /// <returns>A <see cref="T:System.Security.Cryptography.X509Certificates.X509FindType" /> that specifies the type of X.509 search to be executed.</returns>
        [ConfigurationProperty("x509FindType", DefaultValue = X509FindType.FindBySubjectDistinguishedName)]
        public X509FindType X509FindType
        {
            get => (X509FindType)this["x509FindType"];
            set => this["x509FindType"] = (object)value;
        }

        public X509Certificate2 GetCertificate()
        {
            var store = new X509Store(StoreName, StoreLocation);
            store.Open(OpenFlags.ReadOnly);
            var cert = store.Certificates.Find(X509FindType, FindValue, true).OfType<X509Certificate2>().OrderByDescending(x => x.NotAfter).FirstOrDefault();
            return cert;
        }
    }
}