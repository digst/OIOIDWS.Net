using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
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
    public class EndpointCertificateStrategyTests
    {
        [TestMethod]
        public async Task RetrieveCertificate_ReturnsEndpointCertificate()
        {
            var cert = new X509Certificate2();
            var owinRequest = new OwinRequest();
            var owinContextMock = new Mock<IOwinContext>();
            owinContextMock.Setup(x => x.Request).Returns(owinRequest);
            var endpointContext = new OioIdwsMatchEndpointContext(owinContextMock.Object, new OioIdwsAuthorizationServiceOptions());

            endpointContext.ClientCertificate = () => cert;

            var strategy = new EndpointCertificateStrategy();
            var result = await strategy.GetCertificate(endpointContext);

            Assert.IsNotNull(result);
            Assert.AreEqual(cert, result);
        }

        [TestMethod]
        public async Task RetrieveCertificate_ReturnsNull_WhenEndpointCertificateIsNull()
        {
            var owinRequest = new OwinRequest();
            var owinContextMock = new Mock<IOwinContext>();
            owinContextMock.Setup(x => x.Request).Returns(owinRequest);
            var endpointContext = new OioIdwsMatchEndpointContext(owinContextMock.Object, new OioIdwsAuthorizationServiceOptions());

            endpointContext.ClientCertificate = () => null;

            var strategy = new EndpointCertificateStrategy();
            var result = await strategy.GetCertificate(endpointContext);

            Assert.IsNull(result);
        }
    }
}