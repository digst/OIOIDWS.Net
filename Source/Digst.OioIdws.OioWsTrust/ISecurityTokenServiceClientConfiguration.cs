using System;
using System.Security.Cryptography.X509Certificates;

namespace Digst.OioIdws.OioWsTrust
{
    public interface ISecurityTokenServiceClientConfiguration
    {

        /// <summary>
        /// The entity identifier of the securoty token service
        /// </summary>
        string StsIdentifier { get;  }

        /// <summary>
        /// The service endpoint where an authentication token can be exchanged for a bootstrap token.
        /// </summary>
        Uri BootstrapTokenFromAuthenticationTokenUrl { get; }

        /// <summary>
        /// The uri of the service endpoint where a bootstrap token can be exchanged for an identity token.
        /// </summary>
        Uri IdentityTokenFromBootstrapTokenUrl { get; }

        /// <summary>
        /// The uri of the service endpoint where a bootstrap token can be exchanged for an identity token.
        /// </summary>
        Uri ServiceTokenUrl { get; }

        /// <summary>
        /// The certificate of the security token service. This certificate is used to sign responses from, and tokens issued by, the security token service.
        /// </summary>
        X509Certificate2 StsCertificate { get;  }

        /// <summary>
        /// The default requested life time to be used when requesting tokens. Note that the security token service is not obligated to follow this value. 
        /// </summary>
        TimeSpan? TokenLifeTime { get;  }

        TimeSpan? SendTimeout { get;  }

        string WscIdentifier { get;  }

        X509Certificate2 WscCertificate { get;  }

        TimeSpan CacheClockSkew { get; set; }
    }
}