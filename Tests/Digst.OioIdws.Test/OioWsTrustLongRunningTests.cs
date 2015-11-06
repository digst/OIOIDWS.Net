using System.ServiceModel;
using System.ServiceModel.Security;
using System.Threading;
using Digst.OioIdws.Wsc.OioWsTrust;
using Fiddler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.Oioidws.Test
{
    [TestClass]
    public class OioWsTrustLongRunningTests
    {
        private SessionStateHandler _fiddlerApplicationOnBeforeRequest;
        private SessionStateHandler _fiddlerApplicationOnBeforeResponse;
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
        public void OioWsTrustRequestExpiredTest()
        {
            // Arrange
            ITokenService tokenService = new TokenService();

            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (StsHostName != oS.hostname)
                    return;

                Thread.Sleep(610000); // Wait 10 minutes seconds. 5 minutes token time + 5 minutes clockscrew + 10 seconds extra to be sure that token is expired
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            // Act
            try
            {
                tokenService.GetToken();
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (MessageSecurityException mse)
            {
                // Assert
                var fe = mse.InnerException as FaultException;
                Assert.IsNotNull(fe, "Expected inner fault exception");
                Assert.AreEqual("At least one security token in the message could not be validated.", mse.Message);
            }
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void OioWsTrustResponseExpiredTest()
        {
            // Arrange
            ITokenService tokenService = new TokenService();

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

                Thread.Sleep(610000); // Wait 10 minutes seconds. 5 minutes token time + 5 minutes clockscrew + 10 seconds extra to be sure that the response is expired
            };
            FiddlerApplication.BeforeResponse += _fiddlerApplicationOnBeforeResponse;

            // Act
            try
            {
                tokenService.GetToken();
                Assert.IsTrue(false, "Expected exception was not thrown!!!");
            }
            catch (MessageSecurityException mse)
            {
                // Assert
                var fe = mse.InnerException as FaultException;
                Assert.IsNotNull(fe, "Expected inner fault exception");
                Assert.AreEqual("At least one security token in the message could not be validated.", mse.Message);
            }
        }
    }
}
