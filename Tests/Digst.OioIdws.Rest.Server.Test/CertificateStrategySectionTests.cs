using Digst.OioIdws.Common.Utils;
using Digst.OioIdws.Rest.Server.AuthorizationServer.CertificateRetrieval;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.Rest.Server.Test
{
    [TestClass]
    [TestCategory(Constants.UnitTest)]
    public class CertificateStrategySectionTests
    {
        private readonly CertificateStrategyFactoryTest _certificateStrategyFactoryTest =
            new CertificateStrategyFactoryTest();

        [TestMethod]
        public void CertificateStrategySection_Constructor_ReturnsCorrectValues()
        {
            // Arrange
            var section = new CertificateStrategySection();

            // Act & Assert
            Assert.AreEqual(section.StrategyType, CertificateStrategyType.Connection);
            Assert.IsTrue(string.IsNullOrEmpty(section.Value));
        }

        [TestMethod]
        public void CertificateStrategySection_CanBeSetToHttpHeader()
        {
            // Arrange
            var section = new CertificateStrategySection
            {
                StrategyType = CertificateStrategyType.HttpHeader,
                Value = "X-Client-Cert"
            };

            // Act & Assert
            Assert.AreEqual(CertificateStrategyType.HttpHeader, section.StrategyType);
            Assert.AreEqual("X-Client-Cert", section.Value);
        }

        [TestMethod]
        public void CertificateStrategySection_CanBeSetToConnection()
        {
            // Arrange
            var section = new CertificateStrategySection
            {
                StrategyType = CertificateStrategyType.Connection,
                Value = null
            };

            // Act & Assert
            Assert.AreEqual(CertificateStrategyType.Connection, section.StrategyType);
            Assert.IsNull(section.Value);
        }
    }
}