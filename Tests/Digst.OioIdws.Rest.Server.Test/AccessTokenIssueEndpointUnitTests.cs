using System;
using System.Collections.Generic;
using System.Configuration;
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
using Digst.OioIdws.Common.Utils;
using Digst.OioIdws.Rest.Server.AuthorizationServer.CertificateRetrieval;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using Owin;

namespace Digst.OioIdws.Rest.Server.Test
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
        
        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task IssueAccessToken_Success_ReturnsCorrectly_FromHttpHeader()
        {
            var requestSamlToken = Utils.ToBase64("accesstoken1");
            var accessToken = "dummy";

            var accessTokenGeneratorMock = new Mock<IKeyGenerator>();
            var tokenStoreMock = new Mock<ISecurityTokenStore>();
            var tokenValidatorMock = new Mock<ITokenValidator>();
            var cert = new X509Certificate2(Convert.FromBase64String("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"));

            accessTokenGeneratorMock
                .Setup(x => x.GenerateUniqueKey())
                .Returns(accessToken);

            LoadConfigurationSection(new CertificateStrategySection
                { StrategyType = CertificateStrategyType.HttpHeader });
            tokenValidatorMock
                .Setup(x => x.ValidateTokenAsync(Utils.FromBase64(requestSamlToken), It.IsAny<X509Certificate2>(),
                    It.IsAny<OioIdwsAuthorizationServiceOptions>()))
                .ReturnsAsync(new TokenValidationResult
                {
                    Success = true,
                    ClaimsIdentity = new ClaimsIdentity()
                });

            string base64 = Convert.ToBase64String(cert.Export(X509ContentType.Cert));
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
                IssuerAudiences = () => Task.FromResult(new[]
                {
                    new IssuerAudiences("thumbprint1", "name")
                }),
                SecurityTokenStore = tokenStoreMock.Object,
                TokenDataFormat = tokenDataFormatMock.Object
            };
            using (var server = TestServer.Create(app => { app.UseOioIdwsAuthorizationService(options); }))
            {
                
                var content = new FormUrlEncodedContent(new[]
                    { new KeyValuePair<string, string>("saml-token", requestSamlToken) });
                content.Headers.Add("Client-Cert", base64);
                server.BaseAddress = new Uri("https://localhost/");

                var response = await server.HttpClient.PostAsync("/accesstoken/issue", content);


                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.IsNotNull(response.Content.Headers.ContentType);
                Assert.AreEqual("UTF-8", response.Content.Headers.ContentType.CharSet);
                Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);
                var accesssTokenFromResponse = JObject.Parse(await response.Content.ReadAsStringAsync());
                Assert.AreEqual(accessToken, accesssTokenFromResponse["access_token"]);
                Assert.AreEqual("Bearer", accesssTokenFromResponse["token_type"]);
                Assert.AreEqual((int)options.AccessTokenExpiration.TotalSeconds,
                    accesssTokenFromResponse["expires_in"]);
            }

            accessTokenGeneratorMock.Verify(x => x.GenerateUniqueKey(), Times.Once);
            tokenValidatorMock.Verify(x=>x.ValidateTokenAsync(Utils.FromBase64(requestSamlToken), It.IsAny<X509Certificate2>(),
                It.IsAny<OioIdwsAuthorizationServiceOptions>()), Times.Once);
        }
        
        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task IssueAccessToken_InvalidCertificateFromHeader_ReturnsBadRequest()
        {
            var requestSamlToken = Utils.ToBase64("accesstoken1");
            var accessToken = "dummy";

            var accessTokenGeneratorMock = new Mock<IKeyGenerator>();
            var tokenStoreMock = new Mock<ISecurityTokenStore>();
            var tokenValidatorMock = new Mock<ITokenValidator>();

            accessTokenGeneratorMock
                .Setup(x => x.GenerateUniqueKey())
                .Returns(accessToken);

            LoadConfigurationSection(new CertificateStrategySection
                { StrategyType = CertificateStrategyType.HttpHeader });
            tokenValidatorMock
                .Setup(x => x.ValidateTokenAsync(Utils.FromBase64(requestSamlToken), It.IsAny<X509Certificate2>(),
                    It.IsAny<OioIdwsAuthorizationServiceOptions>()))
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
                IssuerAudiences = () => Task.FromResult(new[]
                {
                    new IssuerAudiences("thumbprint1", "name")
                }),
                SecurityTokenStore = tokenStoreMock.Object,
                TokenDataFormat = tokenDataFormatMock.Object
            };
            using (var server = TestServer.Create(app => { app.UseOioIdwsAuthorizationService(options); }))
            {
                
                var content = new FormUrlEncodedContent(new[]
                    { new KeyValuePair<string, string>("saml-token", requestSamlToken) });
                content.Headers.Add("Client-Cert", "not-cert");
                server.BaseAddress = new Uri("https://localhost/");

                var response = await server.HttpClient.PostAsync("/accesstoken/issue", content);


                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        private void LoadConfigurationSection(ConfigurationSection strategyConfig)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.Sections["certificateStrategy"] != null)
                config.Sections.Remove("certificateStrategy");

            config.Sections.Add("certificateStrategy", strategyConfig);
            config.Save(ConfigurationSaveMode.Modified, true);
            ConfigurationManager.RefreshSection("certificateStrategy");
        }

        [TestInitialize]
        public void CleanConfig()
        {
            
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.Sections["certificateStrategy"] != null)
                config.Sections.Remove("certificateStrategy");
            config.Save(ConfigurationSaveMode.Modified, true);
            ConfigurationManager.RefreshSection("certificateStrategy");
           
        }
    }
}