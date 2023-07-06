using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Digst.OioIdws.Soap.Basic.Test.HelloWorldProxy;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Common.Utils;
using Digst.OioIdws.Wsc.OioWsTrust;
using Fiddler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.Soap.Test
{

    [TestClass]
    public class SoapTests
    {
        private static Process _process;
        private SessionStateHandler _fiddlerApplicationOnBeforeRequest;
        private static StsTokenServiceCache _stsTokenService;
        private const string TimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
        private const string WspHostName = "digst.oioidws.wsp";

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            // Check certificates
            if (!CertMaker.rootCertIsTrusted())
                CertMaker.trustRootCert();

            // Start proxy server (to simulate man in the middle attacks)
            FiddlerApplication.Startup(
                8877, /* Port */ 
                true, /* Register as System Proxy */
                true, /* Decrypt SSL */
                false /* Allow Remote */
            );

            // Start WSP
            _process = Process.Start(@"..\..\..\..\Examples\Digst.OioIdws.WspExample\bin\Debug\Digst.OioIdws.WspExample.exe");

            // Retrieve token
            _stsTokenService = new StsTokenServiceCache(TokenServiceConfigurationFactory.CreateConfiguration());
        }

        [ClassCleanup]
        public static void TearDown()
        {
            // Shutdown WSP
            _process.Kill();

            // Shut down proxy server
            FiddlerApplication.Shutdown();
        }

        [TestCleanup]
        public void CleanupAfterEachTest()
        {
            // Unregister event handlers after each test so tests do not interfere with each other.
            FiddlerApplication.BeforeRequest -= _fiddlerApplicationOnBeforeRequest;
        }

        [Ignore("Skipped because fiddler don't work")]
        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void TotalFlowNoneSucessTest()
        {
            // Arrange
            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetToken());

            // Act
            var response = channelWithIssuedToken.HelloNone("Schultz");

            // Assert
            Assert.IsTrue(response.StartsWith("Hello"));
        }

        [Ignore("Skipped because fiddler don't work")]
        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void TotalFlowNoneSoapFaultSucessTest()
        {
            // Arrange
            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetToken());

            // Act
            try
            {
                channelWithIssuedToken.HelloNoneError("Schultz");
            }
            catch (Exception e)
            {
                // Assert
                Assert.IsTrue(e.Message.StartsWith("Hello"));
            }
        }

        [Ignore("Skipped because fiddler don't work")]
        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void TotalFlowSignSucessTest()
        {
            // Arrange
            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetToken());

            // Act
            var response = channelWithIssuedToken.HelloSign("Schultz");

            // Assert
            Assert.IsTrue(response.StartsWith("Hello"));
        }

        [Ignore("Skipped because fiddler don't work")]
        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void TotalFlowSignSoapFaultSucessTest()
        {
            // Arrange
            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetToken());

            // Act
            try
            {
                channelWithIssuedToken.HelloSignError("Schultz");
            }
            catch (Exception e)
            {
                // Assert
                Assert.IsTrue(e.Message.StartsWith("Hello"));
            }
        }

        [Ignore("Skipped because fiddler don't work")]
        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void TotalFlowEncryptAndSignSucessTest()
        {
            // Arrange
            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetToken());

            // Act
            var response = channelWithIssuedToken.HelloEncryptAndSign("Schultz");

            // Assert
            Assert.IsTrue(response.StartsWith("Hello"));
        }

        [Ignore("Skipped because fiddler don't work")]
        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void TotalFlowEncryptAndSignSoapFaultSucessTest()
        {
            // Arrange
            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetToken());

            // Act
            try
            {
                channelWithIssuedToken.HelloEncryptAndSignError("Schultz");
            }
            catch (Exception e)
            {
                // Assert
                Assert.IsTrue(e.Message.StartsWith("Hello"));
            }
        }

        #region Request tests

        [Ignore("Skipped because fiddler don't work")]
        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void SoapRequestFailDueToBodyTamperingTest()
        {
            // Arrange
            _fiddlerApplicationOnBeforeRequest = delegate(Session oS)
            {
                // Only act on requests to WSP
                if (WspHostName != oS.hostname)
                    return;

                oS.utilReplaceInRequest("Schultz", "Tampered");
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            // Act
            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetToken());
            try
            {
                channelWithIssuedToken.HelloSign("Schultz");
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (MessageSecurityException mse)
            {
                // Assert
                var fe = mse.InnerException as FaultException;
                Assert.IsNotNull(fe, "Expected inner fault exception");
                Assert.AreEqual("An error occurred when verifying security for the message.", fe.Message);
            }
        }

        [Ignore("Skipped because fiddler don't work")]
        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void SoapRequestFailDueToHeaderMessageIdTamperingTest()
        {
            // Arrange
            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (WspHostName != oS.hostname)
                    return;

                // Use xml version instead of utilReplaceInRequest(...) because message id is dynamically
                var bodyAsString = Encoding.UTF8.GetString(oS.RequestBody);
                var bodyAsXml = XDocument.Load(new StringReader(bodyAsString));
                var namespaceManager = new XmlNamespaceManager(new NameTable());
                namespaceManager.AddNamespace("s", "http://www.w3.org/2003/05/soap-envelope");
                namespaceManager.AddNamespace("a", "http://www.w3.org/2005/08/addressing");
                var messageIdElement = bodyAsXml.XPathSelectElement("/s:Envelope/s:Header/a:MessageID", namespaceManager);
                messageIdElement.Value = "urn:uuid:0e07468e-42b2-4813-b837-6c2c6122a9c9";
                oS.RequestBody = Encoding.UTF8.GetBytes(bodyAsXml.ToString(SaveOptions.DisableFormatting));
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            // Act
            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetToken());
            try
            {
                channelWithIssuedToken.HelloSign("Schultz");
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (MessageSecurityException mse)
            {
                // Assert
                var fe = mse.InnerException as FaultException;
                Assert.IsNotNull(fe, "Expected inner fault exception");
                Assert.AreEqual("An error occurred when verifying security for the message.", fe.Message);
            }
        }

        [Ignore("Skipped because fiddler don't work")]
        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void SoapRequestFailDueToHeaderToTamperingTest()
        {
            // Arrange
            _fiddlerApplicationOnBeforeRequest = delegate(Session oS)
            {
                // Only act on requests to WSP
                if (WspHostName != oS.hostname)
                    return;

                oS.utilReplaceInRequest("https://digst.oioidws.wsp:9090/HelloWorld</a:To>",
                    "https://digst.oioidws.wsp:9090/HelloWorldTampered</a:To>");
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            // Act
            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetToken());
            try
            {
                channelWithIssuedToken.HelloSign("Schultz");
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (MessageSecurityException mse)
            {
                // Assert
                var fe = mse.InnerException as FaultException;
                Assert.IsNotNull(fe, "Expected inner fault exception");
                Assert.AreEqual("An error occurred when verifying security for the message.", fe.Message);
            }
        }

        [Ignore("Skipped because fiddler don't work")]
        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void SoapRequestFailDueToHeaderActionTamperingTest()
        {
            // Arrange
            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (WspHostName != oS.hostname)
                    return;

                oS.utilReplaceInRequest("<a:Action", "<a:Action testAttribute=\"Tampered\"");
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            // Act
            HelloWorldClient client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetToken());
            try
            {
                channelWithIssuedToken.HelloSign("Schultz");
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (MessageSecurityException mse)
            {
                // Assert
                var fe = mse.InnerException as FaultException;
                Assert.IsNotNull(fe, "Expected inner fault exception");
                Assert.AreEqual("An error occurred when verifying security for the message.", fe.Message);
            }
        }

        [Ignore("Skipped because fiddler don't work")]
        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void SoapRequestFailDueToHeaderSecurityTamperingTest()
        {
            // Arrange

            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (WspHostName != oS.hostname)
                    return;

                // Use xml version instead of utilReplaceInRequest(...) because message id is dynamically
                var bodyAsString = Encoding.UTF8.GetString(oS.RequestBody);
                var bodyAsXml = XDocument.Load(new StringReader(bodyAsString));
                var namespaceManager = new XmlNamespaceManager(new NameTable());
                namespaceManager.AddNamespace("s", "http://www.w3.org/2003/05/soap-envelope");
                namespaceManager.AddNamespace("o",
                    "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
                namespaceManager.AddNamespace("u",
                    "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
                var createdTimestampElement =
                    bodyAsXml.XPathSelectElement("/s:Envelope/s:Header/o:Security/u:Timestamp/u:Created",
                        namespaceManager);
                var dateTime = DateTime.Parse(createdTimestampElement.Value);
                var addMinutes = dateTime.AddMinutes(1);
                var longDateString = addMinutes.ToUniversalTime().ToString(TimeFormat);
                createdTimestampElement.Value = longDateString;
                oS.RequestBody = Encoding.UTF8.GetBytes(bodyAsXml.ToString(SaveOptions.DisableFormatting));
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            // Act
            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetToken());
            try
            {
                channelWithIssuedToken.HelloSign("Schultz");
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (MessageSecurityException mse)
            {
                // Assert
                var fe = mse.InnerException as FaultException;
                Assert.IsNotNull(fe, "Expected inner fault exception");
                Assert.IsTrue(fe.Message.StartsWith("An error occurred when verifying security for the message."));
            }
        }

        [Ignore("Skipped because fiddler don't work")]
        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void SoapRequestFailDueToTokenTamperingTest()
        {
            // Arrange

            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (WspHostName != oS.hostname)
                    return;

                // Use xml version instead of utilReplaceInRequest(...) because message id is dynamically
                var bodyAsString = Encoding.UTF8.GetString(oS.RequestBody);
                var bodyAsXml = XDocument.Load(new StringReader(bodyAsString));
                var namespaceManager = new XmlNamespaceManager(new NameTable());
                namespaceManager.AddNamespace("s", "http://www.w3.org/2003/05/soap-envelope");
                namespaceManager.AddNamespace("o",
                    "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
                namespaceManager.AddNamespace("u", "urn:oasis:names:tc:SAML:2.0:assertion");
                namespaceManager.AddNamespace("xenc", "http://www.w3.org/2001/04/xmlenc#");
                var cipherValueElement =
                    bodyAsXml.XPathSelectElement(
                        "/s:Envelope/s:Header/o:Security/u:EncryptedAssertion/xenc:EncryptedData/xenc:CipherData/xenc:CipherValue",
                        namespaceManager);
                cipherValueElement.Value =
                    "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";
                oS.RequestBody = Encoding.UTF8.GetBytes(bodyAsXml.ToString(SaveOptions.DisableFormatting));
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            // Act
            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetToken());
            try
            {
                channelWithIssuedToken.HelloSign("Schultz");
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (MessageSecurityException mse)
            {
                // Assert
                var fe = mse.InnerException as FaultException;
                Assert.IsNotNull(fe, "Expected inner fault exception");
                Assert.IsTrue(fe.Message.StartsWith("An error occurred when verifying security for the message."));
            }
        }

        [Ignore("Skipped because fiddler don't work")]
        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void SoapRequestFailDueToReplayAttackTest()
        {
            // Arrange
            byte[] recordedRequest = null;
            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (WspHostName != oS.hostname)
                    return;
                if (recordedRequest == null)
                {
                    // record request
                    recordedRequest = oS.RequestBody;
                }
                else
                {
                    // Replay
                    oS.RequestBody = recordedRequest;
                }
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetToken());
            channelWithIssuedToken.HelloSign("Schultz");

            // Act
            try
            {
                channelWithIssuedToken.HelloSign("Schultz");
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (MessageSecurityException mse)
            {
                // Assert
                var fe = mse.InnerException as FaultException;
                Assert.IsNotNull(fe, "Expected inner fault exception");
                Assert.IsTrue(fe.Message.StartsWith("An error occurred when verifying security for the message."));
            }
        }

        [Ignore("Skipped because fiddler don't work")]
        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void SoapRequestSucceedIsSoap12Test()
        {
            var soap12 = "http://www.w3.org/2003/05/soap-envelope";

            var isSoap12 = false;

            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (WspHostName != oS.hostname)
                    return;

                // Use xml version instead of utilReplaceInRequest(...)
                // because message id is dynamically
                var bodyAsString = Encoding.UTF8.GetString(oS.RequestBody);
                var bodyAsXml = XDocument.Load(new StringReader(bodyAsString));
                var namespaceManager = new XmlNamespaceManager(new NameTable());
                namespaceManager.AddNamespace("s", soap12);
                // If we can find the root element, it's because the namespace
                // of SOAP 1.2 matches
                var envelopeElement =
                    bodyAsXml.XPathSelectElement(
                        "/s:Envelope",
                        namespaceManager
                    );
                isSoap12 = (null != envelopeElement);
            };
            FiddlerApplication.BeforeRequest +=
                _fiddlerApplicationOnBeforeRequest;

            // Act
            var client = new HelloWorldClient();
            var channelWithIssuedToken = 
                client.ChannelFactory.CreateChannelWithIssuedToken(
                    _stsTokenService.GetToken()
                );
            channelWithIssuedToken.HelloSign("Schultz");
            Assert.IsTrue(isSoap12, "Succeed with a valid SOAP 1.2 header.");
        }

        [Ignore("Skipped because fiddler don't work")]
        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void SoapRequestSucceedNoLibertyHeaderTest()
        {
            var soap12 = "http://www.w3.org/2003/05/soap-envelope";

            var noLibHead = false;

            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (WspHostName != oS.hostname)
                    return;

                // Use xml version instead of utilReplaceInRequest(...)
                // because message id is dynamically
                var bodyAsString = Encoding.UTF8.GetString(oS.RequestBody);
                var bodyAsXml = XDocument.Load(new StringReader(bodyAsString));
                var namespaceManager = new XmlNamespaceManager(new NameTable());
                namespaceManager.AddNamespace("s", soap12);
                // If we can't find the Liberty Header, it's because it's no 
                // longer part of the SOAP 1.2 message
                var frameworkElement =
                    bodyAsXml.XPathSelectElement(
                        "/s:Envelope/s:Header/Framework",
                        namespaceManager
                    );
                noLibHead = (null == frameworkElement);
            };
            FiddlerApplication.BeforeRequest +=
                _fiddlerApplicationOnBeforeRequest;

            // Act
            var client = new HelloWorldClient();
            var channelWithIssuedToken =
                client.ChannelFactory.CreateChannelWithIssuedToken(
                    _stsTokenService.GetToken()
                );
            channelWithIssuedToken.HelloSign("Schultz");
            Assert.IsTrue(noLibHead, "Succeed with no Liberty header.");
        }

        [Ignore("Skipped because fiddler don't work")]
        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void SoapRequestSucceedIsOasisSamlToken11Test()
        {
            var soap12 = "http://www.w3.org/2003/05/soap-envelope";
            var wsse00 = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
            var wsse11 = "http://docs.oasis-open.org/wss/oasis-wss-wssecurity-secext-1.1.xsd";

            var oasisSamlToken11 = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0";

            var isOasisSamlToken11 = false;

            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (WspHostName != oS.hostname)
                    return;

                // Use xml version instead of utilReplaceInRequest(...)
                // because message id is dynamically
                var bodyAsString = Encoding.UTF8.GetString(oS.RequestBody);
                var bodyAsXml = XDocument.Load(new StringReader(bodyAsString));
                var namespaceManager = new XmlNamespaceManager(new NameTable());
                namespaceManager.AddNamespace("s", soap12);
                namespaceManager.AddNamespace("o", wsse00);
                namespaceManager.AddNamespace("b", wsse11);
                // If we can't find the Liberty Header, it's because it's no 
                // longer part of the SOAP 1.2 message
                var tokenElement =
                    bodyAsXml.XPathSelectElement(
                        "/s:Envelope/s:Header/o:Security/o:SecurityTokenReference",
                        namespaceManager
                    );
                if (null != tokenElement)
                {
                    XNamespace lns = wsse11;
                    var tokenType =
                        tokenElement
                            .Attributes(lns + "TokenType")
                            .First();

                    if (null != tokenType)
                    {
                        isOasisSamlToken11 =
                            tokenType.Value.Equals(oasisSamlToken11);
                    }
                }
            };
            FiddlerApplication.BeforeRequest +=
                _fiddlerApplicationOnBeforeRequest;

            // Act
            var client = new HelloWorldClient();
            var channelWithIssuedToken =
                client.ChannelFactory.CreateChannelWithIssuedToken(
                    _stsTokenService.GetToken()
                );
            channelWithIssuedToken.HelloSign("Schultz");
            Assert.IsTrue(
                isOasisSamlToken11, 
                "Succeed with a OASIS SAML Token 1.1 profile."
            );
        }

        #endregion
    }
}
