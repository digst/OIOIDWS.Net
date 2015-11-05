using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Threading;
using Digst.OioIdws.Test.HelloWorldProxy;
using Digst.OioIdws.Wsc.OioWsTrust;
using Fiddler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.Oioidws.Test
{
    [TestClass]
    public class LibBasLongRunningTests
    {
        private static Process _process;
        private SessionStateHandler _fiddlerApplicationOnBeforeRequest;
        private SessionStateHandler _fiddlerApplicationOnBeforeResponse;
        private const string WspHostName = "digst.oioidws.wsp";

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            // Start proxy server (to simulate man in the middle attacks)
            FiddlerApplication.Startup(8877, true, false, false);

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
        [TestCategory(Constants.IntegrationTest)]
        public void TotalFlowTokenExpiredTest()
        {
            // Arrange
            // Retrieve token
            var tokenService = new TokenService();
            var securityToken = tokenService.GetToken();
            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(securityToken);

            // Act
            try
            {
                Thread.Sleep(610000); // Wait 10 minutes and 10 seconds. 5 minutes token time + 5 minutes clockscrew + 10 seconds extra to be sure that token is expired
                channelWithIssuedToken.HelloSign("Schultz");
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
        public void LibBasRequestExpiredTest()
        {
            // Arrange
            // Retrieve token
            var tokenService = new TokenService();
            var securityToken = tokenService.GetToken();

            _fiddlerApplicationOnBeforeRequest = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (WspHostName != oS.hostname)
                    return;

                Thread.Sleep(610000); // Wait 10 minutes seconds. 5 minutes token time + 5 minutes clockscrew + 10 seconds extra to be sure that the request is expired
            };
            FiddlerApplication.BeforeRequest += _fiddlerApplicationOnBeforeRequest;

            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(securityToken);

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
                Assert.AreEqual("At least one security token in the message could not be validated.", mse.Message);
            }
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public void LibBasResponseExpiredTest()
        {
            // Arrange
            // Retrieve token
            var tokenService = new TokenService();
            var securityToken = tokenService.GetToken();

            _fiddlerApplicationOnBeforeResponse = delegate (Session oS)
            {
                // Only act on requests to WSP
                if (WspHostName != oS.hostname)
                    return;

                Thread.Sleep(610000); // Wait 10 minutes seconds. 5 minutes token time + 5 minutes clockscrew + 10 seconds extra to be sure that the response is expired
            };
            FiddlerApplication.BeforeResponse += _fiddlerApplicationOnBeforeResponse;

            var client = new HelloWorldClient();
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(securityToken);

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
                Assert.AreEqual("At least one security token in the message could not be validated.", mse.Message);
            }
        }
    }
}
