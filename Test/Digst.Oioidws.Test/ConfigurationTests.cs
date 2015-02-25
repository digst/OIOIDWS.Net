using System;
using System.Security.Cryptography.X509Certificates;
using Digst.OioIdws;
using Digst.OioIdws.Configurations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.Oioidws.Test
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void ConfigMissingInConfigurationTest()
        {
            // Arrange
            ITokenService tokenService = new TokenService();
            
            // Act
            try
            {
                tokenService.GetToken(null);
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
            ITokenService tokenService = new TokenService();
            var oioIdwsConfiguration = new OioIdwsConfiguration();
            oioIdwsConfiguration.ClientCertificate = new Certificate
            {
                StoreLocation = StoreLocation.CurrentUser,
                StoreName = StoreName.My,
                X509FindType = X509FindType.FindByThumbprint,
                FindValue = "ce3b36692d8d5b731dd1157849a31f1599e524da"
            };
            oioIdwsConfiguration.StsCertificate = new Certificate
            {
                StoreLocation = StoreLocation.LocalMachine,
                StoreName = StoreName.My,
                X509FindType = X509FindType.FindByThumbprint,
                FindValue = "2e7a061560fa2c5e141a634dc1767dacaeec8d12"
            };
            oioIdwsConfiguration.StsEndpointAddress =
                "https://SecureTokenService.test-nemlog-in.dk/SecurityTokenService.svc";

            // Act
            try
            {
                tokenService.GetToken(oioIdwsConfiguration);
                Assert.Fail("Should fail due to wrong configuration");
            }
            // Assert
            catch (ArgumentException e)
            {
                Assert.AreEqual("WspEndpointID", e.Message);
            }
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void StsEndpointAddressMissingInConfigurationTest()
        {
            // Arrange
            ITokenService tokenService = new TokenService();
            var oioIdwsConfiguration = new OioIdwsConfiguration();
            oioIdwsConfiguration.ClientCertificate = new Certificate
            {
                StoreLocation = StoreLocation.CurrentUser,
                StoreName = StoreName.My,
                X509FindType = X509FindType.FindByThumbprint,
                FindValue = "ce3b36692d8d5b731dd1157849a31f1599e524da"
            }; oioIdwsConfiguration.StsCertificate = new Certificate
            {
                StoreLocation = StoreLocation.LocalMachine,
                StoreName = StoreName.My,
                X509FindType = X509FindType.FindByThumbprint,
                FindValue = "2e7a061560fa2c5e141a634dc1767dacaeec8d12"
            };
            oioIdwsConfiguration.WspEndpointID = "https://saml.nnit001.dmz.inttest";

            // Act
            try
            {
                tokenService.GetToken(oioIdwsConfiguration);
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
            ITokenService tokenService = new TokenService();
            var oioIdwsConfiguration = new OioIdwsConfiguration();
            oioIdwsConfiguration.ClientCertificate = new Certificate
            {
                StoreLocation = StoreLocation.CurrentUser,
                StoreName = StoreName.My,
                X509FindType = X509FindType.FindByThumbprint,
                FindValue = "ce3b36692d8d5b731dd1157849a31f1599e524da"
            }; oioIdwsConfiguration.StsEndpointAddress =
                "https://SecureTokenService.test-nemlog-in.dk/SecurityTokenService.svc";
            oioIdwsConfiguration.WspEndpointID = "https://saml.nnit001.dmz.inttest";

            // Act
            try
            {
                tokenService.GetToken(oioIdwsConfiguration);
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
            ITokenService tokenService = new TokenService();
            var oioIdwsConfiguration = new OioIdwsConfiguration();
            oioIdwsConfiguration.StsCertificate = new Certificate
            {
                StoreLocation = StoreLocation.LocalMachine,
                StoreName = StoreName.My,
                X509FindType = X509FindType.FindByThumbprint,
                FindValue = "2e7a061560fa2c5e141a634dc1767dacaeec8d12"
            };
            oioIdwsConfiguration.StsEndpointAddress =
                "https://SecureTokenService.test-nemlog-in.dk/SecurityTokenService.svc";
            oioIdwsConfiguration.WspEndpointID = "https://saml.nnit001.dmz.inttest";

            // Act
            try
            {
                tokenService.GetToken(oioIdwsConfiguration);
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
