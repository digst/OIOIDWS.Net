using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Digst.OioIdws.Soap.Cross.Test.Utils;
using Digst.OioIdws.Common.Utils;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Soap.Cross.Healthcare.Test.Connected_Services.HelloJavaWorldProxy;

namespace Digst.OioIdws.Soap.Cross.Healthcare.Test
{
    [TestClass]
    public class SoapCrossHealthcareTest
    {
        private static Process _dotnetWspBearerExample;
        private static ISecurityTokenServiceClient _stsTokenService;

        private static int _tomcatPort = 8444;
        private static string _cd = Directory.GetCurrentDirectory();

        private static string _maven =
            Path.Combine(
                _cd,
                @"..\..\..\..\Examples\Digst.OioIdws.Java\tools\maven3\bin\mvn.cmd"
            );

        private static string _pomWscBearer =
            Path.Combine(
                _cd,
            @"..\..\..\..\Examples\Digst.OioIdws.Java\Healthcare\system-user-scenario-bearer\pom.xml"
            );

        private static string _pomWsp =
            Path.Combine(
                _cd,
            @"..\..\..\..\Examples\Digst.OioIdws.Java\Healthcare\service-hok\pom.xml"
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
                            RedirectStandardOutput = true
                        }
                };
            process.Start();

            return process;
        }


        private static void KillPortProcesses(int port)
        {
            var processes = SocketConnection.GetAllTcpConnections().Where(tcp => tcp.LocalPort == port).Select(tcp => tcp.ProcessId).Select(pid => Process.GetProcessById(pid)).Distinct().Where(x => !x.HasExited).ToList();
            foreach (var process in processes)
            {
                if (!process.HasExited) process.Kill();
            }

            foreach (var process in processes)
            {
                process.WaitForExit();
            }
        }

        [ClassInitialize]
        public static void Setup(TestContext context)
        {

            KillPortProcesses(8444);
            if (_dotnetWspBearerExample != null && !(_dotnetWspBearerExample.HasExited))
            {
                _dotnetWspBearerExample.Kill();
                _dotnetWspBearerExample.WaitForExit();
            }

            // Start .NET WSP 
            _dotnetWspBearerExample = ProcessInit(
                @"..\..\..\..\Examples\Digst.OioIdws.WspHealthcareBearerExample\bin\Debug\Digst.OioIdws.WspHealthcareBearerExample.exe"
            );

            // Start Java WSP (port 8444)
            ProcessInit(_maven, "tomcat7:run-war -U -f " + _pomWsp);

            var securityTokenServiceClientConfiguration = new SecurityTokenServiceClientConfiguration()
            {
                WscIdentifier = "https://digst.oioidws.wsp:9090/helloworld",
                BootstrapTokenFromAuthenticationTokenUrl = new Uri("https://sts-idws-xua:8181/service/sts"),
                IdentityTokenFromBootstrapTokenUrl = new Uri("https://sts-idws-xua:8181/service/sts"),
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
            // Shutdown .NET WSP
            _dotnetWspBearerExample.Kill();

            // Shutdown Tomcat server on port 8444
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

            //// Maven clean
            var mavenWsc = ProcessInit(_maven, "clean -f " + _pomWscBearer);
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
            var stdOut = string.Empty;

            // Maven clean + build from .NET WSP with custom WSDL
            var maven = ProcessInit(_maven, "clean install -U -f " + _pomWscBearer);

            stdOut = maven.StandardOutput.ReadToEnd();
            
            succeeded = stdOut.Contains(pattern);

            maven.WaitForExit();

            Assert.IsTrue(succeeded, stdOut);
        }

        [TestMethod]
        [TestCategory(Constants.CrossTest)]
        public void JavaWscCallDotnetWspTestBearer()
        {
            Thread.Sleep(5000);

            var succeeded = false;
            var patterns =
                new Dictionary<string, bool>
                {
                    { @"Hello None John", false },
                    { @"Hello None Jane", false },
                    { @"BUILD SUCCESS", false }
                };
            var keys = patterns.Keys.ToArray();

            var stdOut = string.Empty;

            // Exec from .NET WSP with custom WSDL
            var maven = ProcessInit(_maven, "clean install exec:exec -U -f " + _pomWscBearer);

            stdOut = maven.StandardOutput.ReadToEnd();

            foreach (var key in keys)
            {
                if (stdOut.Contains(key))
                {
                    patterns[key] = true;
                }
            }

            succeeded = true;

            foreach (var kv in patterns)
            {
                succeeded &= kv.Value;
            }

            Assert.IsTrue(succeeded, stdOut);
        }

        [TestMethod]
        [TestCategory(Constants.CrossTest)]
        public void DotnetWscCallJavaWspTest()
        {
            // Ensure that the WSP is up and running.
            Thread.Sleep(30000);

            var succeeded = false;
            
            // Client proxy
            var client = new HelloWorldPortTypeClient();

            var securityToken = _stsTokenService.GetServiceToken("http://localhost:8080/service/hello", KeyType.HolderOfKey);

            var channelWithIssuedToken =
                client.ChannelFactory.CreateChannelWithIssuedToken(
                    securityToken
                );

            var helloWorldRequestJohn = new HelloWorldRequest("John");

            var resp = channelWithIssuedToken.HelloWorld(helloWorldRequestJohn).response;

            succeeded = resp.Equals("Hello John");

            Assert.IsTrue(succeeded);
        }
    }
}
