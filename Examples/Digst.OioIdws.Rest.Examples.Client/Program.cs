using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
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
                ClientCertificate = CertificateUtil.GetCertificate(ConfigurationManager.AppSettings["ClientCertificate"]),
                AudienceUri = new Uri(ConfigurationManager.AppSettings["AudienceUri"]),
                AccessTokenIssuerEndpoint = new Uri(ConfigurationManager.AppSettings["AsEndpoint"]),
                SecurityTokenService = new OioIdwsStsSettings
                {
                    Certificate = CertificateUtil.GetCertificate(ConfigurationManager.AppSettings["StsCertificate"]),
                    EndpointAddress = new Uri(ConfigurationManager.AppSettings["StsEndpointAddress"]),
                    TokenLifeTime = TimeSpan.FromSeconds(int.Parse(ConfigurationManager.AppSettings["TokenLifeTimeInSeconds"]))
                }
            };

            var idwsClient = new OioIdwsClient(settings);

            var httpClient = new HttpClient(idwsClient.CreateMessageHandler());

            {
                //first invocation - security token is retrieved and stored in the AS, access token cached by client
                var response = await httpClient.GetAsync(ConfigurationManager.AppSettings["WspTestEndpointAddress"]);
                var responseString = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
                Console.WriteLine(responseString);
            }

            {
                //second invocation - cached access token is reused
                httpClient = new HttpClient(idwsClient.CreateMessageHandler());
                var wspTestEndpointAddress2 = ConfigurationManager.AppSettings["WspTestEndpointAddress2"];
                if (!string.IsNullOrWhiteSpace(wspTestEndpointAddress2))
                {
                    var response =
                        await httpClient.GetAsync(wspTestEndpointAddress2);
                    var responseString = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
                    Console.WriteLine(responseString);
                }
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