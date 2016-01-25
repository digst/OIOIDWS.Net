﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
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
using Digst.OioIdws.Test.Common;

namespace Digst.OioIdws.Rest.SystemTests
{
    [TestClass]
    public class ServerCombinedTests
    {
        [TestMethod]
        [TestCategory(Constants.SystemTest)]
        public async Task CallWspService_Authenticates_ReturnsUserInformation()
        {
            var serverEndpoint = "https://digst.oioidws.rest.wsp:10002";

            using (WebApp.Start(serverEndpoint, app =>
            {
                app.SetLoggerFactory(new ConsoleLoggerFactory());

                app
                    .UseOioIdwsAuthentication(new OioIdwsAuthenticationOptions())
                    .UseOioIdwsAuthorizationService(new OioIdwsAuthorizationServiceOptions
                    {
                        AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                        IssuerAudiences = () => Task.FromResult(new[]
                        {
                            new IssuerAudiences("2E7A061560FA2C5E141A634DC1767DACAEEC8D12", "test cert")
                                .Audience(new Uri("https://wsp.itcrew.dk")),
                        }),
                    })
                    .Use(async (context, next) =>
                    {
                        var identity = (ClaimsIdentity) context.Request.User.Identity;
                        await context.Response.WriteAsync(identity.Claims.Single(x => x.Type == "dk:gov:saml:attribute:CvrNumberIdentifier").Value);
                    });
            }))
            {
                var settings = new OioIdwsClientSettings
                {
                    ClientCertificate = CertificateUtil.GetCertificate("0919ed32cf8758a002b39c10352be7dcccf1222a"),
                    AudienceUri = new Uri("https://wsp.itcrew.dk"),
                    AccessTokenIssuerEndpoint = new Uri(serverEndpoint + "/accesstoken/issue"),
                    SecurityTokenService = new OioIdwsStsSettings
                    {
                        Certificate = CertificateUtil.GetCertificate("2e7a061560fa2c5e141a634dc1767dacaeec8d12"),
                        EndpointAddress = new Uri("https://SecureTokenService.test-nemlog-in.dk/SecurityTokenService.svc"),
                        TokenLifeTime = TimeSpan.FromMinutes(5)
                    },
                    DesiredAccessTokenExpiry = TimeSpan.FromMinutes(5),
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
            }
        }
    }
}
