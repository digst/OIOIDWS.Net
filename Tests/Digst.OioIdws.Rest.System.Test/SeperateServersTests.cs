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
                            new IssuerAudiences("fcb5edc9fb09cf39716c09c35fdc883bd48add8d", "test cert")
                                .Audience(new Uri("https://wsp.oioidws-net.dk")),
                        }),
                        TrustedWspCertificateThumbprints = new[] {"ca30025a4981147505b8d7a59052ac40c7033688"},
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
                                CertificateUtil.GetCertificate("ca30025a4981147505b8d7a59052ac40c7033688"))

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
                ClientCertificate = CertificateUtil.GetCertificate("a402bb172929ae0d0ada62f6864329c35dc29483"),
                AudienceUri = new Uri("https://wsp.oioidws-net.dk"),
                AccessTokenIssuerEndpoint = new Uri(asEndpoint + "/accesstoken/issue"),
                SecurityTokenService = new OioIdwsStsSettings
                {
                    Certificate = CertificateUtil.GetCertificate("fcb5edc9fb09cf39716c09c35fdc883bd48add8d"),
                    EndpointAddress = new Uri("https://SecureTokenService.test-nemlog-in.dk/SecurityTokenService.svc"),
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
