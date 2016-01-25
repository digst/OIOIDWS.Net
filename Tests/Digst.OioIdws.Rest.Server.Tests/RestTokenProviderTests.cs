using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;
using Digst.OioIdws.Rest.Server.Wsp;
using Digst.OioIdws.Test.Common;
using Microsoft.Owin.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Digst.OioIdws.Rest.Server.Tests
{
    [TestClass]
    public class RestTokenProviderTests
    {
        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task RetrieveToken_IsInCache_ReturnedFromCache()
        {
            var accessToken = "token1";
            var token = new OioIdwsToken
            {
                CertificateThumbprint = "cert1",
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
            };

            var cacheMock = new Mock<ITokenCache>();
            cacheMock
                .Setup(x => x.RetrieveAsync(accessToken))
                .ReturnsAsync(token);

            Func<HttpClient> clientFactory = () => new HttpClient();
            var sut = new RestTokenProvider(clientFactory, cacheMock.Object);
            sut.Initialize(null, null, new Mock<ILogger>().Object);
            var tokenResult = await sut.RetrieveTokenAsync(accessToken);

            Assert.IsTrue(tokenResult.Success);
            Assert.AreEqual(token.CertificateThumbprint, tokenResult.Result.CertificateThumbprint);
            cacheMock.Verify(x => x.RetrieveAsync(accessToken));
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task RetrieveToken_IsInCacheButExpired_ReturnedFromRestInvocation()
        {
            var accessToken = "token1";
            var expiredToken = new OioIdwsToken
            {
                CertificateThumbprint = "cert1",
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(-1),
            };

            var validToken = new OioIdwsToken
            {
                CertificateThumbprint = "cert2",
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
            };

            var cacheMock = new Mock<ITokenCache>();
            cacheMock
                .Setup(x => x.RetrieveAsync(accessToken))
                .ReturnsAsync(expiredToken);

            var handler = new TokenHandler(validToken);

            Func<HttpClient> clientFactory = () => new HttpClient(handler)
            {
                BaseAddress = new Uri("http://dummy")
            };

            var sut = new RestTokenProvider(clientFactory, cacheMock.Object);
            sut.Initialize(null, null, new Mock<ILogger>().Object);
            var tokenResult = await sut.RetrieveTokenAsync(accessToken);

            Assert.IsTrue(tokenResult.Success);
            Assert.AreEqual(validToken.CertificateThumbprint, tokenResult.Result.CertificateThumbprint);
            cacheMock.Verify(x => x.RetrieveAsync(accessToken));
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task RetrieveToken_UnknownAccessToken_ReturnsNothing()
        {
            var accessToken = "token1";

            var cacheMock = new Mock<ITokenCache>();

            var handler = new TokenHandler(null); //no token returned, i.e 404

            Func<HttpClient> clientFactory = () => new HttpClient(handler)
            {
                BaseAddress = new Uri("http://dummy")
            };

            var sut = new RestTokenProvider(clientFactory, cacheMock.Object);
            sut.Initialize(null, null, new Mock<ILogger>().Object);
            var tokenResult = await sut.RetrieveTokenAsync(accessToken);

            Assert.IsFalse(tokenResult.Success);
            Assert.IsFalse(tokenResult.Expired);
            Assert.IsNull(tokenResult.Result);
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task RetrieveToken_ExpiredAccessToken_ReturnsExpired()
        {
            var accessToken = "token1";

            var cacheMock = new Mock<ITokenCache>();

            var handler = TokenHandler.ExpiredToken();

            Func<HttpClient> clientFactory = () => new HttpClient(handler)
            {
                BaseAddress = new Uri("http://dummy")
            };

            var sut = new RestTokenProvider(clientFactory, cacheMock.Object);
            sut.Initialize(null, null, new Mock<ILogger>().Object);
            var tokenResult = await sut.RetrieveTokenAsync(accessToken);

            Assert.IsFalse(tokenResult.Success);
            Assert.IsTrue(tokenResult.Expired);
        }

        class TokenHandler : HttpMessageHandler
        {
            private readonly OioIdwsToken _token;
            private bool _expired;

            public TokenHandler(OioIdwsToken token)
            {
                _token = token;
            }

            private TokenHandler()
            {
                
            }

            public static TokenHandler ExpiredToken()
            {
                return new TokenHandler
                {
                    _expired = true
                };
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (_expired)
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        Content = WriteJsonToStreamContent(new JObject
                        {
                            new JProperty("expired", 1)
                        })
                    });
                }

                if (_token == null)
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
                }

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = WriteJsonToStreamContent(_token)
                });
            }

            private static StreamContent WriteJsonToStreamContent(object obj)
            {
                //stream must not be closed, since it's used by the response handling
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                var jsonWriter = new JsonTextWriter(writer);

                new JsonSerializer().Serialize(jsonWriter, obj);
                jsonWriter.Flush();
                writer.Flush();
                stream.Position = 0;

                return new StreamContent(stream);
            }
        }
    }
}