using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Wsc.OioWsTrust;

namespace DK.Gov.Oio.Idws.IntegrationTests
{
    public class Configuration
    {
        private const string WscCertificatePathKey = "WscCertificatePath";
        private const string StsEndpointAddressKey = "StsEndpointAddress";
        private const string StsCertificatePathKey = "StsCertificatePath";
        private const string TokenLifeTimeInMinutesKey = "TokenLifeTimeInMinutes";
        private const string DotNetWspEntityIDKey = "DotNetWspEntityID";
        private const string DotNetWspHostnameKey = "DotNetWspHostname";
        private const string DotNetWspCertificatePathKey = "DotNetWspCertificatePath";
        
        public StsTokenServiceConfiguration StsConfiguration { get; private set; }
        public WspConfiguration WspConfiguration { get; private set; }

        public static Configuration BuildDotNetWspConfiguration()
        {
            var wspConfiguration = BuildDotNetWsp();
            
            var stsConfiguration = BuildStsConfiguration(wspConfiguration);
            
            return new Configuration
            {
                StsConfiguration = stsConfiguration,
                WspConfiguration = wspConfiguration
            };
        }
        
        private static WspConfiguration BuildDotNetWsp()
        {
            var wspConfiguration = new WspConfiguration
            {
                EntityID = ConfigurationManager.AppSettings[DotNetWspEntityIDKey],
                Hostname = ConfigurationManager.AppSettings[DotNetWspHostnameKey],
                Certificate = ReadCertificateFile(ConfigurationManager.AppSettings[DotNetWspCertificatePathKey], "Test1234")
            };
            return wspConfiguration;
        }

        private static StsTokenServiceConfiguration BuildStsConfiguration(WspConfiguration wspConfiguration)
        {
            var stsConfiguration = new StsTokenServiceConfiguration
            {
                ClientCertificate = ReadCertificateFile(ConfigurationManager.AppSettings[WscCertificatePathKey], "Test1234"),
                StsCertificate = ReadCertificateFile(ConfigurationManager.AppSettings[StsCertificatePathKey]),
                SendTimeout = null,
                StsEndpointAddress = ConfigurationManager.AppSettings[StsEndpointAddressKey],
                TokenLifeTimeInMinutes = int.Parse(ConfigurationManager.AppSettings[TokenLifeTimeInMinutesKey]),
                WspEndpointId = wspConfiguration.EntityID
            };
            return stsConfiguration;
        }

        private static X509Certificate2 ReadCertificateFile(string path)
        {
            return new X509Certificate2(path);
        }
        
        private static X509Certificate2 ReadCertificateFile(string path, string password)
        {
            return new X509Certificate2(path, password, X509KeyStorageFlags.Exportable);
        }
    }
}