using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fiddler;
using System.Diagnostics;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.OioWsTrust.TokenCache;
using Digst.OioIdws.Common;
using Digst.OioIdws.Common.Utils;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Digst.OioIdws.Soap.Basic.Healthcare.Test.Connected_Services.HelloWorldProxy;

namespace Digst.OioIdws.Soap.Basic.Healthcare.Test
{
    [TestClass]
    public class SoapTestsHealthcare
    {
        private static Process _process;
        private SessionStateHandler _fiddlerApplicationOnBeforeRequest;
        private static ISecurityTokenServiceClient _stsTokenService;
        private const string TimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
        private const string WspHostName = "digst.oioidws.wsp";
        private const string WspUri = "https://digst.oioidws.wsp:9090/helloworld";


        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            if (_process != null) _process.Kill();

            if (FiddlerApplication.IsStarted())
            {
                try
                {
                    FiddlerApplication.oProxy.Detach();
                    FiddlerApplication.oProxy.Dispose();
                }
                finally
                {
                    FiddlerApplication.Shutdown();
                }
            }

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
            _process = Process.Start(@"..\..\..\..\Examples\Digst.OioIdws.WspHealthcareExample\bin\Debug\Digst.OioIdws.WspHealthcareExample.exe");

            // Retrieve token
            ITokenCache memoryCache = new MemoryTokenCache();

            var securityTokenServiceClientConfiguration = new SecurityTokenServiceClientConfiguration()
            {
                WscIdentifier = "https://digst.oioidws.wsp:9090/helloworld",
                ServiceTokenUrl = new Uri("https://sts-idws-xua:8181/service/sts"),
                TokenLifeTime = TimeSpan.FromMinutes(5),
                WscCertificate = CertificateUtil.GetCertificate("0E6DBCC6EFAAFF72E3F3D824E536381B26DEECF5"),
                StsCertificate = CertificateUtil.GetCertificate("af7691346492dc30d127d85537297d702993176c")
            };

            _stsTokenService = new LocalSecurityTokenServiceClient(securityTokenServiceClientConfiguration, null);
        }

        [ClassCleanup]
        public static void TearDown()
        {
            // Shutdown WSP
            _process.Kill();

            // Shut down proxy server
            try
            {
                FiddlerApplication.oProxy.Detach();
                FiddlerApplication.oProxy.Dispose();
            }
            finally
            {
                FiddlerApplication.Shutdown();
            }
        }

        [TestCleanup]
        public void CleanupAfterEachTest()
        {
            FiddlerApplication.BeforeRequest -= _fiddlerApplicationOnBeforeRequest;
        }


        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void TotalFlowNoneSucessTest()
        {
            // Arrange
            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetServiceToken(WspUri, KeyType.HolderOfKey));

            // Act
            var response = channelWithIssuedToken.HelloNone("Schultz");

            // Assert
            Assert.IsTrue(response.StartsWith("Hello"));
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void TotalFlowSignSucessTest()
        {
            // Arrange
            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetServiceToken(WspUri, KeyType.HolderOfKey));

            // Act
            var response = channelWithIssuedToken.HelloSign("Schultz");

            // Assert
            Assert.IsTrue(response.StartsWith("Hello"));
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void SoapRequestFailDueToBodyTamperingTest()
        {
            // Arrange
            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (WspHostName != oS.hostname)
                    return;

                oS.utilReplaceInRequest("Schultz", "Tampered");
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            // Act
            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetServiceToken(WspUri, KeyType.HolderOfKey));
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
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetServiceToken(WspUri, KeyType.HolderOfKey));
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

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void SoapRequestFailDueToHeaderToTamperingTest()
        {
            // Arrange
            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (WspHostName != oS.hostname)
                    return;

                oS.utilReplaceInRequest("https://digst.oioidws.wsp:9090/helloworld</a:To>",
                    "https://digst.oioidws.wsp:9090/helloworldTampered</a:To>");
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            // Act
            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetServiceToken(WspUri, KeyType.HolderOfKey));
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
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetServiceToken(WspUri, KeyType.HolderOfKey));
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
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetServiceToken(WspUri, KeyType.HolderOfKey));
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

        

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void SoapRequestFailDueToReplayAttackTest()
        {
            // Arrange
            //byte[] recordedRequest = null;
            string recordedRequest = null;
            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (WspHostName != oS.hostname)
                    return;

                if (oS.RequestBody.Length > 0)
                {
                    if (recordedRequest == null)
                    {
                        // record request
                        recordedRequest = oS.GetRequestBodyAsString();
                    }
                    else
                    {
                        // Replay
                        oS.utilSetRequestBody(recordedRequest);
                    }
                }
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetServiceToken(WspUri, KeyType.HolderOfKey));
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
    }
}
