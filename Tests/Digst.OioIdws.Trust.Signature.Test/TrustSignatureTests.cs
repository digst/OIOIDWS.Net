using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using Digst.OioIdws.Common.Utils;
using Digst.OioIdws.OioWsTrust.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.Trust.Signature.Test
{
    [TestClass]
    public class TrustSignatureTests
    {
        /// <summary>
        /// Tests that the XML signature can be verified.
        /// </summary>
        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void SignatureTest()
        {
            // Arrange
            var rtsSoapMessageNotSigned = XDocument.Load(@"Resources\RST_Not_Signed.xml");
            var ids = new[] { "action", "msgid", "to", "sec-ts", "sec-binsectoken", "body" };
            var cert = CertificateUtil.GetCertificate("A402BB172929AE0D0ADA62F6864329C35DC29483");

            // Act
            var rtsSoapMessageSigned = XmlSignatureUtils.SignDocument(rtsSoapMessageNotSigned, ids, cert);

            // Assert
            Assert.IsTrue(XmlSignatureUtils.VerifySignature(rtsSoapMessageSigned, cert));
        }

        /// <summary>
        /// Tests that the XML signature can be verified on the resulting request after having been through the complete WCF channel stack. Request has been taken by using Fiddler.
        /// </summary>
        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void SignatureFinalRequestTest()
        {
            // Arrange
            var rtsSoapMessageSigned = XDocument.Load(@"Resources\Request25022015.xml");

            var cert = new X509Certificate2(Convert.FromBase64String("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"));

            // Act
            var verified = XmlSignatureUtils.VerifySignature(rtsSoapMessageSigned, cert);

            // Assert
            Assert.IsTrue(verified);
        }

        /// <summary>
        /// Tests that our <see cref="XmlSignatureUtils.VerifySignature"/> works with the RSTR comming from the STS. Response has been taken by using Fiddler.
        /// </summary>
        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void SignatureResponseTest()
        {
            // Arrange
            var rtsrSoapMessageSigned = XDocument.Load(@"Resources\Response25022015.xml");

            var cert = new X509Certificate2(Convert.FromBase64String("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"));

            // Act
            var verified = XmlSignatureUtils.VerifySignature(rtsrSoapMessageSigned, cert);

            // Assert
            Assert.IsTrue(verified);
        }
    }
}
