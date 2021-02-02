using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;

namespace DK.Gov.Oio.Idws.IntegrationTests
{
    public class SoapWspConfiguration
    {
        public string EntityId { get; set; }
        public Uri Endpoint { get; set; }
        public X509Certificate2 Certificate { get; set; }

        private string CertificateCommonName => Certificate.GetNameInfo(X509NameType.SimpleName, false);
        
        public EndpointAddress EndpointAddress => new EndpointAddress(Endpoint, EndpointIdentity.CreateDnsIdentity(CertificateCommonName));
    }
}