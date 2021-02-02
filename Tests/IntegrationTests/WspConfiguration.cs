using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;

namespace DK.Gov.Oio.Idws.IntegrationTests
{
    public class WspConfiguration
    {
        private const string DotNetWspEntityIdKey = "DotNetWspEntityId";

        public WspConfiguration(string endpointKey)
        {
            EntityID = ConfigurationManager.AppSettings[DotNetWspEntityIdKey];
            Endpoint = new Uri(ConfigurationManager.AppSettings[endpointKey]);
        }
        public string EntityID { get; }
        public Uri Endpoint { get; }
    }

    public class RestWspConfiguration : WspConfiguration
    {
        private const string DotNetRestWspEndpointKey = "DotNetRestWspEndpoint";

        public RestWspConfiguration() : base(DotNetRestWspEndpointKey) { }
    }

    public class SoapWspConfiguration : WspConfiguration
    {
        private const string DotNetSoapWspEndpointKey = "DotNetSoapWspEndpoint";
        private const string DotNetSoapWspCertificatePathKey = "DotNetSoapWspCertificatePath";

        public SoapWspConfiguration() : base(DotNetSoapWspEndpointKey)
        {
            Certificate = Utils.ReadCertificateFile(ConfigurationManager.AppSettings[DotNetSoapWspCertificatePathKey], "Test1234");
        }
        
        public X509Certificate2 Certificate { get; }

        private string CertificateCommonName => Certificate.GetNameInfo(X509NameType.SimpleName, false);
        
        public EndpointAddress EndpointAddress => new EndpointAddress(Endpoint, EndpointIdentity.CreateDnsIdentity(CertificateCommonName));
    }
}