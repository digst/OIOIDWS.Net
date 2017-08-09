using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using Digst.OioIdws.OioWsTrust;

namespace Digst.OioIdws.Wsc.OioWsTrust
{
    public class Configuration : ConfigurationSection
    {
        /// <summary>
        /// Endpoint address of STS. E.g. https://SecureTokenService.test-nemlog-in.dk/SecurityTokenService.svc
        /// </summary>
        [ConfigurationProperty("stsEndpointAddress", IsRequired = true)]
        public string StsEndpointAddress
        {
            get
            {
                return (string)this["stsEndpointAddress"];
            }
            set
            {
                this["stsEndpointAddress"] = value;
            }
        }

        /// <summary>
        /// Endpoint ID of WSP. E.g. https://saml.nnit001.dmz.inttest
        /// </summary>
        [ConfigurationProperty("wspEndpointID", IsRequired = true)]
        public string WspEndpointID
        {
            get
            {
                return (string)this["wspEndpointID"];
            }
            set
            {
                this["wspEndpointID"] = value;
            }
        }

        /// <summary>
        /// Token life time can be specified in minutes. Default life time is chossen by STS if nothing is specified (8 hours according to the specification at the time of this writing).
        /// If specified, according to specification the STS is not obligated to honor this range and may return a token with a shorter life time in RSTR.
        /// All values above 480 minutes (8 hours) will result in a token life time of 8 hours.
        /// </summary>
        [ConfigurationProperty("tokenLifeTimeInMinutes", IsRequired = false)]
        public int? TokenLifeTimeInMinutes
        {
            get
            {
                return (int?)this["tokenLifeTimeInMinutes"];
            }
            set
            {
                this["tokenLifeTimeInMinutes"] = value;
            }
        }

        /// <summary>
        /// If set to true the call timeout to the STS is set to 1 day. This is needed when a developer wants to do debugging and needs more than 1 minute to do the debugging.
        /// </summary>
        [ConfigurationProperty("debugMode", DefaultValue = false, IsRequired = false)]
        public bool DebugMode
        {
            get
            {
                return (bool)this["debugMode"];
            }
            set
            {
                this["debugMode"] = value;
            }
        }

        /// <summary>
        /// Represents the client certificate including the private key. This should be either a MOCES, FOCES or VOCES certificate.
        /// </summary>
        [ConfigurationProperty("clientCertificate", IsRequired = true)]
        public Certificate ClientCertificate
        {
            get
            {
                return (Certificate) this["clientCertificate"];
            }
            set
            {
                this["clientCertificate"] = value;
            }
        }

        /// <summary>
        /// Represents the STS certificate containing only the public key. This should be a FOCES certificate.
        /// </summary>
        [ConfigurationProperty("stsCertificate", IsRequired = true)]
        public Certificate StsCertificate
        {
            get
            {
                return (Certificate)this["stsCertificate"];
            }
            set
            {
                this["stsCertificate"] = value;
            }
        }

        /// <summary>
        /// This is used to determine how many seconds before the token actually expires ... the token should be removed from the cache.
        /// E.g. if token will expire in 100 seconds and <see cref="CacheClockSkewInSeconds"/> is set to 10 seconds ... then the token will be removed from the cache after 90 seconds.
        /// If not set ... the default value is 300 seconds.
        /// This configuration setting is only used in conjunction with <see cref="TokenServiceCache"/>
        /// </summary>
        [ConfigurationProperty("cacheClockSkewInSeconds", IsRequired = false)]
        public int? CacheClockSkewInSeconds
        {
            get
            {
                return (int?)this["cacheClockSkewInSeconds"];
            }
            set
            {
                this["cacheClockSkewInSeconds"] = value;
            }
        }
    }

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
