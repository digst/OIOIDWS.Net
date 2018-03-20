using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Wsc.OioWsTrust;
using Digst.OioIdws.Soap.Cross.Test.Utils;
using Digst.OioIdws.Soap.Cross.Test.HelloWorldProxy;
using Digst.OioIdws.Common.Utils;

namespace Digst.OioIdws.Soap.CrossTest
{

    [TestClass]
    public class SoapCrossTest
    {
        private static Process _dotnetWspExample;

        private static int _tomcatPort = 8443;
        private static string _cd = Directory.GetCurrentDirectory();

        private static string _maven =
            Path.Combine(
                _cd,
                @"..\..\..\..\Examples\Digst.OioIdws.Java\tools\maven\bin\mvn.bat"
            );

        private static string _pomWsc =
            Path.Combine(
                _cd,
                @"..\..\..\..\Examples\Digst.OioIdws.Java\system-user-scenario-hok\pom.xml"
            );
        private static string _pomWsp =
            Path.Combine(
                _cd,
                @"..\..\..\..\Examples\Digst.OioIdws.Java\service-hok\pom.xml"
            );

        private static Process ProcessInit(string filename, string args = "")
        {
            var process =
                new Process
                {
                    StartInfo =
                        new ProcessStartInfo
                        {
                            FileName = filename,
                            Arguments = args,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                        }
                };
            process.Start();

            return process;
        }

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            // Start .NET WSP
            _dotnetWspExample = ProcessInit(
                @"..\..\..\..\Examples\Digst.OioIdws.WspExample\bin\Debug\Digst.OioIdws.WspExample.exe"
            );

            // Start Java WSP
            ProcessInit(_maven, "tomcat7:run-war -U -f " + _pomWsp);
        }

        [ClassCleanup]
        public static void TearDown()
        {
            // Shutdown .NET WSP
            _dotnetWspExample.Kill();

            // Shutdown Tomcat server on port 8443
            var tomcatPID =
                SocketConnection.GetAllTcpConnections()
                    .Where(tcp => tcp.LocalPort == _tomcatPort)
                    .Select(tcp => tcp.ProcessId)
                    .ToList();

            foreach (var pid in tomcatPID)
            {
                var p = Process.GetProcessById(pid);

                p.Kill();
                p.WaitForExit();
            }

            // Maven clean
            var mavenWsc = ProcessInit(_maven, "clean -f " + _pomWsc);
            var mavenWsp = ProcessInit(_maven, "clean -f " + _pomWsp);

            mavenWsc.WaitForExit();
            mavenWsp.WaitForExit();
        }

        [TestMethod]
        [TestCategory(Constants.CrossTest)]
        public void BuildJavaWscFromCustomDotnetWspWsdlTest()
        {
            Thread.Sleep(5000);

            var succeeded = false;
            var pattern = @"BUILD SUCCESS";
            var stdout = string.Empty;

            // Maven clean + build from .NET WSP with custom WSDL
            var maven = ProcessInit(_maven, "clean compile -U -f " + _pomWsc);

            while (!succeeded && (stdout = maven.StandardOutput.ReadLine()) != null)
            {
                succeeded = stdout.Contains(pattern);
            }

            maven.WaitForExit();

            Assert.IsTrue(succeeded);
        }

        [TestMethod]
        [TestCategory(Constants.CrossTest)]
        public void JavaWscCallDotnetWspTest()
        {
            Thread.Sleep(5000);

            var succeeded = false;
            var patterns =
                new Dictionary<string, bool>
                {
                    { @"Hello None Schultz. Your claims are:", false },
                    { @"Hello Sign Schultz. Your claims are:", false },
                    { @"Hello SignError Schultz. You can read signed but not encrypted SOAP faults ... nice!", false },
                    { @"BUILD SUCCESS", false }
                };
            var keys = patterns.Keys.ToArray();

            var stdout = string.Empty;

            // Maven clean + build + exec from .NET WSP with custom WSDL
            var maven = ProcessInit(_maven, "clean compile exec:exec -U -f " + _pomWsc);

            while (!succeeded && (stdout = maven.StandardOutput.ReadLine()) != null)
            {
                foreach (var key in keys)
                {
                    if (stdout.Contains(key))
                    {
                        patterns[key] = true;
                    }
                }

                succeeded = true;

                foreach (var kv in patterns)
                {
                    succeeded &= kv.Value;
                }
            }

            maven.WaitForExit();

            Assert.IsTrue(succeeded);
        }

        [TestMethod]
        [TestCategory(Constants.CrossTest)]
        public void DotnetWscCallJavaWspTest()
        {
            // Ensure that the WSP is up and running.
            Thread.Sleep(30000);

            var succeeded = false;

            // Retrieve token
            IStsTokenService stsTokenService =
                new StsTokenServiceCache(
                    TokenServiceConfigurationFactory.CreateConfiguration()
                );
            var securityToken = stsTokenService.GetToken();

            // Call WSP with token
            var client = new HelloWorldPortTypeClient();

            var channelWithIssuedToken =
                client.ChannelFactory.CreateChannelWithIssuedToken(
                    securityToken
                );

            var helloWorldRequestJohn = new HelloWorldRequest("John");

            succeeded =
                channelWithIssuedToken
                    .HelloWorld(helloWorldRequestJohn)
                        .response.Equals("Hello John");

            Assert.IsTrue(succeeded);
        }
    }
}