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
            owinContextMock.Setup(x => x.Get<X509Certificate2>("ssl.ClientCertificate")).Returns(cert);
            var endpointContext =
                new OioIdwsMatchEndpointContext(owinContextMock.Object, new OioIdwsAuthorizationServiceOptions())
                    {
                        ClientCertificate = () => cert
                    };

            var strategy = new EndpointCertificateStrategy();
            var result = strategy.GetCertificate(endpointContext.OwinContext);

            Assert.AreEqual(cert, result);
            owinContextMock.Verify(x => x.Get<X509Certificate2>("ssl.ClientCertificate"), Times.Once);
        }

        [TestMethod]
        public void RetrieveCertificate_ReturnsNull_WhenEndpointCertificateIsNull()
        {
            var owinRequest = new OwinRequest();
            var owinContextMock = new Mock<IOwinContext>();
            owinContextMock.Setup(x => x.Request).Returns(owinRequest);
            var endpointContext =
                new OioIdwsMatchEndpointContext(owinContextMock.Object, new OioIdwsAuthorizationServiceOptions());

            endpointContext.ClientCertificate = () => null;

            var strategy = new EndpointCertificateStrategy();
            var result = strategy.GetCertificate(endpointContext.OwinContext);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetCertificate_FromIOwinContext_ReturnsCertificate()
        {
            var cert = new X509Certificate2();
            var context = new OwinContext();
            context.Set("ssl.ClientCertificate", cert);
            var strategy = new EndpointCertificateStrategy();
            var result = strategy.GetCertificate(context);
            Assert.AreSame(cert, result);
        }

        [TestMethod]
        public void GetCertificate_FromIOwinContext_ReturnsNull_WhenMissing()
        {
            var context = new OwinContext();
            var strategy = new EndpointCertificateStrategy();
            var result = strategy.GetCertificate(context);
            Assert.IsNull(result);
        }
    }
}