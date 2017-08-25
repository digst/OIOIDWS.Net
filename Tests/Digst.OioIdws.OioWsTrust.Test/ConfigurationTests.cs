using System;
using System.Security.Cryptography.X509Certificates;
using Digst.OioIdws.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.OioWsTrust.Test
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void ConfigMissingInConfigurationTest()
        {
            
            // Act
            try
            {
                new TokenServiceCache(null);
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
        public void WspEndpointIDMissingInConfigurationTest()
        {
            // Arrange
            var tokenServiceConfiguration = new TokenServiceConfiguration();
            tokenServiceConfiguration.ClientCertificate = new X509Certificate2();
            tokenServiceConfiguration.StsCertificate = new X509Certificate2();
            tokenServiceConfiguration.StsEndpointAddress =
                "https://SecureTokenService.test-nemlog-in.dk/SecurityTokenService.svc";

            // Act
            try
            {
                new TokenServiceCache(tokenServiceConfiguration);
                Assert.Fail("Should fail due to wrong configuration");
            }
            // Assert
            catch (ArgumentException e)
            {
                Assert.AreEqual("WspEndpointId", e.Message);
            }
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void StsEndpointAddressMissingInConfigurationTest()
        {
            // Arrange
            var tokenServiceConfiguration = new TokenServiceConfiguration();
            tokenServiceConfiguration.ClientCertificate = new X509Certificate2();
            tokenServiceConfiguration.StsCertificate = new X509Certificate2();
            tokenServiceConfiguration.WspEndpointId = "https://saml.nnit001.dmz.inttest";

            // Act
            try
            {
                new TokenServiceCache(tokenServiceConfiguration);
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
            var tokenServiceConfiguration = new TokenServiceConfiguration();
            tokenServiceConfiguration.ClientCertificate = new X509Certificate2();
            tokenServiceConfiguration.StsEndpointAddress =
                "https://SecureTokenService.test-nemlog-in.dk/SecurityTokenService.svc";
            tokenServiceConfiguration.WspEndpointId = "https://saml.nnit001.dmz.inttest";

            // Act
            try
            {
                new TokenServiceCache(tokenServiceConfiguration);
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
            var tokenServiceConfiguration = new TokenServiceConfiguration();
            tokenServiceConfiguration.StsCertificate = new X509Certificate2();
            tokenServiceConfiguration.StsEndpointAddress =
                "https://SecureTokenService.test-nemlog-in.dk/SecurityTokenService.svc";
            tokenServiceConfiguration.WspEndpointId = "https://saml.nnit001.dmz.inttest";

            // Act
            try
            {
                new TokenServiceCache(tokenServiceConfiguration);
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
