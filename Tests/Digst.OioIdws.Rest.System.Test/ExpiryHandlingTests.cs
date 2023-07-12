using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Client;
using Digst.OioIdws.Rest.Server.AuthorizationServer;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;
using Digst.OioIdws.Rest.Server.Wsp;
using Digst.OioIdws.Common.Utils;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Logging;
using Moq;
using Owin;

namespace Digst.OioIdws.Rest.System.Test
{
    [TestClass]
    public class ExpiryHandlingTests
    {
        /// <summary>
        /// This test depends on timing, therefore it contains sleeps that SHOULD be longer than the clock skew and token lifetime combined
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        [TestCategory(Constants.IntegrationTestLongRunning)]
        public async Task CallWspService_ShortTokenLifeTime_RenegotiatesAccessToken()
        {
            var serverEndpoint = "https://digst.oioidws.rest.wsp:10002";

            var tokensIssuedCount = 0;

            using (WebApp.Start(serverEndpoint, app =>
            {
                app.SetLoggerFactory(new ConsoleLoggerFactory());

                var tokenStore = new InMemorySecurityTokenStore();

                var tokenStoreWrapper = new Mock<ISecurityTokenStore>();
                tokenStoreWrapper
                    .Setup(x => x.RetrieveTokenAsync(It.IsAny<string>()))
                    .Returns((string accessToken) => tokenStore.RetrieveTokenAsync(accessToken));
                tokenStoreWrapper
                    .Setup(x => x.StoreTokenAsync(It.IsAny<string>(), It.IsAny<OioIdwsToken>()))
                    .Returns((string accessToken, OioIdwsToken token) =>
                    {
                        tokensIssuedCount++;
                        return tokenStore.StoreTokenAsync(accessToken, token);
                    });

                var authorizationServerOptions = new OioIdwsAuthorizationServiceOptions
                {
                    AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                    IssuerAudiences = () => Task.FromResult(new[]
                    {
                        new IssuerAudiences("357faaab559e427fcf66bf81627378a86a1106c3", "test cert")
                            .Audience(new Uri("https://wsp.oioidws-net.dk")),
                    }),
                    SecurityTokenStore = tokenStoreWrapper.Object,
                    MaxClockSkew = TimeSpan.FromSeconds(10), //a little time skew is needed for trusting STS tokens
                };

                app
                    .UseOioIdwsAuthentication(new OioIdwsAuthenticationOptions())
                    .UseOioIdwsAuthorizationService(authorizationServerOptions)
                    .Use(async (context, next) =>
                    {
                        if (context.Request.User == null)
                        {
                            //we expect the service to REQUIRE authorization
                            context.Response.StatusCode = 401;
                            return;
                        }

                        var identity = (ClaimsIdentity) context.Request.User.Identity;
                        await context.Response.WriteAsync(identity.Claims
                            .Single(x => x.Type == "dk:gov:saml:attribute:CvrNumberIdentifier").Value);
                    });
            }))
            {
                var settings = new OioIdwsClientSettings
                {
                    ClientCertificate = CertificateUtil.GetCertificate("8ba800bd54682d2b1d4713f41bf6698763f106e5"),
                    AudienceUri = new Uri("https://wsp.oioidws-net.dk"),
                    AccessTokenIssuerEndpoint = new Uri(serverEndpoint + "/accesstoken/issue"),
                    SecurityTokenService = new OioIdwsStsSettings
                    {
                        Certificate = CertificateUtil.GetCertificate("357faaab559e427fcf66bf81627378a86a1106c3"),
                        EndpointAddress =
                            new Uri("https://SecureTokenService.test-devtest4-nemlog-in.dk/SecurityTokenService.svc"),
                        TokenLifeTime = TimeSpan.FromMinutes(5)
                    },
                    DesiredAccessTokenExpiry = TimeSpan.FromSeconds(5), //set a very low token expiry time
                };

                var idwsClient = new OioIdwsClient(settings);

                {
                    var handler = (OioIdwsRequestHandler) idwsClient.CreateMessageHandler();

                    var httpClient = new HttpClient(handler)
                    {
                        BaseAddress = new Uri(serverEndpoint)
                    };

                    //first request, token should be valid
                    var response = await httpClient.GetAsync("/myservice");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                    var str = await response.Content.ReadAsStringAsync();
                    Assert.AreEqual("34051178", str);
                }

                {
                    var handler = (OioIdwsRequestHandler) idwsClient.CreateMessageHandler();

                    var httpClient = new HttpClient(handler)
                    {
                        BaseAddress = new Uri(serverEndpoint)
                    };

                    Thread.Sleep(TimeSpan.FromSeconds(30));
                    //second request, time has passed, token should be renegotiated
                    var response = await httpClient.GetAsync("/myservice");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                    var str = await response.Content.ReadAsStringAsync();
                    Assert.AreEqual("34051178", str);
                }

                Assert.AreEqual(2, tokensIssuedCount);
            }
        }

        /// <summary>
        /// This test verifies that the access token cache is used
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        [TestCategory(Constants.IntegrationTestLongRunning)]
        public async Task CallWspService_ShortTokenLifeTime_ReuseAccessToken()
        {
            var serverEndpoint = "https://digst.oioidws.rest.wsp:10002";

            var tokensIssuedCount = 0;

            using (WebApp.Start(serverEndpoint, app =>
            {
                app.SetLoggerFactory(new ConsoleLoggerFactory());

                var tokenStore = new InMemorySecurityTokenStore();

                var tokenStoreWrapper = new Mock<ISecurityTokenStore>();
                tokenStoreWrapper
                    .Setup(x => x.RetrieveTokenAsync(It.IsAny<string>()))
                    .Returns((string accessToken) => tokenStore.RetrieveTokenAsync(accessToken));
                tokenStoreWrapper
                    .Setup(x => x.StoreTokenAsync(It.IsAny<string>(), It.IsAny<OioIdwsToken>()))
                    .Returns((string accessToken, OioIdwsToken token) =>
                    {
                        tokensIssuedCount++;
                        return tokenStore.StoreTokenAsync(accessToken, token);
                    });

                var authorizationServerOptions = new OioIdwsAuthorizationServiceOptions
                {
                    AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                    IssuerAudiences = () => Task.FromResult(new[]
                    {
                        new IssuerAudiences("357faaab559e427fcf66bf81627378a86a1106c3", "test cert")
                            .Audience(new Uri("https://wsp.oioidws-net.dk")),
                    }),
                    SecurityTokenStore = tokenStoreWrapper.Object,
                    MaxClockSkew = TimeSpan.FromSeconds(10), //a little time skew is needed for trusting STS tokens
                };

                app
                    .UseOioIdwsAuthentication(new OioIdwsAuthenticationOptions())
                    .UseOioIdwsAuthorizationService(authorizationServerOptions)
                    .Use(async (context, next) =>
                    {
                        if (context.Request.User == null)
                        {
                            //we expect the service to REQUIRE authorization
                            context.Response.StatusCode = 401;
                            return;
                        }

                        var identity = (ClaimsIdentity)context.Request.User.Identity;
                        await context.Response.WriteAsync(identity.Claims.Single(x => x.Type == "dk:gov:saml:attribute:CvrNumberIdentifier").Value);
                    });
            }))
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
                        TokenLifeTime = TimeSpan.FromMinutes(10)
                    },
                    DesiredAccessTokenExpiry = TimeSpan.FromMinutes(10), //set a very low token expiry time
                    UseTokenCache = true
                };

                var idwsClient = new OioIdwsClient(settings);

                {
                    var handler = (OioIdwsRequestHandler)idwsClient.CreateMessageHandler();

                    var httpClient = new HttpClient(handler)
                    {
                        BaseAddress = new Uri(serverEndpoint)
                    };

                    //first request, token should be valid
                    var response = await httpClient.GetAsync("/myservice");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                    var str = await response.Content.ReadAsStringAsync();
                    Assert.AreEqual("34051178", str);
                }

                {
                    var handler = (OioIdwsRequestHandler)idwsClient.CreateMessageHandler();

                    var httpClient = new HttpClient(handler)
                    {
                        BaseAddress = new Uri(serverEndpoint)
                    };

                    //second request, time has passed, token should be renegotiated
                    var response = await httpClient.GetAsync("/myservice");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                    var str = await response.Content.ReadAsStringAsync();
                    Assert.AreEqual("34051178", str);
                }

                Assert.AreEqual(1, tokensIssuedCount);
            }
        }
    }
}
