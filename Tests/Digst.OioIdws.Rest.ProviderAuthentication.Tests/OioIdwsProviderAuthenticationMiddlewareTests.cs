using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;
using Digst.OioIdws.Test.Common;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Owin;

namespace Digst.OioIdws.Rest.ProviderAuthentication.Tests
{
    [TestClass]
    public class OioIdwsProviderAuthenticationMiddlewareTests
    {
        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task Authenticate_Success_IsLoggedIn()
        {
            var accessToken = "token123";
            var token = new OioIdwsToken();
            var principalResult = new ClaimsPrincipal(new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, "MYNAME"),}));

            var tokenProviderMock = new Mock<ITokenProvider>();
            tokenProviderMock.Setup(x => x.RetrieveTokenAsync(accessToken))
                .ReturnsAsync(token);

            var principalBuilderMock = new Mock<IPrincipalBuilder>();
            principalBuilderMock.Setup(x => x.BuildPrincipalAsync(token))
                .ReturnsAsync(principalResult);

            using (var server = TestServer.Create(app =>
            {
                app.Use<OioIdwsProviderAuthenticationMiddleware>(new OioIdwsProviderAuthenticationOptions
                {
                }, tokenProviderMock.Object, principalBuilderMock.Object)
                .Use(async (context, next) =>
                {
                    await context.Response.WriteAsync(context.Request.User.Identity.Name);
                });
            }))
            {
                var client = server.HttpClient;
                var authHeader = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Authorization = authHeader;
                var response = await client.PostAsync("/securedendpoint", new StringContent("payload"));

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                var text = await response.Content.ReadAsStringAsync();
                Assert.AreEqual("MYNAME", text);
            }

            tokenProviderMock.Verify(x => x.RetrieveTokenAsync(accessToken), Times.Once);
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task Authenticate_Unsuccessful_IsNotLoggedIn()
        {
            var accessToken = "token123";
            var token = new OioIdwsToken();
            var principalResult = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "MYNAME"), }));

            var tokenProviderMock = new Mock<ITokenProvider>();
            tokenProviderMock.Setup(x => x.RetrieveTokenAsync(accessToken))
                .ReturnsAsync(token);

            var principalBuilderMock = new Mock<IPrincipalBuilder>();
            principalBuilderMock.Setup(x => x.BuildPrincipalAsync(token))
                .ReturnsAsync(principalResult);

            using (var server = TestServer.Create(app =>
            {
                app.Use<OioIdwsProviderAuthenticationMiddleware>(new OioIdwsProviderAuthenticationOptions
                {
                }, tokenProviderMock.Object, principalBuilderMock.Object)
                .Use(async (context, next) =>
                {
                    await context.Response.WriteAsync((context.Request.User != null && context.Request.User.Identity.IsAuthenticated).ToString());
                });
            }))
            {
                var client = server.HttpClient;
                var authHeader = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Authorization = authHeader;
                var response = await client.PostAsync("/securedendpoint", new StringContent("payload"));

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                var text = await response.Content.ReadAsStringAsync();
                Assert.AreEqual("False", text);
            }

            tokenProviderMock.Verify(x => x.RetrieveTokenAsync(accessToken), Times.Once);
        }
    }
}
