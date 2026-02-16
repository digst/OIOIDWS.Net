using System.IdentityModel.Selectors;
using Digst.OioIdws.Common.Utils;
using Digst.OioIdws.Rest.Server.AuthorizationServer;
using Digst.OioIdws.Rest.Server.AuthorizationServer.CertificateRetrieval;
using Digst.OioIdws.Rest.Server.Wsp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.Rest.Server.Test
{
    [TestClass]
    [TestCategory(Constants.UnitTest)]
    public class OioIdwsAuthenticationServiceOptionsTest
    {
        [TestMethod]
        public void Constructor_Should_SetDefaultValues()
        {
            var testInstance = new OioIdwsAuthenticationOptions();
            
            Assert.IsNull(testInstance.TokenProvider);
            Assert.IsNull(testInstance.CertificateStrategyType);
            Assert.IsNull(testInstance.CertificateStrategyValue);
            Assert.IsInstanceOfType(testInstance.CertificateRetrievalStrategy, typeof(EndpointCertificateStrategy));
        }   
    }
}