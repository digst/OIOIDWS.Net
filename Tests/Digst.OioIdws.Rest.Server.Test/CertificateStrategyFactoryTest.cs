using System.Configuration;
using Digst.OioIdws.Common.Utils;
using Digst.OioIdws.Rest.Server.AuthorizationServer;
using Digst.OioIdws.Rest.Server.AuthorizationServer.CertificateRetrieval;
using Digst.OioIdws.Rest.Server.Wsp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                { StrategyType = CertificateStrategyType.Connection });

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

        [TestMethod]
        public void Create_WithExplicitStrategy_ReturnsExplicitStrategy_AuthorizationService()
        {
            var explicitStrategy = new EndpointCertificateStrategy();
            var options = new OioIdwsAuthorizationServiceOptions { CertificateRetrievalStrategy = explicitStrategy };
            var result = CertificateStrategyFactory.Create(options);
            Assert.AreSame(explicitStrategy, result);
        }

        [TestMethod]
        public void Create_WithExplicitStrategy_ReturnsExplicitStrategy_Wsp()
        {
            var explicitStrategy = new EndpointCertificateStrategy();
            var options = new OioIdwsAuthenticationOptions { CertificateRetrievalStrategy = explicitStrategy };
            var result = CertificateStrategyFactory.Create(options);
            Assert.AreSame(explicitStrategy, result);
        }

        [TestMethod]
        public void Create_WithHttpHeaderStrategyType_ReturnsHeaderStrategy_AuthorizationService()
        {
            var options = new OioIdwsAuthorizationServiceOptions
            {
                CertificateStrategyType = CertificateStrategyType.HttpHeader, CertificateStrategyValue = "X-Client-Cert"
            };
            var result = CertificateStrategyFactory.Create(options);
            Assert.IsInstanceOfType(result, typeof(Rfc9440HttpHeaderCertificateStrategy));
        }

        [TestMethod]
        public void Create_WithHttpHeaderStrategyType_ReturnsHeaderStrategy_Wsp()
        {
            var options = new OioIdwsAuthenticationOptions
            {
                CertificateStrategyType = CertificateStrategyType.HttpHeader, CertificateStrategyValue = "X-Client-Cert"
            };
            var result = CertificateStrategyFactory.Create(options);
            Assert.IsInstanceOfType(result, typeof(Rfc9440HttpHeaderCertificateStrategy));
        }

        [TestMethod]
        public void Create_WithConnectionStrategyType_ReturnsEndpointStrategy_AuthorizationService()
        {
            var options = new OioIdwsAuthorizationServiceOptions
                { CertificateStrategyType = CertificateStrategyType.Connection };
            var result = CertificateStrategyFactory.Create(options);
            Assert.IsInstanceOfType(result, typeof(EndpointCertificateStrategy));
        }

        [TestMethod]
        public void Create_WithConnectionStrategyType_ReturnsEndpointStrategy_Wsp()
        {
            var options = new OioIdwsAuthenticationOptions
                { CertificateStrategyType = CertificateStrategyType.Connection };
            var result = CertificateStrategyFactory.Create(options);
            Assert.IsInstanceOfType(result, typeof(EndpointCertificateStrategy));
        }

        [TestMethod]
        public void Create_WithNoConfiguration_ReturnsDefaultEndpointStrategy_AuthorizationService()
        {
            var options = new OioIdwsAuthorizationServiceOptions();
            var result = CertificateStrategyFactory.Create(options);
            Assert.IsInstanceOfType(result, typeof(EndpointCertificateStrategy));
        }

        [TestMethod]
        public void Create_WithNoConfiguration_ReturnsDefaultEndpointStrategy_Wsp()
        {
            var options = new OioIdwsAuthenticationOptions();
            var result = CertificateStrategyFactory.Create(options);
            Assert.IsInstanceOfType(result, typeof(EndpointCertificateStrategy));
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