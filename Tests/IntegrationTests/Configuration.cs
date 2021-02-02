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

        public StsTokenServiceConfiguration StsConfiguration { get; private set; }
        public SoapWspConfiguration SoapWspConfiguration { get; private set; }
        public RestWspConfiguration RestWspConfiguration { get; private set; }
        public LocalStsConfiguration LocalStsConfiguration { get; private set; }
        public OioIdwsClientSettings OioIdwsClientSettings { get; private set; }

        public static Configuration BuildDotNetWspConfiguration()
        {
            var soapWspConfiguration = new SoapWspConfiguration();
            var restWspConfiguration = new RestWspConfiguration();
            var wspEntityId = soapWspConfiguration.EntityID; // WSP Entity IDs are identical.
            
            var wscCertificate = Utils.ReadCertificateFile(ConfigurationManager.AppSettings[WscCertificatePathKey], "Test1234");
            
            var stsConfiguration = BuildStsConfiguration(wspEntityId, wscCertificate);
            var localStsConfiguration = BuildLocalStsConfiguration(wscCertificate);
            var oioIdwsClientSettings = BuildOioIdwsClientSettings(wscCertificate);

            return new Configuration
            {
                StsConfiguration = stsConfiguration,
                RestWspConfiguration = restWspConfiguration,
                SoapWspConfiguration = soapWspConfiguration,
                LocalStsConfiguration = localStsConfiguration,
                OioIdwsClientSettings = oioIdwsClientSettings
            };
        }

        private static LocalStsConfiguration BuildLocalStsConfiguration(X509Certificate2 wscCertificate)
        {
            return new LocalStsConfiguration
            {
                EntityId = ConfigurationManager.AppSettings[LocalTokenServiceEntityIdKey],
                SigningCertificate = Utils.ReadCertificateFile(ConfigurationManager.AppSettings[LocalTokenServiceSigningCertificatePathKey], "Test1234"),
                HolderOfKeyCertificate = wscCertificate
            };
        }

        private static StsTokenServiceConfiguration BuildStsConfiguration(string wspEntityId, X509Certificate2 wscCertificate)
        {
            var stsConfiguration = new StsTokenServiceConfiguration
            {
                ClientCertificate = wscCertificate,
                StsCertificate = Utils.ReadCertificateFile(ConfigurationManager.AppSettings[StsCertificatePathKey]),
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
    }
}