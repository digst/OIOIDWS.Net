using System;
using System.ComponentModel;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using Digst.OioIdws.Common;

namespace Digst.OioIdws.OioWsTrust
{
    public class SecurityTokenServiceClientConfigurationSection : ConfigurationSection, ISecurityTokenServiceClientConfiguration
    {
        [ConfigurationProperty("stsIdentifier")]
        public string StsIdentifier
        {
            get => (string)this["stsIdentifier"];
            set => this["stsIdentifier"] = value;
        }



        [ConfigurationProperty("bootstrapTokenFromAuthenticationTokenUrl")]
        [TypeConverter(typeof(UriTypeConverter))]
        public Uri BootstrapTokenFromAuthenticationTokenUrl
        {
            get => (Uri)this["bootstrapTokenFromAuthenticationTokenUrl"];
            set => this["bootstrapTokenFromAuthenticationTokenUrl"] = value;
        }

        [ConfigurationProperty("identityTokenFromBootstrapTokenUrl")]
        [TypeConverter(typeof(UriTypeConverter))]
        public Uri IdentityTokenFromBootstrapTokenUrl
        {
            get => (Uri)this["identityTokenFromBootstrapTokenUrl"];
            set => this["identityTokenFromBootstrapTokenUrl"] = value;
        }


        [ConfigurationProperty("serviceTokenUrl")]
        [TypeConverter(typeof(UriTypeConverter))]
        public Uri ServiceTokenUrl
        {
            get => (Uri)this["serviceTokenUrl"];
            set => this["serviceTokenUrl"] = value;
        }



        [ConfigurationProperty("stsCertificate")]
        public X509CertificateConfigurationElement StsCertificate
        {
            get => (X509CertificateConfigurationElement)this["stsCertificate"];
            set => this["stsCertificate"] = value;
        }


        [ConfigurationProperty("tokenLifeTime")]
        [TypeConverter(typeof(InfiniteTimeSpanConverter))]
        public TimeSpan? TokenLifeTime
        {
            get => (TimeSpan?)this["tokenLifeTime"];
            set => this["tokenLifeTime"] = value;
        }


        [ConfigurationProperty("sendTimeout")]
        [TypeConverter(typeof(InfiniteTimeSpanConverter))]
        public TimeSpan? SendTimeout
        {
            get => (TimeSpan?)this["sendTimeout"];
            set => this["sendTimeout"] = value;
        }


        [ConfigurationProperty("wscIdentifier")]
        public string WscIdentifier
        {
            get => (string)this["wscIdentifier"];
            set => this["wscIdentifier"] = value;
        }


        [ConfigurationProperty("wscCertificate")]
        public X509CertificateConfigurationElement WscCertificate
        {
            get => (X509CertificateConfigurationElement)this["wscCertificate"];
            set => this["wscCertificate"] = value;
        }


        [ConfigurationProperty("cacheClockSkew")]
        public TimeSpan CacheClockSkew
        {
            get => (TimeSpan)this["cacheClockSkew"];
            set => this["cacheClockSkew"] = value;
        }


        X509Certificate2 ISecurityTokenServiceClientConfiguration.StsCertificate => StsCertificate.GetCertificate();

        X509Certificate2 ISecurityTokenServiceClientConfiguration.WscCertificate => WscCertificate.GetCertificate();


        public static ISecurityTokenServiceClientConfiguration FromConfiguration()
        {
            return (ISecurityTokenServiceClientConfiguration) ConfigurationManager.GetSection("stsConfiguration");
        }
    }
}