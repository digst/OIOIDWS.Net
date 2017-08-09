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
using Digst.OioIdws.Test.Common;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Owin;

namespace Digst.OioIdws.Rest.SystemTests
{ 

    [TestClass]
    public class SeperateServersTests
    {
        [TestMethod]
        [TestCategory(Constants.SystemTest)]
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
                            new IssuerAudiences("d9f10c97aa647727adb64a349bb037c5c23c9a7a", "test cert")
                                .Audience(new Uri("https://wsp.itcrew.dk")),
                        }),
                        TrustedWspCertificateThumbprints = new[] {"dc35c0466ad606422adff717c9cb8e3274d8772e"},
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
                                CertificateUtil.GetCertificate("dc35c0466ad606422adff717c9cb8e3274d8772e"))

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
                ClientCertificate = CertificateUtil.GetCertificate("0919ed32cf8758a002b39c10352be7dcccf1222a"),
                AudienceUri = new Uri("https://wsp.itcrew.dk"),
                AccessTokenIssuerEndpoint = new Uri(asEndpoint + "/accesstoken/issue"),
                SecurityTokenService = new OioIdwsStsSettings
                {
                    Certificate = CertificateUtil.GetCertificate("d9f10c97aa647727adb64a349bb037c5c23c9a7a"),
                    EndpointAddress = new Uri("https://SecureTokenService.test-nemlog-in.dk/SecurityTokenService.svc"),
                    TokenLifeTime = TimeSpan.FromMinutes(5)
                }
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
