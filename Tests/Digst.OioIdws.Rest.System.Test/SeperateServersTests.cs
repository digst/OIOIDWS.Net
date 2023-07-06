using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Client;
using Digst.OioIdws.Rest.Server.AuthorizationServer;
using Digst.OioIdws.Rest.Server.Wsp;
using Digst.OioIdws.Common.Utils;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Owin;

namespace Digst.OioIdws.Rest.System.Test
{ 

    [TestClass]
    public class SeperateServersTests
    {
        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public async Task CallWspService_Authenticates_ReturnsUserInformation()
        {
            var asEndpoint = "https://digst.oioidws.rest.as:10001";
            var wspEndpoint = "https://digst.oioidws.rest.wsp:10002";

            var asServer = WebApp.Start(asEndpoint, app =>
            {
                app.SetLoggerFactory(new ConsoleLoggerFactory());

                app
                    .UseOioIdwsAuthorizationService(new OioIdwsAuthorizationServiceOptions
                    {
                        AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                        AccessTokenRetrievalPath = new PathString("/accesstoken"),
                        IssuerAudiences = () => Task.FromResult(new[]
                        {
                            new IssuerAudiences("357faaab559e427fcf66bf81627378a86a1106c3", "test cert")
                                .Audience(new Uri("https://wsp.oioidws-net.dk")),
                        }),
                        TrustedWspCertificateThumbprints = new[] {"d738a7d146f07e02c16cf28dac11e742e4ce9582"},
                    });
            });

            var wspServer = WebApp.Start(wspEndpoint, app =>
            {
                app.SetLoggerFactory(new ConsoleLoggerFactory());

                app
                    .UseErrorPage()
                    .UseOioIdwsAuthentication(new OioIdwsAuthenticationOptions
                    {
                        TokenProvider =
                            new RestTokenProvider(new Uri(asEndpoint + "/accesstoken"),
                                CertificateUtil.GetCertificate("d738a7d146f07e02c16cf28dac11e742e4ce9582"))

                    })
                    .Use(async (context, next) =>
                    {
                        var identity = (ClaimsIdentity) context.Request.User.Identity;
                        await
                            context.Response.WriteAsync(
                                identity.Claims.Single(x => x.Type == "dk:gov:saml:attribute:CvrNumberIdentifier").Value);
                    });
            });

            var settings = new OioIdwsClientSettings
            {
                ClientCertificate = CertificateUtil.GetCertificate("8ba800bd54682d2b1d4713f41bf6698763f106e5"),
                AudienceUri = new Uri("https://wsp.oioidws-net.dk"),
                AccessTokenIssuerEndpoint = new Uri(asEndpoint + "/accesstoken/issue"),
                SecurityTokenService = new OioIdwsStsSettings
                {
                    Certificate = CertificateUtil.GetCertificate("357faaab559e427fcf66bf81627378a86a1106c3"),
                    EndpointAddress = new Uri("https://SecureTokenService.test-devtest4-nemlog-in.dk/SecurityTokenService.svc"),
                    TokenLifeTime = TimeSpan.FromMinutes(5)
                },
                UseTokenCache = false
            };

            var idwsClient = new OioIdwsClient(settings);

            var httpClient = new HttpClient(idwsClient.CreateMessageHandler())
            {
                BaseAddress = new Uri(wspEndpoint)
            };

            var response = await httpClient.GetAsync("/myservice");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var str = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("34051178", str);

            wspServer.Dispose();
            asServer.Dispose();
        }
    }
}
