using System;
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
    public class Rfc9440HttpHeaderCertificateStrategyTests
    {
        [TestMethod]
        public void  GetCertificate_ReturnsCertificate_FromHeader()
        {
            var cert = new X509Certificate2(Convert.FromBase64String("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"));
            string base64 = Convert.ToBase64String(cert.Export(X509ContentType.Cert));

            var requestMock = new OwinRequest();
            requestMock.Headers.Append("Client-Cert", base64 );
            var owinContextMock = new Mock<IOwinContext>();
            owinContextMock.SetupGet(x => x.Request).Returns(requestMock);
            
            var contextMock = new OioIdwsMatchEndpointContext(owinContextMock.Object, new OioIdwsAuthorizationServiceOptions());
            
            var strategy = new Rfc9440HttpHeaderCertificateStrategy();
            var result =  strategy.GetCertificate(contextMock.OwinContext);

            Assert.IsNotNull(result);
            Assert.AreEqual(cert.Thumbprint, result.Thumbprint);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetCertificate_InvalidOperationException_WhenHeaderInvalid()
        {
            var owinRequest = new OwinRequest();
            owinRequest.Headers.Append( "Client-Cert", "not-a-base64" );
            var owinContextMock = new Mock<IOwinContext>();
            owinContextMock.Setup(x => x.Request).Returns(owinRequest);
            owinContextMock.Setup(x => x.Get<X509Certificate2>("ssl.ClientCertificate")).Returns(new X509Certificate2(new byte[]{}));
            
            var endpointContext = new OioIdwsMatchEndpointContext(owinContextMock.Object, new OioIdwsAuthorizationServiceOptions());
            var strategy = new Rfc9440HttpHeaderCertificateStrategy();
            strategy.GetCertificate(endpointContext.OwinContext);
        }
        
        [TestMethod]
        public void GetCertificate_UsesOnly_AllowedHeaders()
        {
            var cert = new X509Certificate2(Convert.FromBase64String("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"));
            string base64 = Convert.ToBase64String(cert.Export(X509ContentType.Cert));

            var owinRequest = new OwinRequest();
            owinRequest.Headers.Append( "X-Client-Cert", base64 );
            owinRequest.Headers.Append( "Client-Cert", base64 );
            var owinContextMock = new Mock<IOwinContext>();
            owinContextMock.Setup(x => x.Request).Returns(owinRequest);
            var endpointContext = new OioIdwsMatchEndpointContext(owinContextMock.Object, new OioIdwsAuthorizationServiceOptions());

            var strategy = new Rfc9440HttpHeaderCertificateStrategy();
            var result = strategy.GetCertificate(endpointContext.OwinContext);

            Assert.IsNotNull(result);
            Assert.AreEqual(cert.Thumbprint, result.Thumbprint);
        }
    }
}