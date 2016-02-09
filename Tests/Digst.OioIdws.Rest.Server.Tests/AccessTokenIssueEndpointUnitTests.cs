using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;
using Digst.OioIdws.Rest.Server.AuthorizationServer;
using Digst.OioIdws.Rest.Server.AuthorizationServer.Issuing;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;
using Digst.OioIdws.Test.Common;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using Owin;

namespace Digst.OioIdws.Rest.Server.Tests
{
    [TestClass]
    public class AccessTokenIssueEndpointUnitTests
    {
        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task IssueAccessToken_Success_ReturnsCorrectly()
        {
            var requestSamlToken = Utils.ToBase64("accesstoken1");
            var accessToken = "dummy";

            var accessTokenGeneratorMock = new Mock<IKeyGenerator>();
            var tokenStoreMock = new Mock<ISecurityTokenStore>();
            var tokenValidatorMock = new Mock<ITokenValidator>();

            accessTokenGeneratorMock
                .Setup(x => x.GenerateUniqueKey())
                .Returns(accessToken);

            tokenValidatorMock
                .Setup(x => x.ValidateTokenAsync(Utils.FromBase64(requestSamlToken), It.IsAny<X509Certificate2>(), It.IsAny<OioIdwsAuthorizationServiceOptions>()))
                .ReturnsAsync(new TokenValidationResult
                {
                    Success = true,
                    ClaimsIdentity = new ClaimsIdentity()
                });

            var tokenDataFormatMock = new Mock<ISecureDataFormat<AuthenticationProperties>>();
            tokenDataFormatMock
                .Setup(x => x.Protect(It.IsAny<AuthenticationProperties>()))
                .Returns(accessToken);

            var options = new OioIdwsAuthorizationServiceOptions
            {
                AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                AccessTokenRetrievalPath = new PathString("/accesstoken"),
                KeyGenerator = accessTokenGeneratorMock.Object,
                TokenValidator = tokenValidatorMock.Object,
                IssuerAudiences = () => Task.FromResult(new []
                {
                    new IssuerAudiences("thumbprint1", "name"), 
                }),
                SecurityTokenStore = tokenStoreMock.Object,
                TokenDataFormat = tokenDataFormatMock.Object,
            };
            using (var server = TestServer.Create(app =>
            {
                app.UseOioIdwsAuthorizationService(options);
            }))
            { 
                server.BaseAddress = new Uri("https://localhost/");

                var response = await server.HttpClient.PostAsync("/accesstoken/issue",
                            new FormUrlEncodedContent(new[]
                            {new KeyValuePair<string, string>("saml-token", requestSamlToken),}));

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.IsNotNull(response.Content.Headers.ContentType);
                Assert.AreEqual("UTF-8", response.Content.Headers.ContentType.CharSet);
                Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);
                var accesssTokenFromResponse = JObject.Parse(await response.Content.ReadAsStringAsync());
                Assert.AreEqual(accessToken, accesssTokenFromResponse["access_token"]);
                Assert.AreEqual("Bearer", accesssTokenFromResponse["token_type"]);
                Assert.AreEqual((int)options.AccessTokenExpiration.TotalSeconds, accesssTokenFromResponse["expires_in"]);
            }

            accessTokenGeneratorMock.Verify(x => x.GenerateUniqueKey(), Times.Once);
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task IssueAccessToken_OtherEndpoint_PassesThrough()
        {
            using (var server = TestServer.Create(app =>
            {
                app
                    .UseOioIdwsAuthorizationService(new OioIdwsAuthorizationServiceOptions
                    {
                        AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                        AccessTokenRetrievalPath = new PathString("/accesstoken"),
                        IssuerAudiences = () => Task.FromResult(new IssuerAudiences[0])
                    })
                    .Use((context, next) =>
                    {
                        context.Response.Write("finalmiddleware");
                        return Task.FromResult(0);
                    });
            }))
            {
                var response = await server.CreateRequest("/otherendpoint").PostAsync();
                var text = await response.Content.ReadAsStringAsync();
                Assert.IsTrue(text == "finalmiddleware");
            }
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task IssueAccessToken_InvalidRequest_ReturnsBadRequest()
        {
            var accessTokenGeneratorMock = new Mock<IKeyGenerator>();
            var tokenStoreMock = new Mock<ISecurityTokenStore>();

            using (var server = TestServer.Create(app =>
            {
                app.Use<OioIdwsAuthorizationServiceMiddleware>(app, new OioIdwsAuthorizationServiceOptions
                {
                    AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                    AccessTokenRetrievalPath = new PathString("/accesstoken"),
                    IssuerAudiences = () => Task.FromResult(new IssuerAudiences[0]),
                    KeyGenerator = accessTokenGeneratorMock.Object,
                    SecurityTokenStore = tokenStoreMock.Object,
                });
            }))
            {
                server.BaseAddress = new Uri("https://localhost/");

                var response = await server.CreateRequest("/accesstoken/issue").PostAsync();
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

                var authHeader = response.Headers.WwwAuthenticate.Single(x => x.Scheme == "Holder-of-key");
                var bearerParameters = HttpHeaderUtils.ParseOAuthSchemeParameter(authHeader.Parameter);
                Assert.AreEqual(AuthenticationErrorCodes.InvalidRequest, bearerParameters["error"]);
            }
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task IssueAccessToken_SetShouldExpireIn_HonorsExpiration()
        {
            var requestSamlToken = Utils.ToBase64("samltoken1");
            var expiration = 200;

            var issuedAt = new DateTimeOffset(2016, 1, 1, 12, 00, 0, TimeSpan.Zero);

            AuthenticationProperties authenticationProperties = null; //set during token protection

            var clockMock = new Mock<ISystemClock>();
            clockMock.SetupGet(x => x.UtcNow).Returns(issuedAt);

            var tokenStoreMock = new Mock<ISecurityTokenStore>();

            var tokenValidatorMock = new Mock<ITokenValidator>();
            tokenValidatorMock
                .Setup(x => x.ValidateTokenAsync(Utils.FromBase64(requestSamlToken), It.IsAny<X509Certificate2>(), It.IsAny<OioIdwsAuthorizationServiceOptions>()))
                .ReturnsAsync(new TokenValidationResult
                {
                    ClaimsIdentity = new ClaimsIdentity(),
                    AccessTokenType = AccessTokenType.Bearer,
                    Success = true,
                });

            var tokenDataFormatMock = new Mock<ISecureDataFormat<AuthenticationProperties>>();
            tokenDataFormatMock
                .Setup(x => x.Protect(It.IsAny<AuthenticationProperties>()))
                .Returns((AuthenticationProperties props) =>
                {
                    authenticationProperties = props;
                    return "tokenvalue";
                });

            var authorizationOptions = new OioIdwsAuthorizationServiceOptions
            {
                AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                AccessTokenRetrievalPath = new PathString("/accesstoken"),
                IssuerAudiences = () => Task.FromResult(new IssuerAudiences[0]),
                SecurityTokenStore = tokenStoreMock.Object,
                TokenValidator = tokenValidatorMock.Object,
                SystemClock = clockMock.Object,
                TokenDataFormat = tokenDataFormatMock.Object,
            };

            using (var server = TestServer.Create(app =>
            {
                app.Use<OioIdwsAuthorizationServiceMiddleware>(app, authorizationOptions);
            }))
            {
                server.BaseAddress = new Uri("https://localhost/");

                var response = await server.HttpClient.PostAsync("/accesstoken/issue",
                    new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        {"saml-token", requestSamlToken},
                        {"should-expire-in", expiration.ToString()},
                    }));

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                var tokenInfo = JObject.Parse(await response.Content.ReadAsStringAsync());
                Assert.AreEqual(expiration, tokenInfo["expires_in"]);

                var expectedExpiration = issuedAt + TimeSpan.FromSeconds(expiration);
                Assert.AreEqual(expectedExpiration, authenticationProperties.ExpiresUtc);
                tokenStoreMock.Verify(v => v.StoreTokenAsync(authenticationProperties.Dictionary["value"], It.Is<OioIdwsToken>(x => x.ExpiresUtc == authenticationProperties.ExpiresUtc.Value)));
            }
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task IssueAccessToken_SetShouldExpireInToHigh_CapsExpiration()
        {
            var requestSamlToken = Utils.ToBase64("samltoken1");
            var expiration = 1000;
            var serverExpiration = 500;

            var tokenStoreMock = new Mock<ISecurityTokenStore>();

            var tokenValidatorMock = new Mock<ITokenValidator>();
            tokenValidatorMock
                .Setup(x => x.ValidateTokenAsync(Utils.FromBase64(requestSamlToken), It.IsAny<X509Certificate2>(), It.IsAny<OioIdwsAuthorizationServiceOptions>()))
                .ReturnsAsync(new TokenValidationResult
                {
                    ClaimsIdentity = new ClaimsIdentity(),
                    AccessTokenType = AccessTokenType.Bearer,
                    Success = true,
                });

            using (var server = TestServer.Create(app =>
            {
                app.Use<OioIdwsAuthorizationServiceMiddleware>(app, new OioIdwsAuthorizationServiceOptions
                {
                    AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                    AccessTokenRetrievalPath = new PathString("/accesstoken"),
                    IssuerAudiences = () => Task.FromResult(new IssuerAudiences[0]),
                    SecurityTokenStore = tokenStoreMock.Object,
                    TokenValidator = tokenValidatorMock.Object,
                    AccessTokenExpiration = TimeSpan.FromSeconds(serverExpiration),
                });
            }))
            {
                server.BaseAddress = new Uri("https://localhost/");

                var response = await server.HttpClient.PostAsync("/accesstoken/issue",
                    new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        {"saml-token", requestSamlToken},
                        {"should-expire-in", expiration.ToString()},
                    }));

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                var tokenInfo = JObject.Parse(await response.Content.ReadAsStringAsync());
                Assert.AreEqual(serverExpiration, tokenInfo["expires_in"]);
                Assert.IsTrue(serverExpiration < expiration);
            }
        }
    }
}