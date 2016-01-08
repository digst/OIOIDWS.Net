using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;
using Owin;

namespace Digst.OioIdws.Rest.Server.Tests
{
    public class TestServerWithClientCertificate : TestServer
    {
        private readonly X509Certificate2 _clientCertificate;

        public TestServerWithClientCertificate(X509Certificate2 clientCertificate)
        {
            if (clientCertificate == null)
            {
                throw new ArgumentNullException(nameof(clientCertificate));
            }
            _clientCertificate = clientCertificate;
        }

        public static TestServerWithClientCertificate Create(X509Certificate2 clientCertificate, Action<IAppBuilder> startup)
        {
            var server = new TestServerWithClientCertificate(clientCertificate);
            server.Configure(startup);
            return server;
        }

        public new Task Invoke(IDictionary<string, object> environment)
        {
            //adds the client certificate to the OWIN request
            environment["ssl.ClientCertificate"] = _clientCertificate;
            return base.Invoke(environment);
        }

        public new HttpMessageHandler Handler => new OwinClientHandler(Invoke);

        public new HttpClient HttpClient => new HttpClient(Handler) { BaseAddress = BaseAddress };
    }
}
