using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Server.TokenStorage;
using Microsoft.Owin;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Owin;

namespace Digst.OioIdws.Rest.Server.Tests
{
    [TestClass]
    public class AsWspInMemoryTests
    {
        [TestMethod]
        public async Task CanUseInMemoryStore()
        {
            var accessToken = "token1";
            var token = new OioIdwsToken
            {
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
                .Setup(x => x.RetrieveTokenAsync(accessToken))
                .ReturnsAsync(token);

            using (var server = TestServer.Create(app =>
            {
                app
                    .UseOioIdwsAuthentication(new OioIdwsAuthenticationOptions())
                    .UseOioIdwsAuthorizationService(new OioIdwsAuthorizationServiceOptions
                    {
                        AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                        IssuerAudiences = () => Task.FromResult(new [] { new IssuerAudiences("", "")}),
                        SecurityTokenStore = storeMock.Object
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
    }
}
