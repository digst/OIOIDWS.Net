using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Digst.OioIdws.Common.Logging;
using Digst.OioIdws.Rest.Client;

namespace Digst.OioIdws.Rest.Examples.ClientNuget
{
    class Program
    {
        static void Main(string[] args)
        {
            Go().GetAwaiter().GetResult();
        }

        public static async Task Go()
        {
            Console.WriteLine(
                @"Enter <1> to use seperate test servers for for AS and WSP.
Requires the following examples applications are running:
Digst.OioIdws.Rest.Examples.AS.exe
Digst.OioIdws.Rest.Examples.WSP.exe

Enter <2> to use the combined test server.
Requires the following example application is running:
Digst.OioIdws.Rest.Examples.ServerCombined.exe");
            char c;
            while (!new[] {'1', '2'}.Contains(c = Console.ReadKey().KeyChar))
            {

            }

            string asEndpoint = "https://digst.oioidws.rest.as:10001/accesstoken/issue";

            if (c == '2')
            {
                asEndpoint = "https://digst.oioidws.rest.wsp:10002/accesstoken/issue";
            }

            //configures the internal logger for OIO WS-TRUST communication
            LoggerFactory.SetLogger(new ConsoleLogger());

            var settings = new OioIdwsClientSettings
            {
                ClientCertificate = CertificateUtil.GetCertificate("0919ed32cf8758a002b39c10352be7dcccf1222a"),
                AudienceUri = new Uri("https://wsp.itcrew.dk"),
                AccessTokenIssuerEndpoint = new Uri(asEndpoint),
                SecurityTokenService = new OioIdwsStsSettings
                {
                    Certificate = CertificateUtil.GetCertificate("d9f10c97aa647727adb64a349bb037c5c23c9a7a"),
                    EndpointAddress = new Uri("https://SecureTokenService.test-nemlog-in.dk/SecurityTokenService.svc"),
                }
            };

            var idwsClient = new OioIdwsClient(settings);

            using (var httpClient = new HttpClient(idwsClient.CreateMessageHandler()))
            {
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
    }

    internal class ConsoleLogger : ILogger
    {
        public void WriteCore(TraceEventType eventType, int eventId, object state, Exception exception,
            Func<object, Exception, string> formatter)
        {
            Console.WriteLine($"{eventType}: {formatter(state, exception)}");
        }
    }
}

