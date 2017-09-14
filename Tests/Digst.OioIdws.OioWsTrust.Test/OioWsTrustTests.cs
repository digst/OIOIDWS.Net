using System;
using System.IO;
using System.ServiceModel;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Digst.OioIdws.Test.Common;
using Digst.OioIdws.Wsc.OioWsTrust;
using Fiddler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.OioWsTrust.Test
{
    [TestClass]
    public class OioWsTrustTests
    {
        private SessionStateHandler _fiddlerApplicationOnBeforeRequest;
        private SessionStateHandler _fiddlerApplicationOnBeforeResponse;
        private const string TimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
        private const string StsHostName = "securetokenservice.test-nemlog-in.dk";
        
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            // Check certificates
            if (!CertMaker.rootCertIsTrusted())
                CertMaker.trustRootCert();

            // Start proxy server (to simulate man in the middle attacks)
            FiddlerApplication.Startup(8877, true, true, false);
        }

        [ClassCleanup]
        public static void TearDown()
        {
            // Shut down proxy server
            FiddlerApplication.Shutdown();
        }

        [TestCleanup]
        public void CleanupAfterEachTest()
        {
            // Unregister event handlers after each test so tests do not interfere with each other.
            FiddlerApplication.BeforeRequest -= _fiddlerApplicationOnBeforeRequest;
            FiddlerApplication.BeforeResponse -= _fiddlerApplicationOnBeforeResponse;
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void TotalFlowSucessTest()
        {
            // Arrange
            IStsTokenService stsTokenService = new StsTokenService(TokenServiceConfigurationFactory.CreateConfiguration());

            // Act
            var securityToken = stsTokenService.GetToken();

            // Assert
            Assert.IsNotNull(securityToken);
        }

        #region Request tests

       [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void OioWsTrustRequestFailDueToBodyTamperingTest()
        {
            // Arrange
            IStsTokenService stsTokenService = new StsTokenService(TokenServiceConfigurationFactory.CreateConfiguration());

            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                oS.utilReplaceInRequest("<trust:Lifetime>", "<trust:Lifetime testAttribute=\"Tampered\">");
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            // Act
            try
            {
                stsTokenService.GetToken();
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (FaultException fe)
            {
                // Assert
                Assert.AreEqual("Authentication failed", fe.Message);
            }
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void OioWsTrustRequestFailDueToHeaderActionTamperingTest()
        {
            // Arrange
            IStsTokenService stsTokenService = new StsTokenService(TokenServiceConfigurationFactory.CreateConfiguration());

            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                oS.utilReplaceInRequest("<a:Action", "<a:Action testAttribute=\"Tampered\"");
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            // Act
            try
            {
                stsTokenService.GetToken();
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (FaultException fe)
            {
                // Assert
                Assert.AreEqual("Authentication failed", fe.Message);
            }
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void OioWsTrustRequestFailDueToHeaderMessageIdTamperingTest()
        {
            // Arrange
            IStsTokenService stsTokenService = new StsTokenService(TokenServiceConfigurationFactory.CreateConfiguration());

            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                // Use xml version instead of utilReplaceInRequest(...) because message id is dynamically
                // For some reason there are two calls where the first call has en empty body.
                if (oS.RequestBody.Length > 0)
                {
                    var bodyAsString = Encoding.UTF8.GetString(oS.RequestBody);
                    var bodyAsXml = XDocument.Load(new StringReader(bodyAsString));
                    var namespaceManager = new XmlNamespaceManager(new NameTable());
                    namespaceManager.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
                    namespaceManager.AddNamespace("a", "http://www.w3.org/2005/08/addressing");
                    var messageIdElement = bodyAsXml.XPathSelectElement("/s:Envelope/s:Header/a:MessageID",
                        namespaceManager);
                    messageIdElement.Value = "uuid:0e07468e-42b2-4813-b837-6c2c6122a9c9";
                    oS.RequestBody = Encoding.UTF8.GetBytes(bodyAsXml.ToString(SaveOptions.DisableFormatting));
                }
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            // Act
            try
            {
                stsTokenService.GetToken();
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (FaultException fe)
            {
                // Assert
                Assert.AreEqual("Authentication failed", fe.Message);
            }
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void OioWsTrustRequestFailDueToHeaderToTamperingTest()
        {
            // Arrange
            IStsTokenService stsTokenService = new StsTokenService(TokenServiceConfigurationFactory.CreateConfiguration());

            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                 oS.utilReplaceInRequest("<a:To", "<a:To testAttribute=\"Tampered\"");
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            // Act
            try
            {
                stsTokenService.GetToken();
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (FaultException fe)
            {
                // Assert
                Assert.AreEqual("Authentication failed", fe.Message);
            }
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void OioWsTrustRequestFailDueToHeaderSecurityTamperingTest()
        {
            // Arrange
            IStsTokenService stsTokenService = new StsTokenService(TokenServiceConfigurationFactory.CreateConfiguration());

            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                // Use xml version instead of utilReplaceInRequest(...) because message id is dynamically
                // For some reason there are two calls where the first call has en empty body.
                if (oS.RequestBody.Length > 0)
                {
                    var bodyAsString = Encoding.UTF8.GetString(oS.RequestBody);
                    var bodyAsXml = XDocument.Load(new StringReader(bodyAsString));
                    var namespaceManager = new XmlNamespaceManager(new NameTable());
                    namespaceManager.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
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
                }
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            // Act
            try
            {
                stsTokenService.GetToken();
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (FaultException fe)
            {
                // Assert
                Assert.AreEqual("Authentication failed", fe.Message);
            }
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void OioWsTrustRequestFailDueToTokenTamperingTest()
        {
            // Arrange
            IStsTokenService stsTokenService = new StsTokenService(TokenServiceConfigurationFactory.CreateConfiguration());

            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                // Use xml version instead of utilReplaceInRequest(...) because message id is dynamically
                // For some reason there are two calls where the first call has en empty body.
                if (oS.RequestBody.Length > 0)
                {
                    var bodyAsString = Encoding.UTF8.GetString(oS.RequestBody);
                    var bodyAsXml = XDocument.Load(new StringReader(bodyAsString));
                    var namespaceManager = new XmlNamespaceManager(new NameTable());
                    namespaceManager.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
                    namespaceManager.AddNamespace("o",
                        "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
                    var binarySecurityTokenElement =
                        bodyAsXml.XPathSelectElement(
                            "/s:Envelope/s:Header/o:Security/o:BinarySecurityToken",
                            namespaceManager);
                    // Følgende er en gammel udgave af Morten Mortensen MOCES certifikatet. Det får STS til at svare med "The request was invalid or malformed"
                    //binarySecurityTokenElement.Value =
                    //    "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";
                    // Følgende er den nye udgave af Morten Mortensen MOCES certifikatet. Det får STS til at svare korrekt med "The request was invalid or malformed".
                    binarySecurityTokenElement.Value =
                        "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";
                    oS.RequestBody = Encoding.UTF8.GetBytes(bodyAsXml.ToString(SaveOptions.DisableFormatting));
                }
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            // Act
            try
            {
                stsTokenService.GetToken();
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (FaultException fe)
            {
                // Assert
                Assert.AreEqual("Authentication failed", fe.Message);
            }
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void OioWsTrustRequestFailDueToReplayAttackTest()
        {
            // Arrange
            IStsTokenService stsTokenService = new StsTokenService(TokenServiceConfigurationFactory.CreateConfiguration());

            byte[] recordedRequest = null;
            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                // For some reason there are two calls where the first call has en empty body.
                if (oS.RequestBody.Length > 0)
                {
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
                }
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            stsTokenService.GetToken();

            // Act
            try
            {
                stsTokenService.GetToken();
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (FaultException fe)
            {
                // Assert
                Assert.AreEqual("The specified request failed", fe.Message);
            }
        }

        #endregion


        #region Response tests

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void OioWsTrustResponseFailDueToBodyTamperingTest()
        {
            // Arrange
            IStsTokenService stsTokenService = new StsTokenService(TokenServiceConfigurationFactory.CreateConfiguration());

            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                // In order to enable response tampering, buffering mode must
                // be enabled; this allows FiddlerCore to permit modification of
                // the response in the BeforeResponse handler rather than streaming
                // the response to the client as the response comes in.
                oS.bBufferResponse = true;
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;


            _fiddlerApplicationOnBeforeResponse = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                oS.utilReplaceInResponse("<EncryptedAssertion", "<EncryptedAssertion testAttribute=\"Tampered\"");
            };
            FiddlerApplication.BeforeResponse += _fiddlerApplicationOnBeforeResponse;

            // Act
            try
            {
                stsTokenService.GetToken();
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (InvalidOperationException ioe)
            {
                // Assert
                Assert.AreEqual("SOAP signature recieved from STS does not validate!", ioe.Message);
            }
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void OioWsTrustResponseFailDueToHeaderMessageIdTamperingTest()
        {
            // Arrange
            IStsTokenService stsTokenService = new StsTokenService(TokenServiceConfigurationFactory.CreateConfiguration());

            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                // In order to enable response tampering, buffering mode must
                // be enabled; this allows FiddlerCore to permit modification of
                // the response in the BeforeResponse handler rather than streaming
                // the response to the client as the response comes in.
                oS.bBufferResponse = true;
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;


            _fiddlerApplicationOnBeforeResponse = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                // Use xml version instead of utilReplaceInRequest(...) because message id is dynamically
                // For some reason there are two calls where the first call has en empty body.
                if (oS.RequestBody.Length > 0)
                {
                    var bodyAsString = Encoding.UTF8.GetString(oS.ResponseBody);
                    var bodyAsXml = XDocument.Load(new StringReader(bodyAsString));
                    var namespaceManager = new XmlNamespaceManager(new NameTable());
                    namespaceManager.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
                    namespaceManager.AddNamespace("a", "http://www.w3.org/2005/08/addressing");
                    var messageIdElement = bodyAsXml.XPathSelectElement("/s:Envelope/s:Header/a:MessageID",
                        namespaceManager);
                    messageIdElement.Value = "urn:uuid:0e07468e-42b2-4813-b837-6c2c6122a9c9";
                    oS.ResponseBody = Encoding.UTF8.GetBytes(bodyAsXml.ToString(SaveOptions.DisableFormatting));
                }
            };
            FiddlerApplication.BeforeResponse += _fiddlerApplicationOnBeforeResponse;

            // Act
            try
            {
                stsTokenService.GetToken();
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (InvalidOperationException ioe)
            {
                // Assert
                Assert.AreEqual("SOAP signature recieved from STS does not validate!", ioe.Message);
            }
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void OioWsTrustResponseFailDueToHeaderRelatesToTamperingTest()
        {
            // Arrange
            IStsTokenService stsTokenService = new StsTokenService(TokenServiceConfigurationFactory.CreateConfiguration());

            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                // In order to enable response tampering, buffering mode must
                // be enabled; this allows FiddlerCore to permit modification of
                // the response in the BeforeResponse handler rather than streaming
                // the response to the client as the response comes in.
                oS.bBufferResponse = true;
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;


            _fiddlerApplicationOnBeforeResponse = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                // Use xml version instead of utilReplaceInRequest(...) because message id is dynamically
                // For some reason there are two calls where the first call has en empty body.
                if (oS.RequestBody.Length > 0)
                {
                    var bodyAsString = Encoding.UTF8.GetString(oS.ResponseBody);
                    var bodyAsXml = XDocument.Load(new StringReader(bodyAsString));
                    var namespaceManager = new XmlNamespaceManager(new NameTable());
                    namespaceManager.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
                    namespaceManager.AddNamespace("a", "http://www.w3.org/2005/08/addressing");
                    var relatesToIdElement = bodyAsXml.XPathSelectElement("/s:Envelope/s:Header/a:RelatesTo",
                        namespaceManager);
                    relatesToIdElement.Value = "urn:uuid:0e07468e-42b2-4813-b837-6c2c6122a9c9";
                    oS.ResponseBody = Encoding.UTF8.GetBytes(bodyAsXml.ToString(SaveOptions.DisableFormatting));
                }
            };
            FiddlerApplication.BeforeResponse += _fiddlerApplicationOnBeforeResponse;

            // Act
            try
            {
                stsTokenService.GetToken();
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (InvalidOperationException ioe)
            {
                // Assert
                Assert.AreEqual("SOAP signature recieved from STS does not validate!", ioe.Message);
            }
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void OioWsTrustResponseFailDueToHeaderActionTamperingTest()
        {
            // Arrange
            IStsTokenService stsTokenService = new StsTokenService(TokenServiceConfigurationFactory.CreateConfiguration());

            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                // In order to enable response tampering, buffering mode must
                // be enabled; this allows FiddlerCore to permit modification of
                // the response in the BeforeResponse handler rather than streaming
                // the response to the client as the response comes in.
                oS.bBufferResponse = true;
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;


            _fiddlerApplicationOnBeforeResponse = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                oS.utilReplaceInResponse("<wsa:Action", "<wsa:Action testAttribute=\"Tampered\"");
            };
            FiddlerApplication.BeforeResponse += _fiddlerApplicationOnBeforeResponse;

            // Act
            try
            {
                stsTokenService.GetToken();
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (InvalidOperationException ioe)
            {
                // Assert
                Assert.AreEqual("SOAP signature recieved from STS does not validate!", ioe.Message);
            }
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void OioWsTrustResponseFailDueToHeaderSecurityTamperingTest()
        {
            // Arrange
            IStsTokenService stsTokenService = new StsTokenService(TokenServiceConfigurationFactory.CreateConfiguration());

            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                // In order to enable response tampering, buffering mode must
                // be enabled; this allows FiddlerCore to permit modification of
                // the response in the BeforeResponse handler rather than streaming
                // the response to the client as the response comes in.
                oS.bBufferResponse = true;
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;


            _fiddlerApplicationOnBeforeResponse = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                // Use xml version instead of utilReplaceInRequest(...) because message id is dynamically
                // For some reason there are two calls where the first call has en empty body.
                if (oS.RequestBody.Length > 0)
                {
                    var bodyAsString = Encoding.UTF8.GetString(oS.ResponseBody);
                    var bodyAsXml = XDocument.Load(new StringReader(bodyAsString));
                    var namespaceManager = new XmlNamespaceManager(new NameTable());
                    namespaceManager.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
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
                    oS.ResponseBody = Encoding.UTF8.GetBytes(bodyAsXml.ToString(SaveOptions.DisableFormatting));
                }
            };
            FiddlerApplication.BeforeResponse += _fiddlerApplicationOnBeforeResponse;

            // Act
            try
            {
                stsTokenService.GetToken();
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (InvalidOperationException ioe)
            {
                // Assert
                Assert.AreEqual("SOAP signature recieved from STS does not validate!", ioe.Message);
            }
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void OioWsTrustResponseFailDueToReplayAttackTest()
        {
            // Arrange
            IStsTokenService stsTokenService = new StsTokenService(TokenServiceConfigurationFactory.CreateConfiguration());

            byte[] recordedResponse = null;
            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                // In order to enable response tampering, buffering mode must
                // be enabled; this allows FiddlerCore to permit modification of
                // the response in the BeforeResponse handler rather than streaming
                // the response to the client as the response comes in.
                oS.bBufferResponse = true;
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            _fiddlerApplicationOnBeforeResponse = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                // For some reason there are two calls where the first call has en empty body.
                if (oS.RequestBody.Length > 0)
                {
                    if (recordedResponse == null)
                    {
                        // record request
                        recordedResponse = oS.ResponseBody;
                    }
                    else
                    {
                        // Replay
                        oS.ResponseBody = recordedResponse;
                    }
                }
            };
            FiddlerApplication.BeforeResponse += _fiddlerApplicationOnBeforeResponse;

            stsTokenService.GetToken();
            
            // Act
            try
            {
                stsTokenService.GetToken();
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (InvalidOperationException ioe)
            {
                // Assert
                Assert.IsTrue(ioe.Message.StartsWith("Replay attack detected. Response message id:"), "Replay attack not detected!");
            }
        }

        #endregion

        #region IStsTokenService tests

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void OioWsTrustTokenServiceGivesDifferentTokensTest()
        {
            // Arrange
            IStsTokenService stsTokenService = new StsTokenService(TokenServiceConfigurationFactory.CreateConfiguration());
            var securityToken = stsTokenService.GetToken();

            // Act
            var securityToken2 = stsTokenService.GetToken();

            // Assert
            Assert.AreNotEqual(securityToken, securityToken2, "Expected that tokens was NOT the same");
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void OioWsTrustTokenServiceCacheGivesTheSameTokenTest()
        {
            // Arrange
            IStsTokenService stsTokenService = new StsTokenServiceCache(TokenServiceConfigurationFactory.CreateConfiguration());
            var securityToken = stsTokenService.GetToken();

            // Act
            var securityToken2 = stsTokenService.GetToken();

            // Assert
            Assert.AreEqual(securityToken, securityToken2, "Expected that tokens was the same");
        }

        #endregion
    }
}
