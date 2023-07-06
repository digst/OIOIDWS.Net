﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Client;
using Digst.OioIdws.Rest.Server.AuthorizationServer;
using Digst.OioIdws.Rest.Server.Wsp;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Owin;
using Digst.OioIdws.Common.Utils;

namespace Digst.OioIdws.Rest.System.Test
{
    [TestClass]
    public class ServerCombinedTests
    {
        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public async Task CallWspService_Authenticates_ReturnsUserInformation()
        {
            var serverEndpoint = "https://digst.oioidws.rest.wsp:10002";

            var wspServer = WebApp.Start(serverEndpoint, app =>
            {
                app.SetLoggerFactory(new ConsoleLoggerFactory());

                app
                    .UseOioIdwsAuthentication(new OioIdwsAuthenticationOptions())
                    .UseOioIdwsAuthorizationService(new OioIdwsAuthorizationServiceOptions
                    {
                        AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                        IssuerAudiences = () => Task.FromResult(new[]
                        {
                            new IssuerAudiences("357faaab559e427fcf66bf81627378a86a1106c3", "test cert")
                                .Audience(new Uri("https://wsp.oioidws-net.dk")),
                        }),
                    })
                    .Use(async (context, next) =>
                    {
                        var identity = (ClaimsIdentity) context.Request.User.Identity;
                        await context.Response.WriteAsync(identity.Claims
                            .Single(x => x.Type == "dk:gov:saml:attribute:CvrNumberIdentifier").Value);
                    });
            });
            {
                var settings = new OioIdwsClientSettings
                {
                    ClientCertificate = CertificateUtil.GetCertificate("8ba800bd54682d2b1d4713f41bf6698763f106e5"),
                    AudienceUri = new Uri("https://wsp.oioidws-net.dk"),
                    AccessTokenIssuerEndpoint = new Uri(serverEndpoint + "/accesstoken/issue"),
                    SecurityTokenService = new OioIdwsStsSettings
                    {
                        Certificate = CertificateUtil.GetCertificate("357faaab559e427fcf66bf81627378a86a1106c3"),
                        EndpointAddress = new Uri("https://SecureTokenService.test-devtest4-nemlog-in.dk/SecurityTokenService.svc"),
                        TokenLifeTime = TimeSpan.FromMinutes(5)
                    },
                    DesiredAccessTokenExpiry = TimeSpan.FromMinutes(5),
                    UseTokenCache = false
                };

                var idwsClient = new OioIdwsClient(settings);

                var httpClient = new HttpClient(idwsClient.CreateMessageHandler())
                {
                    BaseAddress = new Uri(serverEndpoint)
                };

                var response = await httpClient.GetAsync("/myservice");
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                var str = await response.Content.ReadAsStringAsync();
                Assert.AreEqual("34051178", str);

                wspServer.Dispose();
            }
        }
    }
}
