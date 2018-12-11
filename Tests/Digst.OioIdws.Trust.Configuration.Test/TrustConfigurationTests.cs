using System;
using System.Security.Cryptography.X509Certificates;
using Digst.OioIdws.Common.Utils;
using Digst.OioIdws.OioWsTrust;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.Trust.Configuration.Test
{
    [TestClass]
    public class TrustConfigurationTests
    {
        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void ConfigMissingInConfigurationTest()
        {

            // Act
            try
            {
                new LocalSecurityTokenServiceClient(null, null);
                Assert.Fail("Should fail due to wrong configuration");
            }
            // Assert
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("Value cannot be null.\r\nParameter name: config", e.Message);
            }
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void WscEndpointIDMissingInConfigurationTest()
        {
            // Arrange
            var tokenServiceConfiguration = new SecurityTokenServiceClientConfiguration();
            tokenServiceConfiguration.WscCertificate = new X509Certificate2();
            tokenServiceConfiguration.StsCertificate = new X509Certificate2();
            tokenServiceConfiguration.ServiceTokenUrl = new Uri("https://sts-idws-xua:8181/service/sts");

            // Act
            try
            {
                new LocalSecurityTokenServiceClient(tokenServiceConfiguration, null);
                Assert.Fail("Should fail due to wrong configuration");
            }
            // Assert
            catch (ArgumentException e)
            {
                Assert.AreEqual("WscIdentifier", e.Message);
            }
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void StsEndpointAddressMissingInConfigurationTest()
        {
            // Arrange
            var tokenServiceConfiguration = new SecurityTokenServiceClientConfiguration();
            tokenServiceConfiguration.WscCertificate = new X509Certificate2();
            tokenServiceConfiguration.StsCertificate = new X509Certificate2();

            // Act
            try
            {
                new LocalSecurityTokenServiceClient(tokenServiceConfiguration, null);
                Assert.Fail("Should fail due to wrong configuration");
            }
            // Assert
            catch (ArgumentException e)
            {
                Assert.AreEqual("StsEndpointAddress", e.Message);
            }
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void StsCertificateMissingInConfigurationTest()
        {
            // Arrange
            var tokenServiceConfiguration = new SecurityTokenServiceClientConfiguration();
            tokenServiceConfiguration.WscIdentifier = "https://digst.oioidws.wsp:9090/helloworld";
            tokenServiceConfiguration.WscCertificate = new X509Certificate2();
            tokenServiceConfiguration.ServiceTokenUrl = new Uri("https://sts-idws-xua:8181/service/sts");

            // Act
            try
            {
                new LocalSecurityTokenServiceClient(tokenServiceConfiguration, null);
                Assert.Fail("Should fail due to wrong configuration");
            }
            // Assert
            catch (ArgumentException e)
            {
                Assert.AreEqual("StsCertificate", e.Message);
            }
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void ClientCertificateMissingInConfigurationTest()
        {
            // Arrange
            var tokenServiceConfiguration = new SecurityTokenServiceClientConfiguration();
            tokenServiceConfiguration.WscIdentifier = "https://digst.oioidws.wsp:9090/helloworld";
            tokenServiceConfiguration.StsCertificate = new X509Certificate2();
            tokenServiceConfiguration.ServiceTokenUrl = new Uri("https://sts-idws-xua:8181/service/sts");

            // Act
            try
            {
                new LocalSecurityTokenServiceClient(tokenServiceConfiguration, null);
                Assert.Fail("Should fail due to wrong configuration");
            }
            // Assert
            catch (ArgumentException e)
            {
                Assert.AreEqual("ClientCertificate", e.Message);
            }
        }
    }
}
