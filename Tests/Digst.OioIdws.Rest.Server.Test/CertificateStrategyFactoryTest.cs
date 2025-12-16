using System;
using System.Configuration;
using Digst.OioIdws.Common.Utils;
using Digst.OioIdws.Rest.Server.AuthorizationServer;
using Digst.OioIdws.Rest.Server.AuthorizationServer.CertificateRetrieval;
using Microsoft.Owin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Digst.OioIdws.Rest.Server.Test
{
    [TestClass]
    [TestCategory(Constants.UnitTest)]
    public class CertificateStrategyFactoryTest
    {
        [TestMethod]
        public void Factory_CreatesHttpHeaderStrategy_FromSection()
        {
            LoadConfigurationSection(new CertificateStrategySection
                { StrategyType = CertificateStrategyType.HttpHeader, Value = "Client-Cert" });
            
            var strategy = CertificateStrategyFactory.Create();

            Assert.IsNotNull(strategy);
            Assert.IsInstanceOfType(strategy, typeof(Rfc9440HttpHeaderCertificateStrategy));
        }
        
        [TestMethod]
        public void Factory_CreatesHttpHeaderStrategy_FromSectionWithoutValue()
        {
            LoadConfigurationSection(new CertificateStrategySection
                { StrategyType = CertificateStrategyType.HttpHeader });
            
            var strategy = CertificateStrategyFactory.Create();

            Assert.IsNotNull(strategy);
            Assert.IsInstanceOfType(strategy, typeof(Rfc9440HttpHeaderCertificateStrategy));
        }

        [TestMethod]
        public void Factory_CreatesEndpointStrategy_FromSection()
        {
            LoadConfigurationSection(new CertificateStrategySection
                { StrategyType = CertificateStrategyType.Connection});

            var strategy = CertificateStrategyFactory.Create();

            Assert.IsNotNull(strategy);
            Assert.IsInstanceOfType(strategy, typeof(EndpointCertificateStrategy));
        }

        [TestMethod]
        public void Factory_CreatesEndpointStrategy_ByDefault()
        {
            var strategy = CertificateStrategyFactory.Create();

            Assert.IsNotNull(strategy);
            Assert.IsInstanceOfType(strategy, typeof(EndpointCertificateStrategy));
        }
        
        private void LoadConfigurationSection(ConfigurationSection strategyConfig)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.Sections["certificateStrategy"] != null)
                config.Sections.Remove("certificateStrategy");

            config.Sections.Add("certificateStrategy", strategyConfig);
            config.Save(ConfigurationSaveMode.Modified, true);
            ConfigurationManager.RefreshSection("certificateStrategy");
        }

        [TestCleanup]
        public void Clean()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.Sections["certificateStrategy"] != null)
                config.Sections.Remove("certificateStrategy");

            config.Save(ConfigurationSaveMode.Modified, true);
            ConfigurationManager.RefreshSection("certificateStrategy");
        }
    }
}