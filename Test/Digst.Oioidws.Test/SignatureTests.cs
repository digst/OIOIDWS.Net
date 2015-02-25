using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using Digst.OioIdws.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.Oioidws.Test
{
    [TestClass]
    public class SignatureTests
    {
        /// <summary>
        /// Tests that the XML signature can be verified.
        /// Prerequisites:
        /// MOCES client certificate https://test-nemlog-in.dk/Testportal/certifikater/%C3%98S%20-%20Morten%20Mortensen%20RID%2093947552.html must be installed in user store (it automatically is)
        /// </summary>
        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void SignatureTest()
        {
            // Arrange
            var rtsSoapMessage = XDocument.Load(@"Resources\RST_Not_Signed.txt");
            var ids = new [] {"action", "msgid", "to", "sec-ts", "sec-binsectoken", "body"};
            var cert = CertificateUtil.GetStsCertificate(StoreName.My, StoreLocation.CurrentUser,
                X509FindType.FindByThumbprint, "CE3B36692D8D5B731DD1157849A31F1599E524DA");
            
            // Act
            var rtsSoapMessageSigned = XmlSignatureUtils.SignDocument(rtsSoapMessage, ids, cert);

            // Assert
            Assert.IsTrue(XmlSignatureUtils.VerifySignature(rtsSoapMessageSigned, cert));
        }
    }
}
