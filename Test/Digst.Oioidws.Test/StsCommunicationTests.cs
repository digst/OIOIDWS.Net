using System;
using System.Security.Cryptography.X509Certificates;
using Digst.OioIdws;
using Digst.OioIdws.Configurations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.Oioidws.Test
{
    [TestClass]
    public class StsCommunicationTests
    {
        /// <summary>
        /// This integration test verifies that the implementation is working according to the [NEMLOGIN-STSRULES] specification and that life time of token is as expected.
        /// It is assumed that if the STS in NemLog-in test integration environment response succesfully ... then the implementation is according to the [NEMLOGIN-STSRULES] specification.
        /// Prerequisites:
        /// MOCES client certificate https://test-nemlog-in.dk/Testportal/certifikater/%C3%98S%20-%20Morten%20Mortensen%20RID%2093947552.html must be installed in user store (it automatically is)
        /// FOCES STS certificate https://test-nemlog-in.dk/Testportal/certifikater/IntegrationTestSigning.zip must be installed in local store.
        /// </summary>
        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void GetToken1HourLifeTimeTest()
        {
            // Arrange
            ITokenService tokenService = new TokenService();
            var oioIdwsConfiguration = new OioIdwsConfiguration();
            var clientCertificate = new Certificate
            {
                StoreLocation = StoreLocation.CurrentUser,
                StoreName = StoreName.My,
                X509FindType = X509FindType.FindByThumbprint,
                FindValue = "ce3b36692d8d5b731dd1157849a31f1599e524da"
            };
            oioIdwsConfiguration.ClientCertificate = clientCertificate;
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
            oioIdwsConfiguration.TokenLifeTimeInMinutes = 60;

            // Act
            var securityToken = tokenService.GetToken(oioIdwsConfiguration);

            // Assert
            Assert.IsNotNull(securityToken);
            // 30 seconds withdrawn in order to allow some time sync issues.
            Assert.IsTrue(securityToken.ValidTo > DateTime.UtcNow.AddHours(1).AddSeconds(-30), "Life time of token was not one hour!");
        }
    }
}
