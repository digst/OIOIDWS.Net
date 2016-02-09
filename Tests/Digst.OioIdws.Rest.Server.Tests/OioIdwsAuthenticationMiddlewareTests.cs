using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;
using Digst.OioIdws.Rest.Server.Wsp;
using Digst.OioIdws.Test.Common;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Owin;

namespace Digst.OioIdws.Rest.Server.Tests
{
    [TestClass]
    public class OioIdwsAuthenticationMiddlewareTests
    {
        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task Authenticate_Success_IsLoggedIn()
        {
            await ProcessAuthenticateAsync(
                "bearer",
                async (context, next) =>
                {
                    await context.Response.WriteAsync(context.Request.User.Identity.Name);
                },
                async response =>
                {
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                    var text = await response.Content.ReadAsStringAsync();
                    Assert.AreEqual("MYNAME", text);
                },
                identityFunc: token => new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "MYNAME"), }));
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task Authenticate_Unsuccessful_IsNotLoggedIn()
        {
            await ProcessAuthenticateAsync(
                "bearer",
                async (context, next) =>
                {
                    await context.Response.WriteAsync((context.Request.User != null && context.Request.User.Identity.IsAuthenticated).ToString());
                },
                async response =>
                {
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                    var text = await response.Content.ReadAsStringAsync();
                    Assert.AreEqual("False", text);
                },
                identityFunc: token => new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "MYNAME"), }));
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task Authenticate_UnknownToken_UnauthorizedByService()
        {
            await ProcessAuthenticateAsync(
                "bearer",
                (context, next) =>
                {
                    //this service requires auth, therefore denies
                    context.Response.StatusCode = context.Request.User == null ? 401 : 200;
                    return Task.FromResult(0);
                },
                response =>
                {
                    Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
                    var errors = HttpHeaderUtils.ParseOAuthSchemeParameter(response.Headers.WwwAuthenticate.First().Parameter);
                    Assert.AreEqual(AuthenticationErrorCodes.InvalidToken, errors["error"]);
                    Assert.AreEqual("Token information could not be retrieved from the Authorization Server. The access token might be unknown or expired", errors["error_description"]);
                    return Task.FromResult(0);
                },
                tokenFunc: () => null);
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task Authenticate_WrongTokenType_UnauthorizedByService()
        {
            await ProcessAuthenticateAsync(
                "bearer",
                (context, next) =>
                {
                    //this service requires auth, therefore denies
                    context.Response.StatusCode = context.Request.User == null ? 401 : 200;
                    return Task.FromResult(0);
                },
                response =>
                {
                    Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
                    var errors = HttpHeaderUtils.ParseOAuthSchemeParameter(response.Headers.WwwAuthenticate.First().Parameter);
                    Assert.AreEqual(AuthenticationErrorCodes.InvalidToken, errors["error"]);
                    Assert.AreEqual("Authentication scheme was not valid", errors["error_description"]);
                    return Task.FromResult(0);
                },
                tokenFunc: () => new OioIdwsToken {Type = AccessTokenType.HolderOfKey});
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task Authenticate_InvalidHolderOfKeyCertificate_UnauthorizedByService()
        {
            await ProcessAuthenticateAsync(
                "holder-of-key",
                (context, next) =>
                {
                    //this service requires auth, therefore denies
                    context.Response.StatusCode = context.Request.User == null ? 401 : 200;
                    return Task.FromResult(0);
                },
                response =>
                {
                    Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
                    var errors = HttpHeaderUtils.ParseOAuthSchemeParameter(response.Headers.WwwAuthenticate.First().Parameter);
                    Assert.AreEqual(AuthenticationErrorCodes.InvalidToken, errors["error"]);
                    Assert.AreEqual("A valid certificate must be presented when presenting a Holder-of-key token", errors["error_description"]);
                    return Task.FromResult(0);
                },
                tokenFunc: () => new OioIdwsToken { Type = AccessTokenType.HolderOfKey, CertificateThumbprint = "correct cert thumbprint" },
                certificateFunc: () => CertificateUtil.GetCertificate("2e7a061560fa2c5e141a634dc1767dacaeec8d12"));
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task Authenticate_CorrectHolderOfKeyCertificate_BuildsPrincipal()
        {
            await ProcessAuthenticateAsync(
                "Holder-of-key",
                (context, next) =>
                {
                    //this service requires auth, therefore denies if nu user
                    context.Response.StatusCode = context.Request.User == null ? 401 : 200;
                    return Task.FromResult(0);
                },
                response =>
                {
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                    return Task.FromResult(0);
                },
                tokenFunc: () => new OioIdwsToken { Type = AccessTokenType.HolderOfKey, CertificateThumbprint = "2e7a061560fa2c5e141a634dc1767dacaeec8d12" },
                certificateFunc: () => CertificateUtil.GetCertificate("2e7a061560fa2c5e141a634dc1767dacaeec8d12"),
                identityFunc: token => token != null ? new ClaimsIdentity() : null /*returns no identity if no token*/);
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task Authenticate_InsufficientScope_ReturnsRequiredScope()
        {
            await ProcessAuthenticateAsync(
                "bearer",
                (context, next) =>
                {
                    //this service requires auth, therefore denies if nu user
                    var authProperties = new AuthenticationProperties();
                    authProperties.Dictionary[AuthenticationErrorCodes.InsufficentScope] = "scope1";
                    context.Authentication.Challenge(authProperties);
                    return Task.FromResult(0);
                },
                response =>
                {
                    Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
                    var errors = HttpHeaderUtils.ParseOAuthSchemeParameter(response.Headers.WwwAuthenticate.First().Parameter);
                    Assert.AreEqual(AuthenticationErrorCodes.InsufficentScope, errors["error"]);
                    Assert.AreEqual("scope1", errors["scope"]);
                    return Task.FromResult(0);
                });
        }

        private async Task ProcessAuthenticateAsync(
            string tokenType,
            Func<IOwinContext, Func<Task>, Task> serviceUnderTest, 
            Func<HttpResponseMessage, Task> assert, 
            Func<OioIdwsToken> tokenFunc = null,
            Func<OioIdwsToken, ClaimsIdentity> identityFunc = null,
            Func<X509Certificate2> certificateFunc = null)
        {
            var accessToken = "token123";
            var token = tokenFunc != null ? tokenFunc() : new OioIdwsToken();
            
            var tokenProviderMock = new Mock<ITokenProvider>();
            tokenProviderMock.Setup(x => x.RetrieveTokenAsync(accessToken))
                .ReturnsAsync(new RetrieveTokenResult(token));

            var identityBuilderMock = new Mock<IIdentityBuilder>();
            identityBuilderMock.Setup(x => x.BuildIdentityAsync(token))
                .Returns((OioIdwsToken t) => Task.FromResult(identityFunc?.Invoke(t)));

            using (var server = TestServerWithClientCertificate.Create(certificateFunc, app =>
            {
                app
                    .UseOioIdwsAuthentication(new OioIdwsAuthenticationOptions
                    {
                        TokenProvider = tokenProviderMock.Object,
                        IdentityBuilder = identityBuilderMock.Object
                    })
                    .Use((context, next) => serviceUnderTest(context, next));
            }))
            {
                var client = server.HttpClient;
                var authHeader = new AuthenticationHeaderValue(tokenType, accessToken);
                client.DefaultRequestHeaders.Authorization = authHeader;
                var response = await client.PostAsync("/securedendpoint", new StringContent("payload"));

                await assert(response);
            }

            tokenProviderMock.Verify(x => x.RetrieveTokenAsync(accessToken), Times.Once);
        }
    }
}
