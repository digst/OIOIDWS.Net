using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using Digst.OioIdws.OioWsTrust;

namespace DK.Gov.Oio.Idws.IntegrationTests
{
    public class Configuration
    {
        private const string WscCertificatePathKey = "WscCertificatePath";
        private const string TokenLifeTimeInMinutesKey = "TokenLifeTimeInMinutes";
        private const string StsEndpointAddressKey = "StsEndpointAddress";
        private const string StsCertificatePathKey = "StsCertificatePath";
        private const string LocalTokenServiceSigningCertificatePathKey = "LocalTokenServiceSigningCertificatePath";
        private const string LocalTokenServiceEntityIdKey = "LocalTokenServiceEntityId";
        
        private const string DotNetWspEntityIdKey = "DotNetWspEntityId";
        private const string DotNetRestWspEndpointKey = "DotNetRestWspEndpoint";
        private const string DotNetRestWspTokenIssuanceEndpointKey = "DotNetRestWspTokenIssuanceEndpoint";
        private const string DotNetSoapWspEndpointKey = "DotNetSoapWspEndpoint";
        private const string DotNetSoapWspCertificatePathKey = "DotNetSoapWspCertificatePath";
        
        private const string JavaWspEntityIdKey = "JavaWspEntityId";
        private const string JavaRestWspEndpointKey = "JavaRestWspEndpoint";
        private const string JavaRestWspTokenIssuanceEndpointKey = "JavaRestWspTokenIssuanceEndpoint";
        private const string JavaSoapWspEndpointKey = "JavaSoapWspEndpoint";
        private const string JavaSoapWspCertificatePathKey = "JavaSoapWspCertificatePath";

        private static readonly X509Certificate2 WscCertificate =
            ReadCertificateFile(ConfigurationManager.AppSettings[WscCertificatePathKey], "Test1234");
        
        public StsTokenServiceConfiguration StsConfiguration { get; private set; }
        public SoapWspConfiguration SoapWspConfiguration { get; private set; }
        public RestWspConfiguration RestWspConfiguration { get; private set; }
        public LocalStsConfiguration LocalStsConfiguration { get; private set; }
        
        /// <summary>
        /// Build configuration for .NET WSC - .NET WSP scenarios.
        /// </summary>
        public static Configuration BuildDotNetWspConfiguration()
        {
            var restWspConfiguration = BuildDotNetRestWspConfiguration();
            var soapWspConfiguration = BuildDotNetSoapWspConfiguration();

            return BuildConfiguration(restWspConfiguration, soapWspConfiguration);
        }
        
        /// <summary>
        /// Build configuration for .NET WSC - Java WSP scenarios.
        /// </summary>
        public static Configuration BuildJavaWspConfiguration()
        {
            var restWspConfiguration = BuildJavaRestWspConfiguration();
            var soapWspConfiguration = BuildJavaSoapWspConfiguration();

            return BuildConfiguration(restWspConfiguration, soapWspConfiguration);
        }
        
        private static Configuration BuildConfiguration(RestWspConfiguration restWspConfiguration, SoapWspConfiguration soapWspConfiguration)
        {
            var stsConfiguration = BuildStsConfiguration();
            var localStsConfiguration = BuildLocalStsConfiguration();

            // Same configuration values across .NET/Java WSP-scenarios.
            restWspConfiguration.DesiredAccessTokenExpiry = TimeSpan.FromMinutes(5);
            restWspConfiguration.ClientCertificate = WscCertificate;
            
            // WSP Entity ID is the same whether it's the REST or SOAP variant of the WSP.
            stsConfiguration.WspEndpointId = soapWspConfiguration.EntityId;
            
            return new Configuration
            {
                StsConfiguration = stsConfiguration,
                LocalStsConfiguration = localStsConfiguration,
                RestWspConfiguration = restWspConfiguration,
                SoapWspConfiguration = soapWspConfiguration,
            };
        }

        private static SoapWspConfiguration BuildJavaSoapWspConfiguration() =>
            new SoapWspConfiguration
            {
                EntityId = ConfigurationManager.AppSettings[JavaWspEntityIdKey],
                Endpoint = new Uri(ConfigurationManager.AppSettings[JavaSoapWspEndpointKey]),
                Certificate = ReadCertificateFile(ConfigurationManager.AppSettings[JavaSoapWspCertificatePathKey], "Test1234"),
                SecurityAlgorithmSuite = SecurityAlgorithmSuite.Basic256Sha256
            };

        private static RestWspConfiguration BuildJavaRestWspConfiguration() =>
            new RestWspConfiguration
            {
                AudienceUri = new Uri(ConfigurationManager.AppSettings[JavaWspEntityIdKey]),
                Endpoint = new Uri(ConfigurationManager.AppSettings[JavaRestWspEndpointKey]),
                AccessTokenIssuerEndpoint = new Uri(ConfigurationManager.AppSettings[JavaRestWspTokenIssuanceEndpointKey]),
            };

        private static SoapWspConfiguration BuildDotNetSoapWspConfiguration() =>
            new SoapWspConfiguration
            {
                EntityId = ConfigurationManager.AppSettings[DotNetWspEntityIdKey],
                Endpoint = new Uri(ConfigurationManager.AppSettings[DotNetSoapWspEndpointKey]),
                Certificate = ReadCertificateFile(ConfigurationManager.AppSettings[DotNetSoapWspCertificatePathKey], "Test1234"),
                SecurityAlgorithmSuite = SecurityAlgorithmSuite.Default
            };

        private static RestWspConfiguration BuildDotNetRestWspConfiguration()
            => new RestWspConfiguration
            {
                AudienceUri = new Uri(ConfigurationManager.AppSettings[DotNetWspEntityIdKey]),
                Endpoint = new Uri(ConfigurationManager.AppSettings[DotNetRestWspEndpointKey]),
                AccessTokenIssuerEndpoint =
                    new Uri(ConfigurationManager.AppSettings[DotNetRestWspTokenIssuanceEndpointKey]),
            };

        private static LocalStsConfiguration BuildLocalStsConfiguration()
        {
            return new LocalStsConfiguration
            {
                EntityId = ConfigurationManager.AppSettings[LocalTokenServiceEntityIdKey],
                SigningCertificate = ReadCertificateFile(ConfigurationManager.AppSettings[LocalTokenServiceSigningCertificatePathKey], "Test1234"),
                HolderOfKeyCertificate = WscCertificate
            };
        }

        private static StsTokenServiceConfiguration BuildStsConfiguration()
        {
            var stsConfiguration = new StsTokenServiceConfiguration
            {
                ClientCertificate = WscCertificate,
                StsCertificate = ReadCertificateFile(ConfigurationManager.AppSettings[StsCertificatePathKey]),
                SendTimeout = null,
                StsEndpointAddress = ConfigurationManager.AppSettings[StsEndpointAddressKey],
                TokenLifeTimeInMinutes = int.Parse(ConfigurationManager.AppSettings[TokenLifeTimeInMinutesKey]),
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