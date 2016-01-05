using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Digst.OioIdws.Common.Logging;
using Digst.OioIdws.Rest.Client;

namespace Digst.OioIdws.Rest.Examples.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Go().GetAwaiter().GetResult();
        }

        public static async Task Go()
        {
            //configures the internal logger for OIO WS-TRUST communication
            LoggerFactory.SetLogger(new ConsoleLogger());

            var settings = new OioIdwsClientSettings
            {
                ClientCertificate = CertificateUtil.GetCertificate("0919ed32cf8758a002b39c10352be7dcccf1222a"),
                AudienceUri = new Uri("https://wsp.itcrew.dk"),
                AccessTokenIssuerEndpoint = new Uri("https://digst.oioidws.rest.as:10001/accesstoken/issue"),
                SecurityTokenService = new OioIdwsStsSettings
                {
                    Certificate = CertificateUtil.GetCertificate("2e7a061560fa2c5e141a634dc1767dacaeec8d12"),
                    EndpointAddress = new Uri("https://SecureTokenService.test-nemlog-in.dk/SecurityTokenService.svc"),
                }
            };

            var idwsClient = new OioIdwsClient(settings);

            var httpClient = new HttpClient(idwsClient.CreateMessageHandler());

            {
                //first invocation - security token is retrieved and stored in the AS, access token cached by client
                var response = await httpClient.GetAsync("https://digst.oioidws.rest.wsp:10002/hello");
                var responseString = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
                Console.WriteLine(responseString);
            }

            {
                //second invocation - cached access token is reused
                var response = await httpClient.GetAsync("https://digst.oioidws.rest.wsp:10002/hello2");
                var responseString = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
                Console.WriteLine(responseString);
            }
        }
    }

    internal class ConsoleLogger : ILogger
    {
        public void WriteCore(TraceEventType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            Console.WriteLine($"{eventType}: {formatter(state, exception)}");
        }
    }
}