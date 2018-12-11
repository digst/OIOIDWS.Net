using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Threading;
using Digst.OioIdws.Common.Utils;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.OioWsTrust.TokenCache;
using Digst.OioIdws.Soap.TimeLongRunning.Test.Connected_Services.HelloWorldProxy;
using Digst.OioIdws.Wsc.OioWsTrust;
using Fiddler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.Soap.TimeLongRunning.Test
{

    [TestClass]
    public class SoapTimeLongRunningTests
    {
        private static Process _process;
        private SessionStateHandler _fiddlerApplicationOnBeforeRequest;
        private SessionStateHandler _fiddlerApplicationOnBeforeResponse;
        private static ISecurityTokenServiceClient _stsTokenService;
        private const string WspHostName = "digst.oioidws.wsp";
        private const string WspUri = "https://wsp.oioidws-net.dk";

        // Wait 10 minutes:
        //   5 minutes token time + 
        //   5 minutes clockskew + 
        //   10 seconds extra 
        // to be sure that token is expired
        private const int _wait = 610000;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            ITokenCache tokenCache = new MemoryTokenCache();

            _stsTokenService = new CachedSecurityTokenServiceClient(
                new NemloginSecurityTokenServiceClient(TokenServiceConfigurationFactory.CreateConfiguration()),
                tokenCache,
                tokenCache);

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
            FiddlerApplication.BeforeResponse -= _fiddlerApplicationOnBeforeResponse;

        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTestLongRunning)]
        public void TotalFlowTokenExpiredTest()
        {
            // Arrange
            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetServiceToken("https://wsp.oioidws-net.dk", KeyType.HolderOfKey));

            // Act
            try
            {
                Thread.Sleep(_wait);
                channelWithIssuedToken.HelloSign("Schultz");
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (MessageSecurityException mse)
            {
                // Assert
                var fe = mse.InnerException as FaultException;
                Assert.IsNotNull(fe, "Expected inner fault exception");
                Assert.AreEqual("At least one security token in the message could not be validated.", fe.Message);
            }
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTestLongRunning)]
        public void SoapRequestExpiredTest()
        {
            // Arrange
            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (WspHostName != oS.hostname)
                    return;

                Thread.Sleep(_wait);
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetServiceToken(WspUri, KeyType.HolderOfKey));

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
                Assert.AreEqual("An error occurred when verifying security for the message.", fe.Message);
            }
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTestLongRunning)]
        public void SoapResponseExpiredTest()
        {
            // Arrange
            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (WspHostName != oS.hostname)
                    return;

                // it not set then Thread.Sleep is ignored on the response.
                oS.bBufferResponse = true;
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            _fiddlerApplicationOnBeforeResponse = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (WspHostName != oS.hostname)
                    return;

                Thread.Sleep(_wait);
            };
            FiddlerApplication.BeforeResponse += _fiddlerApplicationOnBeforeResponse;

            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(_stsTokenService.GetServiceToken(WspUri, KeyType.HolderOfKey));

            // Act
            try
            {
                channelWithIssuedToken.HelloSign("Schultz");
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (MessageSecurityException mse)
            {
                // Assert
                Assert.IsTrue(mse.Message.StartsWith("The security timestamp is stale because its expiration time"));
            }
        }
    }
}
