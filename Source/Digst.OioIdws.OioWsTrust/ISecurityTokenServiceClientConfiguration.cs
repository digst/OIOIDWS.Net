using System;
using System.Security.Cryptography.X509Certificates;

namespace Digst.OioIdws.OioWsTrust
{
    public interface ISecurityTokenServiceClientConfiguration
    {

        string StsIdentifier { get;  }

        Uri BootstrapTokenFromAuthenticationTokenUrl { get; }

        Uri IdentityTokenFromBootstrapTokenUrl { get; }

        Uri ServiceTokenUrl { get; }

        X509Certificate2 StsCertificate { get;  }

        TimeSpan? TokenLifeTime { get;  }

        TimeSpan? SendTimeout { get;  }

        string WscIdentifier { get;  }

        X509Certificate2 WscCertificate { get;  }
        TimeSpan CacheClockSkew { get; set; }
    }
}