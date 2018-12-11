using System;
using System.ComponentModel;
using System.Configuration;

namespace Digst.OioIdws.WscLocalSignature
{
    public class SecurityTokenServiceClientConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty(nameof(StsIdentifier))]
        public string StsIdentifier
        {
            get => (string)this[nameof(StsIdentifier).ToCamelCase()];
            set => this[nameof(StsIdentifier).ToCamelCase()] = value;
        }

        [ConfigurationProperty(nameof(StsBaseAddress))]
        public Uri StsBaseAddress
        {
            get => (Uri)this[nameof(StsBaseAddress).ToCamelCase()];
            set => this[nameof(StsBaseAddress).ToCamelCase()] = value;
        }


        [ConfigurationProperty(nameof(StsCertificate))]
        public X509CertificateElement StsCertificate
        {
            get => (X509CertificateElement)this[nameof(StsCertificate).ToCamelCase()];
            set => this[nameof(StsCertificate).ToCamelCase()] = value;
        }


        [ConfigurationProperty(nameof(TokenLifeTime))]
        [TypeConverter(typeof(InfiniteTimeSpanConverter))]
        public TimeSpan? TokenLifeTime
        {
            get => (TimeSpan?)this[nameof(TokenLifeTime).ToCamelCase()];
            set => this[nameof(TokenLifeTime).ToCamelCase()] = value;
        }


        [ConfigurationProperty(nameof(SendTimeout))]
        [TypeConverter(typeof(InfiniteTimeSpanConverter))]
        public TimeSpan? SendTimeout
        {
            get => (TimeSpan?)this[nameof(SendTimeout).ToCamelCase()];
            set => this[nameof(SendTimeout).ToCamelCase()] = value;
        }


        [ConfigurationProperty(nameof(WscIdentifier))]
        public string WscIdentifier
        {
            get => (string)this[nameof(WscIdentifier).ToCamelCase()];
            set => this[nameof(WscIdentifier).ToCamelCase()] = value;
        }


        [ConfigurationProperty(nameof(WscCertificate))]
        public X509CertificateElement WscCertificate
        {
            get => (X509CertificateElement)this[nameof(WscCertificate).ToCamelCase()];
            set => this[nameof(WscCertificate).ToCamelCase()] = value;
        }



    }
}