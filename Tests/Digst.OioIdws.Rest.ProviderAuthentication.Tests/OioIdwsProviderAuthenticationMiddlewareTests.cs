using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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
        public async Task Invoke()
        {
            var accessToken = "token123";

            var tokenProviderMock = new Mock<ITokenProvider>();

            using (var server = TestServer.Create(app =>
            {
                app.Use<OioIdwsProviderAuthenticationMiddleware>(new OioIdwsProviderAuthenticationOptions
                {
                }, tokenProviderMock.Object)
                .Use(async (context, next) =>
                {
                    await context.Response.WriteAsync("innerresponse");
                });
            }))
            {
                var client = server.HttpClient;
                var authHeader = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Authorization = authHeader;
                var response = await client.PostAsync("/securedendpoint", new StringContent("payload"));

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                var text = await response.Content.ReadAsStringAsync();
                Assert.AreEqual("innerresponse", text);
            }

            tokenProviderMock.Verify(x => x.RetrieveTokenAsync(accessToken), Times.Once);
        }
    }
}
