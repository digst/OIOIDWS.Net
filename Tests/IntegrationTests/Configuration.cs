using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Rest.Client;

namespace DK.Gov.Oio.Idws.IntegrationTests
{
    public class Configuration
    {
        private const string WscCertificatePathKey = "WscCertificatePath";
        private const string TokenLifeTimeInMinutesKey = "TokenLifeTimeInMinutes";
        private const string DotNetRestAccessTokenIssuerEndpointKey = "DotNetRestAccessTokenIssuerEndpoint";
        private const string StsEndpointAddressKey = "StsEndpointAddress";
        private const string StsCertificatePathKey = "StsCertificatePath";
        private const string LocalTokenServiceSigningCertificatePathKey = "LocalTokenServiceSigningCertificatePath";
        private const string LocalTokenServiceEntityIdKey = "LocalTokenServiceEntityId";
        private const string DotNetWspEntityIdKey = "DotNetWspEntityId";
        private const string DotNetRestWspEndpointKey = "DotNetRestWspEndpoint";
        private const string DotNetSoapWspEndpointKey = "DotNetSoapWspEndpoint";
        private const string DotNetSoapWspCertificatePathKey = "DotNetSoapWspCertificatePath";
        private const string DotNetBootstrapWscEndpointKey = "DotNetBootstrapWscEndpoint";
        private const string DotNetBootstrapWscUsernameKey = "DotNetBootstrapWscUsername";
        private const string DotNetBootstrapWscPasswordKey = "DotNetBootstrapWscPassword";

        public StsTokenServiceConfiguration StsConfiguration { get; private set; }
        public SoapWspConfiguration SoapWspConfiguration { get; private set; }
        public RestWspConfiguration RestWspConfiguration { get; private set; }
        public LocalStsConfiguration LocalStsConfiguration { get; private set; }
        public OioIdwsClientSettings OioIdwsClientSettings { get; private set; }
        public BootstrapWscConfiguration BootstrapWscConfiguration { get; private set; }

        public static Configuration BuildDotNetWspConfiguration()
        {
            var restWspConfiguration = BuildDotNetRestWspConfiguration();
            var soapWspConfiguration = BuildDotNetSoapWspConfiguration();
            
            var wspEntityId = soapWspConfiguration.EntityId; // WSP Entity IDs are identical.
            var wscCertificate = ReadCertificateFile(ConfigurationManager.AppSettings[WscCertificatePathKey], "Test1234");
            
            var stsConfiguration = BuildStsConfiguration(wspEntityId, wscCertificate);
            var localStsConfiguration = BuildLocalStsConfiguration(wscCertificate);
            var oioIdwsClientSettings = BuildOioIdwsClientSettings(wscCertificate);
            var bootstrapWscConfiguration = BuildBootstrapWscConfiguration();

            return new Configuration
            {
                StsConfiguration = stsConfiguration,
                LocalStsConfiguration = localStsConfiguration,
                OioIdwsClientSettings = oioIdwsClientSettings,
                RestWspConfiguration = restWspConfiguration,
                SoapWspConfiguration = soapWspConfiguration,
                BootstrapWscConfiguration = bootstrapWscConfiguration
            };
        }

        private static BootstrapWscConfiguration BuildBootstrapWscConfiguration() =>
            new BootstrapWscConfiguration
            {
                WscEndpoint = new Uri(ConfigurationManager.AppSettings[DotNetBootstrapWscEndpointKey]),
                WscUsername = ConfigurationManager.AppSettings[DotNetBootstrapWscUsernameKey],
                WscPassword = ConfigurationManager.AppSettings[DotNetBootstrapWscPasswordKey]
            };

        private static SoapWspConfiguration BuildDotNetSoapWspConfiguration() =>
            new SoapWspConfiguration
            {
                EntityId = ConfigurationManager.AppSettings[DotNetWspEntityIdKey],
                Endpoint = new Uri(ConfigurationManager.AppSettings[DotNetSoapWspEndpointKey]),
                Certificate = ReadCertificateFile(ConfigurationManager.AppSettings[DotNetSoapWspCertificatePathKey], "Test1234"),
            };

        private static RestWspConfiguration BuildDotNetRestWspConfiguration() =>
            new RestWspConfiguration
            {
                EntityId = ConfigurationManager.AppSettings[DotNetWspEntityIdKey],
                Endpoint = new Uri(ConfigurationManager.AppSettings[DotNetRestWspEndpointKey]),
            };

        private static LocalStsConfiguration BuildLocalStsConfiguration(X509Certificate2 wscCertificate)
        {
            return new LocalStsConfiguration
            {
                EntityId = ConfigurationManager.AppSettings[LocalTokenServiceEntityIdKey],
                SigningCertificate = ReadCertificateFile(ConfigurationManager.AppSettings[LocalTokenServiceSigningCertificatePathKey], "Test1234"),
                HolderOfKeyCertificate = wscCertificate
            };
        }

        private static StsTokenServiceConfiguration BuildStsConfiguration(string wspEntityId, X509Certificate2 wscCertificate)
        {
            var stsConfiguration = new StsTokenServiceConfiguration
            {
                ClientCertificate = wscCertificate,
                StsCertificate = ReadCertificateFile(ConfigurationManager.AppSettings[StsCertificatePathKey]),
                SendTimeout = null,
                StsEndpointAddress = ConfigurationManager.AppSettings[StsEndpointAddressKey],
                TokenLifeTimeInMinutes = int.Parse(ConfigurationManager.AppSettings[TokenLifeTimeInMinutesKey]),
                WspEndpointId = wspEntityId
            };
            return stsConfiguration;
        }

        private static OioIdwsClientSettings BuildOioIdwsClientSettings(X509Certificate2 wscCertificate)
        {
            var oioIdwsClientSettings = new OioIdwsClientSettings
            {
                ClientCertificate = wscCertificate,
                AccessTokenIssuerEndpoint = new Uri(ConfigurationManager.AppSettings[DotNetRestAccessTokenIssuerEndpointKey]),
                DesiredAccessTokenExpiry = TimeSpan.FromMinutes(5)
            };

            return oioIdwsClientSettings;
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