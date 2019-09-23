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
            _process = Process.Start(@"..\..\..\..\Examples\Healthcare\Healthcare.WspExample\bin\Debug\Digst.OioIdws.WspHealthcareExample.exe");

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

            _stsTokenService = new LocalSecurityTokenServiceClient(securityTokenServiceClientConfiguration);
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
        public void TotalFlowNoneSuccessTest()
        {
            // Arrange
            var client = new HelloWorldClient();
            var token = _stsTokenService.GetServiceToken(WspUri, KeyType.HolderOfKey);
            var channelWithIssuedToken = client.ChannelFactory.CreateChannelWithIssuedToken(token);

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

    }
}
