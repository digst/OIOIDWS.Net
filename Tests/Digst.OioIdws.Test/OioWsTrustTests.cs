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

namespace Digst.Oioidws.Test
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
            ITokenService tokenService = new TokenService();

            // Act
            var securityToken = tokenService.GetToken();

            // Assert
            Assert.IsNotNull(securityToken);
        }

        #region Request tests

       [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void OioWsTrustRequestFailDueToBodyTamperingTest()
        {
            // Arrange
            ITokenService tokenService = new TokenService();

            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                oS.utilReplaceInRequest("<wst:Lifetime>", "<wst:Lifetime testAttribute=\"Tampered\">");
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            // Act
            try
            {
                tokenService.GetToken();
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
            ITokenService tokenService = new TokenService();

            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                oS.utilReplaceInRequest("<wsa:Action", "<wsa:Action testAttribute=\"Tampered\"");
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            // Act
            try
            {
                tokenService.GetToken();
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
            ITokenService tokenService = new TokenService();

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
                tokenService.GetToken();
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
            ITokenService tokenService = new TokenService();

            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                oS.utilReplaceInRequest("<wsa:To", "<wsa:To testAttribute=\"Tampered\"");
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            // Act
            try
            {
                tokenService.GetToken();
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
            ITokenService tokenService = new TokenService();

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
                tokenService.GetToken();
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
            ITokenService tokenService = new TokenService();

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
                    binarySecurityTokenElement.Value =
                        "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";
                    oS.RequestBody = Encoding.UTF8.GetBytes(bodyAsXml.ToString(SaveOptions.DisableFormatting));
                }
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            // Act
            try
            {
                tokenService.GetToken();
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
            ITokenService tokenService = new TokenService();

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

            tokenService.GetToken();

            // Act
            try
            {
                tokenService.GetToken();
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
            ITokenService tokenService = new TokenService();

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
                tokenService.GetToken();
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
            ITokenService tokenService = new TokenService();

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
                tokenService.GetToken();
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
            ITokenService tokenService = new TokenService();

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
                tokenService.GetToken();
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
            ITokenService tokenService = new TokenService();

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
                tokenService.GetToken();
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
            ITokenService tokenService = new TokenService();

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
                tokenService.GetToken();
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
            ITokenService tokenService = new TokenService();

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

            tokenService.GetToken();
            
            // Act
            try
            {
                tokenService.GetToken();
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (InvalidOperationException ioe)
            {
                // Assert
                Assert.IsTrue(ioe.Message.StartsWith("Replay attack detected. Response message id:"), "Replay attack not detected!");
            }
        }

        #endregion

    }
}
