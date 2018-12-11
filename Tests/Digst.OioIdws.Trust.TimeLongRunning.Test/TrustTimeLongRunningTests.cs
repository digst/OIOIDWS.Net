using System.ServiceModel;
using System.ServiceModel.Security;
using System.Threading;
using Digst.OioIdws.Common.Utils;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Wsc.OioWsTrust;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Fiddler;

using System.Net;
using System;
using Digst.OioIdws.OioWsTrust.TokenCache;

namespace Digst.OioIdws.Trust.LongRunning.Test
{
    [TestClass]
    public class TrustTimeLongRunningTests
    {
        private static ISecurityTokenServiceClient _stsTokenService;
        private const string WspHostName = "digst.oioidws.wsp";
        private const string WspUri = "https://wsp.oioidws-net.dk";

        private SessionStateHandler _fiddlerApplicationOnBeforeRequest;
        private SessionStateHandler _fiddlerApplicationOnBeforeResponse;
        private const string StsHostName = "securetokenservice.test-nemlog-in.dk";

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
        [TestCategory(Constants.IntegrationTestLongRunning)]
        public void OioWsTrustRequestExpiredTest()
        {
            // Arrange
            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                Thread.Sleep(_wait);
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            // Act
            try
            {
                _stsTokenService.GetServiceToken(WspUri, KeyType.HolderOfKey);
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
        public void OioWsTrustResponseExpiredTest()
        {
            // Arrange
            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                // it not set then Thread.Sleep is ignored on the response.
                oS.bBufferResponse = true;
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            _fiddlerApplicationOnBeforeResponse = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                Thread.Sleep(_wait);
            };
            FiddlerApplication.BeforeResponse += _fiddlerApplicationOnBeforeResponse;

            // Act
            try
            {
                _stsTokenService.GetServiceToken(WspUri, KeyType.HolderOfKey);
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (MessageSecurityException mse)
            {
                // Assert
                Assert.IsTrue(mse.Message.StartsWith("The security timestamp is stale because its expiration time"));
            }
        }

        #region IStsTokenService tests

        [TestMethod]
        [TestCategory(Constants.IntegrationTestLongRunning)]
        public void OioWsTrustTokenServiceCacheGivesDifferentTokenTest()
        {
            // Arrange
            var stsTokenService =
                new NemloginSecurityTokenServiceClient(TokenServiceConfigurationFactory.CreateConfiguration());

            var securityToken = stsTokenService.GetServiceToken(WspUri, KeyType.HolderOfKey);
            Thread.Sleep(230000); // Sleep 4 minutes - 10 seconds ... 4 minutes due to default clock skew of 1 minut

            // Act 1
            var securityToken2 = stsTokenService.GetServiceToken(WspUri, KeyType.HolderOfKey);

            // Assert 1
            Assert.AreEqual(securityToken, securityToken2, "Expected that tokens was the same");

            // Act 2
            Thread.Sleep(20000); // Sleep 20 seconds more and token should be expired.
            var securityToken3 = stsTokenService.GetServiceToken(WspUri, KeyType.HolderOfKey);

            // Assert 2
            Assert.AreNotEqual(securityToken, securityToken3, "Expected that tokens was Not the same");
        }

        #endregion
    }
}
