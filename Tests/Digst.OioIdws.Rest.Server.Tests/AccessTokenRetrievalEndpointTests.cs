using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;
using Digst.OioIdws.Rest.Server.AuthorizationServer;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;
using Digst.OioIdws.Test.Common;
using Microsoft.Owin;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Owin;

namespace Digst.OioIdws.Rest.Server.Tests
{
    [TestClass]
    public class AccessTokenRetrievalEndpointTests
    {
        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task IssueAccessToken_Success_ReturnsCorrectly()
        {
            var accessTokenValue = "accesstoken1";
            var token = new OioIdwsToken
            {
                Type = AccessTokenType.Bearer,
                ValidUntilUtc = DateTime.UtcNow.AddMinutes(1),
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
                .Setup(x => x.RetrieveTokenAsync(accessTokenValue))
                .ReturnsAsync(token);

            var options = new OioIdwsAuthorizationServiceOptions
            {
                AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                AccessTokenRetrievalPath = new PathString("/accesstoken"),
                IssuerAudiences = () => Task.FromResult(new IssuerAudiences[0]),
                SecurityTokenStore = tokenStoreMock.Object
            };

            using (var server = TestServer.Create(app =>
            {
                app.Use<OioIdwsAuthorizationServiceMiddleware>(app, options);
            }))
            {
                var response = await server.HttpClient.GetAsync($"/accesstoken?{accessTokenValue}");
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);
                var responseToken = JsonConvert.DeserializeObject<OioIdwsToken>(await response.Content.ReadAsStringAsync());

                Assert.AreEqual(token.Type, responseToken.Type);
                Assert.AreEqual(token.ValidUntilUtc, responseToken.ValidUntilUtc);
                Assert.AreEqual(token.Claims.Count, responseToken.Claims.Count);
                Assert.AreEqual(token.Claims.ElementAt(0).Type, responseToken.Claims.ElementAt(0).Type);
                Assert.AreEqual(token.Claims.ElementAt(0).Value, responseToken.Claims.ElementAt(0).Value);
            }
        }
    }
}
