using System.Configuration;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer.CertificateRetrieval
{
    /// <summary>
    /// Custom configuration section for certificate strategy.
    /// </summary>
    public class CertificateStrategySection : ConfigurationSection
    {
        private const string SectionName = "certificateStrategy";

        /// <summary>
        /// Gets or sets the certificate retrieval strategy.
        /// Must be one of the <see cref="CertificateStrategyType"/> enum values.
        /// </summary>
        [ConfigurationProperty("type", IsRequired = true)]
        public CertificateStrategyType StrategyType{
        
            get { return (CertificateStrategyType)this["type"]; }
            set { this["type"] = value; }
        }

        /// <summary>
        /// Gets or sets the optional value associated with the strategy.
        /// </summary>
        [ConfigurationProperty("value", IsRequired = false)]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }

        /// <summary>
        /// Retrieves the configuration section from config.
        /// Defaults to <see cref="CertificateStrategyType.Connection"/> if not present.
        /// </summary>
        public static CertificateStrategySection GetConfig()
        {
            var certificateStrategySection = (CertificateStrategySection)ConfigurationManager.GetSection(SectionName);
            return certificateStrategySection;
        }
    }
}
