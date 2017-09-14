using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Rest.Common;
using Digst.OioIdws.Rest.Server.AuthorizationServer;
using Digst.OioIdws.Rest.Server.AuthorizationServer.Issuing;
using Digst.OioIdws.Test.Common;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;

namespace Digst.OioIdws.Rest.Server.Tests
{
    [TestClass]
    public class AccessTokenIssueEndpointIntegrationTests
    {
        private static IStsTokenService _stsTokenService;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            _stsTokenService = new StsTokenServiceCache(new StsTokenServiceConfiguration
            {
                ClientCertificate = CertificateUtil.GetCertificate("0E6DBCC6EFAAFF72E3F3D824E536381B26DEECF5"),
                StsCertificate = CertificateUtil.GetCertificate("d9f10c97aa647727adb64a349bb037c5c23c9a7a"),
                SendTimeout = TimeSpan.FromDays(1),
                StsEndpointAddress = "https://SecureTokenService.test-nemlog-in.dk/SecurityTokenService.svc",
                TokenLifeTimeInMinutes = 5,
                WspEndpointId = "https://wsp.oioidws-net.dk"
            });
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public async Task IssueAccessTokenFromStsToken_ValidateSuccess_ReturnsCorrectly()
        {
            var clientCertificate = CertificateUtil.GetCertificate("0E6DBCC6EFAAFF72E3F3D824E536381B26DEECF5");
            var issuerAudienceses = new[]
            {
                new IssuerAudiences("d9f10c97aa647727adb64a349bb037c5c23c9a7a", "sts cert")
                    .Audience(new Uri("https://wsp.oioidws-net.dk")),
            };
            await PerformValidationTestAsync(clientCertificate, issuerAudienceses, async (options, response) =>
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                var accessTokenJson = JObject.Parse(await response.Content.ReadAsStringAsync());
                var accessToken = (string)accessTokenJson["access_token"];

                var oioIdwsTokenKey = options.TokenDataFormat.Unprotect(accessToken).Dictionary["value"];
                var token = await options.SecurityTokenStore.RetrieveTokenAsync(oioIdwsTokenKey);

                Assert.IsNotNull(token);
                Assert.AreEqual("34051178", token.Claims.Single(x => x.Type == "dk:gov:saml:attribute:CvrNumberIdentifier").Value);
            });
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public async Task IssueAccessTokenFromStsToken_Success_ReturnsHolderOfKeyToken()
        {
            var clientCertificate = CertificateUtil.GetCertificate("0E6DBCC6EFAAFF72E3F3D824E536381B26DEECF5");
            var issuerAudiences = new [] { new IssuerAudiences("d9f10c97aa647727adb64a349bb037c5c23c9a7a", "sts cert")
                .Audience(new Uri("https://wsp.oioidws-net.dk"))};

            await PerformValidationTestAsync(clientCertificate, issuerAudiences, async (options, response) =>
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.IsNotNull(response.Content.Headers.ContentType);
                Assert.AreEqual("UTF-8", response.Content.Headers.ContentType.CharSet);
                Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);
                var accessToken = JObject.Parse(await response.Content.ReadAsStringAsync());

                var oioIdwsTokenKey = options.TokenDataFormat.Unprotect((string)accessToken["access_token"]).Value();

                var token = await options.SecurityTokenStore.RetrieveTokenAsync(oioIdwsTokenKey);
                Assert.IsNotNull(token);
                Assert.AreEqual(clientCertificate.Thumbprint?.ToLowerInvariant(), token.CertificateThumbprint);

                Assert.AreEqual("Holder-of-key", accessToken["token_type"]);
                Assert.AreEqual((int)options.AccessTokenExpiration.TotalSeconds, accessToken["expires_in"]);

            });
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public async Task IssueAccessTokenFromStsToken_IncorrectClientCertificate_ReturnsError()
        {
            //wrong certificate
            var clientCertificate = CertificateUtil.GetCertificate("d9f10c97aa647727adb64a349bb037c5c23c9a7a");
            var issuerAudienceses = new[]
            {
                new IssuerAudiences("d9f10c97aa647727adb64a349bb037c5c23c9a7a", "sts cert")
                    .Audience(new Uri("https://wsp.oioidws-net.dk")),
            };

            await PerformValidationTestAsync(clientCertificate, issuerAudienceses, (options, response) =>
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
                var errors = HttpHeaderUtils.ParseOAuthSchemeParameter(response.Headers.WwwAuthenticate.First().Parameter);
                Assert.AreEqual(AuthenticationErrorCodes.InvalidToken, errors["error"]);
                Assert.AreEqual("X509Certificate used as SubjectConfirmationData did not match the provided client certificate", errors["error_description"]);
                return Task.FromResult(0);
            });
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public async Task IssueAccessTokenFromStsToken_WrongAudience_ReturnsError()
        {
            var clientCertificate = CertificateUtil.GetCertificate("0E6DBCC6EFAAFF72E3F3D824E536381B26DEECF5");
            var issuerAudienceses = new[]
            {
                new IssuerAudiences("d9f10c97aa647727adb64a349bb037c5c23c9a7a", "sts cert")
                    .Audience(new Uri("https://wrongAudience")),
            };

            await PerformValidationTestAsync(clientCertificate, issuerAudienceses, (options, response) =>
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
                var errors = HttpHeaderUtils.ParseOAuthSchemeParameter(response.Headers.WwwAuthenticate.First().Parameter);
                Assert.AreEqual(AuthenticationErrorCodes.InvalidToken, errors["error"]);
                Assert.AreEqual("Audience was not known", errors["error_description"]);
                return Task.FromResult(0);
            });
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public async Task IssueAccessTokenFromStsToken_WrongIssuingCertificate_ReturnsError()
        {
            var clientCertificate = CertificateUtil.GetCertificate("0E6DBCC6EFAAFF72E3F3D824E536381B26DEECF5");
            var issuerAudienceses = new[]
            {
                //wrong issuing cert
                new IssuerAudiences("0E6DBCC6EFAAFF72E3F3D824E536381B26DEECF5", "sts cert")
                    .Audience(new Uri("https://wsp.oioidws-net.dk")),
            };


            await PerformValidationTestAsync(clientCertificate, issuerAudienceses, (options, response) =>
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
                var errors = HttpHeaderUtils.ParseOAuthSchemeParameter(response.Headers.WwwAuthenticate.First().Parameter);
                Assert.AreEqual(AuthenticationErrorCodes.InvalidToken, errors["error"]);
                Assert.AreEqual("Issuer certificate 'D9F10C97AA647727ADB64A349BB037C5C23C9A7A' was unknown", errors["error_description"]);
                return Task.FromResult(0);
            });
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public async Task IssueAccessTokenFromStsToken_ExpiredSecurityToken_ReturnsError()
        {
            var clientCertificate = CertificateUtil.GetCertificate("0E6DBCC6EFAAFF72E3F3D824E536381B26DEECF5");
            var issuerAudienceses = new[]
            {
                new IssuerAudiences("d9f10c97aa647727adb64a349bb037c5c23c9a7a", "sts cert")
                    .Audience(new Uri("https://wsp.oioidws-net.dk")),
            };

            var samlToken = File.ReadAllText(@"Resources\ExpiredSecurityToken.xml");

            await PerformValidationTestAsync(clientCertificate, issuerAudienceses, (options, response) =>
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
                var errors = HttpHeaderUtils.ParseOAuthSchemeParameter(response.Headers.WwwAuthenticate.First().Parameter);
                Assert.AreEqual(AuthenticationErrorCodes.InvalidToken, errors["error"]);
                Assert.AreEqual("The Token is expired", errors["error_description"]);
                return Task.FromResult(0);
            },
            samlToken: () => samlToken);
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public async Task IssueAccessTokenFromStsToken_ModifiedEncryptedSecurityToken_ReturnsError()
        {
            var clientCertificate = CertificateUtil.GetCertificate("0E6DBCC6EFAAFF72E3F3D824E536381B26DEECF5");
            var issuerAudienceses = new[]
            {
                new IssuerAudiences("d9f10c97aa647727adb64a349bb037c5c23c9a7a", "sts cert")
                    .Audience(new Uri("https://wsp.oioidws-net.dk")),
            };

            var loggerMock = new Mock<ILogger>();

            var loggerFactoryMock = new Mock<ILoggerFactory>();
            loggerFactoryMock
                .Setup(x => x.Create("Digst.OioIdws.Rest.Server.AuthorizationServer.OioIdwsAuthorizationServiceMiddleware"))
                .Returns(() => loggerMock.Object);

            var samlToken = GetSamlTokenXml();

            var xml = XElement.Parse(samlToken);

            var namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("e", "http://www.w3.org/2001/04/xmlenc#");
            var elm = xml.XPathSelectElement("//e:EncryptedKey/e:CipherData/e:CipherValue", namespaceManager);

            elm.Value = "modified";

            await PerformValidationTestAsync(clientCertificate, issuerAudienceses, (options, response) =>
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
                var errors = HttpHeaderUtils.ParseOAuthSchemeParameter(response.Headers.WwwAuthenticate.First().Parameter);
                Assert.AreEqual(AuthenticationErrorCodes.InvalidToken, errors["error"]);
                Assert.AreEqual("Token could not be parsed", errors["error_description"]);
                return Task.FromResult(0);
            },
            samlToken: () => xml.ToString(),
            setLogger: () => loggerFactoryMock.Object);

            loggerMock.Verify(v => v.WriteCore(TraceEventType.Error, 103, 
                It.Is<object>(x =>((IDictionary<string, object>)x)["ValidationError"].ToString() == "Token could not be parsed"), 
                It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
        }

        [TestMethod]
        [TestCategory(Constants.IntegrationTest)]
        public async Task IssueAccessTokenFromStsToken_ModifiedUnencryptedSecurityToken_ReturnsError()
        {
            var clientCertificate = CertificateUtil.GetCertificate("0E6DBCC6EFAAFF72E3F3D824E536381B26DEECF5");
            var issuerAudienceses = new[]
            {
                new IssuerAudiences("d9f10c97aa647727adb64a349bb037c5c23c9a7a", "sts cert")
                    .Audience(new Uri("https://wsp.oioidws-net.dk")),
            };

            var loggerMock = new Mock<ILogger>();

            var loggerFactoryMock = new Mock<ILoggerFactory>();
            loggerFactoryMock
                .Setup(x => x.Create("Digst.OioIdws.Rest.Server.AuthorizationServer.OioIdwsAuthorizationServiceMiddleware"))
                .Returns(() => loggerMock.Object);

            var decryptedSamlToken = GetDecryptedSamlToken();

            var xml = XElement.Parse(decryptedSamlToken);

            var namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("a", "urn:oasis:names:tc:SAML:2.0:assertion");
            var elm = xml.XPathSelectElement("//a:Conditions", namespaceManager);
            var attr = elm.Attribute("NotOnOrAfter");
            attr.Value = DateTime.UtcNow.AddHours(1).ToString("O");

            await PerformValidationTestAsync(clientCertificate, issuerAudienceses, async (options, response) =>
            {
                var str = await response.Content.ReadAsStringAsync();
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
                var errors = HttpHeaderUtils.ParseOAuthSchemeParameter(response.Headers.WwwAuthenticate.First().Parameter);
                Assert.AreEqual(AuthenticationErrorCodes.InvalidToken, errors["error"]);
                Assert.AreEqual("Token could not be parsed", errors["error_description"]);
                
            },
            samlToken: () => xml.ToString(),
            setLogger: () => loggerFactoryMock.Object);

            loggerMock.Verify(v => v.WriteCore(TraceEventType.Error, 103, 
                It.Is<object>(x =>((IDictionary<string, object>)x)["ValidationError"].ToString() == "Token could not be parsed"),
                It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
        }

        private string GetDecryptedSamlToken()
        {
            var encryptedSamlToken = GetSamlTokenXml();

            var decryptedSamlToken = "";

            using (var reader = new StringReader(encryptedSamlToken))
            {
                using (var xmlReader = XmlReader.Create(reader))
                {
                    var samlHandler = new Saml2SecurityTokenHandler
                    {
                        Configuration = new SecurityTokenHandlerConfiguration
                        {
                            ServiceTokenResolver = new X509CertificateStoreTokenResolver()
                        }
                    };

                    var token = samlHandler.ReadToken(xmlReader);

                    using (var writer = new StringWriter())
                    {
                        using (var xmlWriter = XmlWriter.Create(writer))
                        {
                            samlHandler.WriteToken(xmlWriter, token);
                        }
                        decryptedSamlToken = writer.GetStringBuilder().ToString();
                    }
                }
            }
            return decryptedSamlToken;
        }

        async Task PerformValidationTestAsync(
            X509Certificate2 clientCertificate, 
            IEnumerable<IssuerAudiences> issuerAudiences, 
            Func<OioIdwsAuthorizationServiceOptions, HttpResponseMessage, Task> assert,
            Func<string> samlToken = null,
            Func<ILoggerFactory> setLogger = null)
        {
            var requestSamlToken = samlToken != null ? samlToken() : GetSamlTokenXml();

            var options = new OioIdwsAuthorizationServiceOptions
            {
                AccessTokenIssuerPath = new PathString("/accesstoken/issue"),
                AccessTokenRetrievalPath = new PathString("/accesstoken"),
                IssuerAudiences = () => Task.FromResult(issuerAudiences.ToArray()),
                CertificateValidator = X509CertificateValidator.None //no reason for tests to require proper certificate validation
            };

            using (var server = TestServerWithClientCertificate.Create(() => clientCertificate, app =>
            {
                app.UseOioIdwsAuthorizationService(options);

                if(setLogger != null)
                {
                    app.SetLoggerFactory(setLogger());
                }
            }))
            {
                server.BaseAddress = new Uri("https://localhost/");

                var response = await server.HttpClient.PostAsync("/accesstoken/issue",
                            new FormUrlEncodedContent(new[]
                            {
                                new KeyValuePair<string, string>("saml-token", Utils.ToBase64(requestSamlToken)),
                            }));

                await assert(options, response);
            }
        }

        private string GetSamlTokenXml()
        {
            var securityToken = (GenericXmlSecurityToken)_stsTokenService.GetToken();

            return securityToken.TokenXml.OuterXml;
        }
    }
}
