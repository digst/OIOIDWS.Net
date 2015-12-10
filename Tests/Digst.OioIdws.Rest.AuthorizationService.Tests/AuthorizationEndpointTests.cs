using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using Owin;

namespace Digst.OioIdws.Rest.AuthorizationService.Tests
{
    [TestClass]
    public class AuthorizationEndpointTests
    {
        [TestMethod]
        public async Task IssueAccessToken_Success_ReturnsCorrectly()
        {
            var requestSamlToken = Utils.ToBase64("mit token");
            var accessTokenValue = "accesstoken1";

            var authorizationServiceMock = new Mock<IAccessTokenGenerator>();
            var tokenStoreMock = new Mock<ITokenStore>();

            authorizationServiceMock
                .Setup(x => x.GenerateAccesstoken())
                .Returns(accessTokenValue);

            var options = new OioIdwsAuthorizationServiceOptions
            {
                IssueAccessTokenEndpoint = "/authorize"
            };

            using (var server = TestServer.Create(app =>
            {
                app.Use<OioIdwsAuthorizationServiceMiddleware>(new OioIdwsAuthorizationServiceOptions
                {
                    IssueAccessTokenEndpoint = "/authorize",
                }, authorizationServiceMock.Object, tokenStoreMock.Object);
            }))
            {
                var response = await server.HttpClient.PostAsync("/authorize",
                            new FormUrlEncodedContent(new[]
                            {new KeyValuePair<string, string>("saml-token", requestSamlToken),}));

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.IsNotNull(response.Content.Headers.ContentType);
                Assert.AreEqual("UTF-8", response.Content.Headers.ContentType.CharSet);
                Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);

                var accessToken = JObject.Parse(await response.Content.ReadAsStringAsync());
                Assert.AreEqual(accessTokenValue, accessToken["access_token"]);
                Assert.AreEqual("Bearer", accessToken["token_type"]);
                Assert.AreEqual(options.AccessTokenExpiration.TotalSeconds.ToString(CultureInfo.InvariantCulture), accessToken["expires_in"]);
            }

            authorizationServiceMock.Verify(x => x.GenerateAccesstoken(), Times.Once);
        }

        [TestMethod]
        public async Task IssueAccessToken_OtherEndpoint_PassesThrough()
        {
            using (var server = TestServer.Create(app =>
            {
                app
                    .UseOioIdwsAuthorizationService(new OioIdwsAuthorizationServiceOptions
                    {
                        IssueAccessTokenEndpoint = "/authorize"
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
        public async Task IssueAccessToken_InvalidRequest_ReturnsUnauthorized()
        {
            var authorizationServiceMock = new Mock<IAccessTokenGenerator>();
            var tokenStoreMock = new Mock<ITokenStore>();

            using (var server = TestServer.Create(app =>
            {
                app.Use<OioIdwsAuthorizationServiceMiddleware>(new OioIdwsAuthorizationServiceOptions
                {
                    IssueAccessTokenEndpoint = "/authorize"
                }, authorizationServiceMock.Object, tokenStoreMock.Object);
            }))
            {
                var response = await server.CreateRequest("/authorize").PostAsync();
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);

                var authHeader = response.Headers.WwwAuthenticate.Single(x => x.Scheme == "Bearer");
                var bearerParameters = HttpHeaderUtils.ParseBearerSchemeParameter(authHeader.Parameter);
                Assert.AreEqual(AuthenticationErrorCodes.InvalidRequest, bearerParameters["error"]);
                Assert.AreEqual(AuthenticationErrorCodes.Descriptions.SamlTokenMissing, bearerParameters["error_description"]);
            }
        }
    }
}