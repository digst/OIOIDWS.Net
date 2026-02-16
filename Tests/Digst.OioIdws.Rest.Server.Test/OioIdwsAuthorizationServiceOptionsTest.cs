using System.IdentityModel.Selectors;
using Digst.OioIdws.Common.Utils;
using Digst.OioIdws.Rest.Server.AuthorizationServer;
using Digst.OioIdws.Rest.Server.AuthorizationServer.CertificateRetrieval;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.Rest.Server.Test
{
    [TestClass]
    [TestCategory(Constants.UnitTest)]
    public class OioIdwsAuthorizationServiceOptionsTest
    {
        [TestMethod]
        public void Constructor_Should_SetDefaultValues()
        {
            var testInstance = new OioIdwsAuthorizationServiceOptions();
            
            Assert.AreEqual(3600, testInstance.AccessTokenExpiration.TotalSeconds);
            Assert.AreEqual(X509CertificateValidator.ChainTrust, testInstance.CertificateValidator);
            
            Assert.IsNull(testInstance.CertificateStrategyType);
            Assert.IsNull(testInstance.CertificateStrategyValue);
            Assert.IsInstanceOfType(testInstance.CertificateRetrievalStrategy, typeof(EndpointCertificateStrategy));
        }   
    }
}