using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Rest.Server.AuthorizationServer;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;
using Digst.OioIdws.Test.Common;
using Microsoft.Owin;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Owin;

namespace Digst.OioIdws.Rest.Server.Tests
{
    [TestClass]
    public class AccessTokenIssueEndpointIntegrationTests
    {
        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public async Task IssueAccessTokenFromStsToken_ValidateSuccess_ReturnsCorrectly()
        {
            var clientCertificate = CertificateUtil.GetCertificate("0919ed32cf8758a002b39c10352be7dcccf1222a");
            var requestSamlToken = GetSamlTokenXml();

            var tokenStore = new InMemorySecurityTokenStore();

            var options = new OioIdwsAuthorizationServiceOptions
            {
                AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                AccessTokenRetrievalPath = new PathString("/accesstoken"),
                IssuerAudiences = () => Task.FromResult(new[]
                {
                    new IssuerAudiences("2e7a061560fa2c5e141a634dc1767dacaeec8d12", "sts cert")
                        .Audience(new Uri("https://wsp.itcrew.dk")),
                }),
                SecurityTokenStore = tokenStore
            };
            using (var server = TestServerWithClientCertificate.Create(clientCertificate, app =>
            {
                app.UseOioIdwsAuthorizationService(options);
            }))
            {
                server.BaseAddress = new Uri("https://localhost/");

                var response = await server.HttpClient.PostAsync("/accesstoken/issue",
                            new FormUrlEncodedContent(new[]
                            {new KeyValuePair<string, string>("saml-token", requestSamlToken)}));

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                var accessTokenJson = JObject.Parse(await response.Content.ReadAsStringAsync());
                var accessToken = (string)accessTokenJson["access_token"];

                var token = await tokenStore.RetrieveTokenAsync(accessToken);

                Assert.IsNotNull(token);
                Assert.AreEqual("34051178", token.Claims.Single(x => x.Type == "dk:gov:saml:attribute:CvrNumberIdentifier").Value);
            }
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task IssueAccessTokenFromStsToken_Success_ReturnsHolderOfKeyToken()
        {
            var clientCertificate = CertificateUtil.GetCertificate("0919ed32cf8758a002b39c10352be7dcccf1222a");
            var requestSamlToken = GetSamlTokenXml();

            var options = new OioIdwsAuthorizationServiceOptions
            {
                AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                AccessTokenRetrievalPath = new PathString("/accesstoken"),
                IssuerAudiences = () => Task.FromResult(new[]
                {
                    new IssuerAudiences("2e7a061560fa2c5e141a634dc1767dacaeec8d12", "sts cert")
                        .Audience(new Uri("https://wsp.itcrew.dk")),
                }),
            };
            using (var server = TestServerWithClientCertificate.Create(clientCertificate, app =>
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
                var accessToken = JObject.Parse(await response.Content.ReadAsStringAsync());

                var token = await options.SecurityTokenStore.RetrieveTokenAsync((string) accessToken["access_token"]);
                Assert.IsNotNull(token);
                Assert.AreEqual(clientCertificate.Thumbprint?.ToLowerInvariant(), token.CertificateThumbprint);

                Assert.AreEqual("holder-of-key", accessToken["token_type"]);
                Assert.AreEqual((int)options.AccessTokenExpiration.TotalSeconds, accessToken["expires_in"]);
            }
        }

        //todo: test that ensures correct holder-of-key certificate is used

        private string GetSamlTokenXml()
        {
            var tokenService = new TokenIssuingService();
            var securityToken = (GenericXmlSecurityToken)tokenService.RequestToken(new TokenIssuingRequestConfiguration
            {
                ClientCertificate = CertificateUtil.GetCertificate("0919ed32cf8758a002b39c10352be7dcccf1222a"),
                StsCertificate = CertificateUtil.GetCertificate("2e7a061560fa2c5e141a634dc1767dacaeec8d12"),
                SendTimeout = TimeSpan.FromDays(1),
                StsEndpointAddress = "https://SecureTokenService.test-nemlog-in.dk/SecurityTokenService.svc",
                TokenLifeTimeInMinutes = 5,
                WspEndpointId = "https://wsp.itcrew.dk"
            });

            return securityToken.TokenXml.OuterXml;
        }
    }
}
