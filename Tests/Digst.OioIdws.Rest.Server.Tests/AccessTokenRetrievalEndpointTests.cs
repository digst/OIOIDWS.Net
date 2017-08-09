using System;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;
using Digst.OioIdws.Rest.Server.AuthorizationServer;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;
using Digst.OioIdws.Test.Common;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Digst.OioIdws.Rest.Server.Tests
{
    [TestClass]
    public class AccessTokenRetrievalEndpointTests
    {
        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task RetrieveAccessToken_Success_TokenInformationIsInResponse()
        {
            var wspCertificate = CertificateUtil.GetCertificate("d9f10c97aa647727adb64a349bb037c5c23c9a7a");

            var accessToken = "dummy";
            var oioIdwsTokenKey = "accesstoken1";
            var token = new OioIdwsToken
            {
                Type = AccessTokenType.Bearer,
                ExpiresUtc = DateTime.UtcNow.AddHours(1),
                Claims = new[]
                {
                    new OioIdwsClaim
                    {
                        Type = "type1",
                        Value = "value1",
                        ValueType = "valuetype1",
                        Issuer = "issuer1",
                    },
                    new OioIdwsClaim
                    {
                        Type = "type2",
                        Value = "value2",
                        ValueType = "valuetype2",
                        Issuer = "issuer2",
                    },
                }
            };

            var tokenStoreMock = new Mock<ISecurityTokenStore>();
            tokenStoreMock
                .Setup(x => x.RetrieveTokenAsync(oioIdwsTokenKey))
                .ReturnsAsync(token);

            var tokenDataFormatMock = new Mock<ISecureDataFormat<AuthenticationProperties>>();
            tokenDataFormatMock
                .Setup(x => x.Unprotect(accessToken))
                .Returns(new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
                    Dictionary = { { "value", oioIdwsTokenKey } }
                });

            var options = new OioIdwsAuthorizationServiceOptions
            {
                AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                AccessTokenRetrievalPath = new PathString("/accesstoken"),
                IssuerAudiences = () => Task.FromResult(new IssuerAudiences[0]),
                SecurityTokenStore = tokenStoreMock.Object,
                TokenDataFormat = tokenDataFormatMock.Object,
                TrustedWspCertificateThumbprints = new[] { "d9f10c97aa647727adb64a349bb037c5c23c9a7a" },
                CertificateValidator = X509CertificateValidator.None //no reason for tests to validate certs
            };

            using (var server = TestServerWithClientCertificate.Create(() => wspCertificate, app =>
            {
                app.UseOioIdwsAuthorizationService(options);
            }))
            {
                server.BaseAddress = new Uri("https://localhost/");

                var response = await server.HttpClient.GetAsync($"/accesstoken?{accessToken}");
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);
                var responseToken = JsonConvert.DeserializeObject<OioIdwsToken>(await response.Content.ReadAsStringAsync());

                Assert.AreEqual(token.Type, responseToken.Type);
                Assert.AreEqual(token.ExpiresUtc, responseToken.ExpiresUtc);
                Assert.AreEqual(token.Claims.Count, responseToken.Claims.Count);
                Assert.AreEqual(token.Claims.ElementAt(0).Type, responseToken.Claims.ElementAt(0).Type);
                Assert.AreEqual(token.Claims.ElementAt(0).Value, responseToken.Claims.ElementAt(0).Value);
            }
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task RetrieveAccessToken_InvalidCertificate_ReturnsUnauthorized()
        {
            var options = new OioIdwsAuthorizationServiceOptions
            {
                AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                AccessTokenRetrievalPath = new PathString("/accesstoken"),
                IssuerAudiences = () => Task.FromResult(new IssuerAudiences[0]),
                TrustedWspCertificateThumbprints = new[] { "other cert" },
                CertificateValidator = X509CertificateValidator.None //no reason for tests to validate certs
            };

            X509Certificate2 certificate = null;

            using (var server = TestServerWithClientCertificate.Create(() => certificate, app =>
            {
                app.UseOioIdwsAuthorizationService(options);
            }))
            {
                server.BaseAddress = new Uri("https://localhost/");

                //without presenting certificate
                var response = await server.HttpClient.GetAsync($"/accesstoken?accesstoken1");
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);

                //with untrusted certificate
                certificate = CertificateUtil.GetCertificate("d9f10c97aa647727adb64a349bb037c5c23c9a7a");
                response = await server.HttpClient.GetAsync($"/accesstoken?accesstoken1");
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task RetrieveAccessToken_ExpiredAccessToken_ReturnsUnauthorized()
        {
            var wspCertificate = CertificateUtil.GetCertificate("d9f10c97aa647727adb64a349bb037c5c23c9a7a");

            var accessToken = "accessToken1";
            var oioIdwsTokenKey = "tokenValue1";

            var tokenInformation = new OioIdwsToken();

            var authProperties = new AuthenticationProperties
            {
                Dictionary = { {"value", oioIdwsTokenKey} }
            };

            var tokenDataFormatMock = new Mock<ISecureDataFormat<AuthenticationProperties>>();
            tokenDataFormatMock
                .Setup(x => x.Unprotect(accessToken))
                .Returns(() => authProperties);

            var currentTime = DateTimeOffset.UtcNow; //ensure static time during test

            var timeMock = new Mock<ISystemClock>();
            // ReSharper disable once AccessToModifiedClosure
            timeMock
                .SetupGet(x => x.UtcNow)
                .Returns(() => currentTime);

            var storeMock = new Mock<ISecurityTokenStore>();
            storeMock
                .Setup(x => x.RetrieveTokenAsync(oioIdwsTokenKey))
                .Returns(() => Task.FromResult(tokenInformation));

            var options = new OioIdwsAuthorizationServiceOptions
            {
                AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                AccessTokenRetrievalPath = new PathString("/accesstoken"),
                IssuerAudiences = () => Task.FromResult(new IssuerAudiences[0]),
                TrustedWspCertificateThumbprints = new[] { "d9f10c97aa647727adb64a349bb037c5c23c9a7a" },
                CertificateValidator = X509CertificateValidator.None, //no reason for tests to validate certs
                TokenDataFormat = tokenDataFormatMock.Object,
                SystemClock = timeMock.Object,
                MaxClockSkew = TimeSpan.FromMinutes(5),
                SecurityTokenStore = storeMock.Object,
            };

            using (var server = TestServerWithClientCertificate.Create(() => wspCertificate, app =>
            {
                app.UseOioIdwsAuthorizationService(options);
            }))
            {
                server.BaseAddress = new Uri("https://localhost/");

                {
                    //test that token content is checked properly
                    authProperties.ExpiresUtc = currentTime - options.MaxClockSkew.Add(TimeSpan.FromSeconds(1));

                    var response = await server.HttpClient.GetAsync($"/accesstoken?{accessToken}");
                    Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
                    var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                    Assert.AreEqual(1, json["expired"].Value<int>());
                }

                {
                    //test that stored token information is checked properly
                    authProperties.ExpiresUtc = currentTime;
                    tokenInformation.ExpiresUtc = currentTime - options.MaxClockSkew.Add(TimeSpan.FromSeconds(1));

                    var response = await server.HttpClient.GetAsync($"/accesstoken?{accessToken}");
                    Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
                    var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                    Assert.AreEqual(1, json["expired"].Value<int>());
                }
            }
        }
    }
}
