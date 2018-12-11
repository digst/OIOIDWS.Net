using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace Digst.OioIdws.Wsc.OioWsTrust
{
    public class Certificate : ConfigurationElement
    {
        /// <summary>
        /// Only values defined in the <see cref="StoreLocation"/> enum can be specified.
        /// </summary>
        [ConfigurationProperty("storeLocation", IsRequired = true)]
        public StoreLocation StoreLocation
        {
            get
            {
                return (StoreLocation)this["storeLocation"];
            }
            set
            {
                this["storeLocation"] = value;
            }
        }

        /// <summary>
        /// Only values defined in the <see cref="StoreName"/> enum can be specified.
        /// </summary>
        [ConfigurationProperty("storeName", IsRequired = true)]
        public StoreName StoreName
        {
            get
            {
                return (StoreName)this["storeName"];
            }
            set
            {
                this["storeName"] = value;
            }
        }

        /// <summary>
        /// Only values defined in the <see cref="X509FindType"/> enum can be specified
        /// </summary>
        [ConfigurationProperty("x509FindType", IsRequired = true)]
        public X509FindType X509FindType
        {
            get
            {
                return (X509FindType)this["x509FindType"];
            }
            set
            {
                this["x509FindType"] = value;
            }
        }

        /// <summary>
        /// A value representing the type defined in <see cref="X509FindType"/>
        /// </summary>
        [ConfigurationProperty("findValue", IsRequired = true)]
        public string FindValue
        {
            get
            {
                return (string)this["findValue"];
            }
            set
            {
                this["findValue"] = value;
            }
        }
    }
}