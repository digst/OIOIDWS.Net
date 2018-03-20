using System;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;
using Digst.OioIdws.Rest.Server.AuthorizationServer;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;
using Digst.OioIdws.Rest.Server.Wsp;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using Owin;
using Digst.OioIdws.Common.Utils;

namespace Digst.OioIdws.Rest.Server.Test
{
    [TestClass]
    public class AsWspInMemoryTests
    {
        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task Authenticates_UseInMemoryTokenStore_IsAuthorized()
        {
            var accessToken = "dummy";
            var oioIdwsTokenKey = "token1";
            var token = new OioIdwsToken
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
                Claims = new[]
                {
                    new OioIdwsClaim
                    {
                        Value = "hans",
                        Type = "name"
                    },
                }
            };

            var storeMock = new Mock<ISecurityTokenStore>();
            storeMock
                .Setup(x => x.RetrieveTokenAsync(oioIdwsTokenKey))
                .ReturnsAsync(token);

            var tokenDataFormatMock = new Mock<ISecureDataFormat<AuthenticationProperties>>();
            tokenDataFormatMock
                .Setup(x => x.Unprotect(accessToken))
                .Returns(new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
                    Dictionary = {{"value", oioIdwsTokenKey}}
                });

            using (var server = TestServer.Create(app =>
            {
                app
                    .UseOioIdwsAuthentication(new OioIdwsAuthenticationOptions())
                    .UseOioIdwsAuthorizationService(new OioIdwsAuthorizationServiceOptions
                    {
                        AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                        IssuerAudiences = () => Task.FromResult(new [] { new IssuerAudiences("", "")}),
                        SecurityTokenStore = storeMock.Object,
                        TokenDataFormat = tokenDataFormatMock.Object,
                    })
                    .Use((context, next) =>
                    {
                        if (context.Request.Path == new PathString("/wsp"))
                        {
                            var name = ((ClaimsIdentity) context.Request.User.Identity).Claims.Single(x => x.Type == "name").Value;
                            context.Response.Write(name);
                        }
                        return Task.FromResult(0);
                    });
            }))
            {
                var client = server.HttpClient;
                var authHeader = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Authorization = authHeader;

                var response = await client.GetAsync("/wsp");
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                var str = await response.Content.ReadAsStringAsync();
                Assert.AreEqual("hans", str);
            }
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task Authenticates_TokenExpired_IsUnauthorized()
        {
            var accessToken = "dummy";
            var oioIdwsTokenKey = "token1";
            var token = new OioIdwsToken
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(-1),
                Claims = new[]
                {
                    new OioIdwsClaim
                    {
                        Value = "hans",
                        Type = "name"
                    },
                }
            };

            var storeMock = new Mock<ISecurityTokenStore>();
            storeMock
                .Setup(x => x.RetrieveTokenAsync(oioIdwsTokenKey))
                .ReturnsAsync(token);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(-1), 
                Dictionary = {{"value", oioIdwsTokenKey}}
            };

            var tokenDataFormatMock = new Mock<ISecureDataFormat<AuthenticationProperties>>();
            tokenDataFormatMock
                .Setup(x => x.Unprotect(accessToken))
                .Returns(authProperties);

            using (var server = TestServer.Create(app =>
            {
                app
                    .UseOioIdwsAuthentication(new OioIdwsAuthenticationOptions())
                    .UseOioIdwsAuthorizationService(new OioIdwsAuthorizationServiceOptions
                    {
                        AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                        IssuerAudiences = () => Task.FromResult(new[] { new IssuerAudiences("", "") }),
                        SecurityTokenStore = storeMock.Object,
                        TokenDataFormat = tokenDataFormatMock.Object,
                    })
                    .Use((context, next) =>
                    {
                        if (context.Request.Path == new PathString("/wsp"))
                        {
                            context.Response.StatusCode = context.Request.User != null ? 200 : 401;
                        }
                        return Task.FromResult(0);
                    });
            }))
            {
                var client = server.HttpClient;
                var authHeader = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Authorization = authHeader;

                {
                    var response = await client.GetAsync("/wsp");
                    Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
                    var errors = HttpHeaderUtils.ParseOAuthSchemeParameter(response.Headers.WwwAuthenticate.First().Parameter);
                    Assert.AreEqual(AuthenticationErrorCodes.InvalidToken, errors["error"]);
                    Assert.AreEqual("Token was expired", errors["error_description"]);
                }

                {
                    authProperties.ExpiresUtc = DateTimeOffset.Now.AddHours(1);

                    var response = await client.GetAsync("/wsp");
                    Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
                    var errors = HttpHeaderUtils.ParseOAuthSchemeParameter(response.Headers.WwwAuthenticate.First().Parameter);
                    Assert.AreEqual(AuthenticationErrorCodes.InvalidToken, errors["error"]);
                    Assert.AreEqual("Token was expired", errors["error_description"]);
                }
            }
        }
    }
}
