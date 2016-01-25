using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Digst.OioIdws.OioWsTrust;
using Digst.OioIdws.Rest.Common;
using Newtonsoft.Json.Linq;

namespace Digst.OioIdws.Rest.Client
{
    public class OioIdwsClient
    {
        public OioIdwsClientSettings Settings { get; }

        public OioIdwsClient(OioIdwsClientSettings settings)
        {
            Settings = settings;
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (settings.ClientCertificate == null)
            {
                throw new ArgumentNullException(nameof(settings.ClientCertificate));
            }

            if (!settings.ClientCertificate.HasPrivateKey)
            {
                throw new ArgumentException("You must have access to the private key of the ClientCertificate", nameof(settings.ClientCertificate));
            }

            if (settings.SecurityTokenService == null)
            {
                throw new ArgumentNullException(nameof(settings.SecurityTokenService));
            }

            if (settings.SecurityTokenService.Certificate == null)
            {
                throw new ArgumentNullException(nameof(settings.SecurityTokenService.Certificate), "Certificate for the SecurityTokenService must be set");
            }
        }

        /// <summary>
        /// Creates a handler that takes care of issuing tokens and renewing tokens when expiring. 
        /// It can be used inside a HttpClient or similar that supports it.
        /// The handler is not thread-safe, but you can create multiple instances
        /// </summary>
        /// <returns></returns>
        public HttpMessageHandler CreateMessageHandler()
        {
            return new OioIdwsRequestHandler(this);
        }

        public GenericXmlSecurityToken GetSecurityToken()
        {
            var tokenService = new TokenIssuingService();
            return (GenericXmlSecurityToken) tokenService.RequestToken(new TokenIssuingRequestConfiguration
            {
                ClientCertificate = Settings.ClientCertificate,
                StsCertificate = Settings.SecurityTokenService.Certificate,
                StsEndpointAddress = Settings.SecurityTokenService.EndpointAddress.ToString(),
                TokenLifeTimeInMinutes = (int?)Settings.SecurityTokenService.TokenLifeTime.GetValueOrDefault().TotalMinutes,
                SendTimeout = Settings.SecurityTokenService.SendTimeout,
                WspEndpointId = Settings.AudienceUri.ToString(),
            });
        }

        public async Task<AccessToken> GetAccessTokenAsync(
            GenericXmlSecurityToken securityToken,
            CancellationToken cancellationToken)
        {
            if (securityToken == null)
            {
                throw new ArgumentNullException(nameof(securityToken));
            }

            string samlToken;

            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream))
                {
                    securityToken.TokenXml.WriteTo(writer);
                }

                samlToken = Convert.ToBase64String(stream.ToArray());
            }

            var requestHandler = new WebRequestHandler
            {
                ClientCertificates = {Settings.ClientCertificate}
            };

            var formFields = new Dictionary<string, string>
            {
                {"saml-token", samlToken}
            };

            if (Settings.DesiredAccessTokenExpiry.HasValue)
            {
                formFields["should-expire-in"] = ((int) Settings.DesiredAccessTokenExpiry.Value.TotalSeconds).ToString();
            }

            var client = new HttpClient(requestHandler);
            var response = await client.PostAsync(
                Settings.AccessTokenIssuerEndpoint, 
                new FormUrlEncodedContent(formFields),
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                //just find the first valid authenticate header we recognize
                var challenge = response.Headers.WwwAuthenticate
                    .Select(x => new
                    {
                        Type = AccessTokenTypeParser.FromString(x.Scheme),
                        x.Parameter
                    })
                    .FirstOrDefault(x => x.Type.HasValue);

                if (challenge != null)
                {
                    var parms = HttpHeaderUtils.ParseOAuthSchemeParameter(challenge.Parameter);
                    throw new OioIdwsChallengeException(
                        challenge.Type.Value,
                        parms["error"],
                        parms["error_description"],
                        $@"Got unexpected challenge while issuing access token from '{Settings.AccessTokenIssuerEndpoint}'
({response.StatusCode})': {parms["error"]} - {parms["error_description"]}");
                }

                throw new InvalidOperationException(
                    $@"Got unexpected response while issuing access token from '{Settings.AccessTokenIssuerEndpoint}'
{response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var jsonValue = JObject.Parse(json);

            var accessToken = new AccessToken
            {
                Value = (string) jsonValue["access_token"],
                ExpiresIn = TimeSpan.FromSeconds((int) jsonValue["expires_in"]),
                RetrievedAtUtc = DateTime.UtcNow,
                Type = ParseAccessTokenType((string) jsonValue["token_type"]),
                TypeString = (string) jsonValue["token_type"]
            };

            return accessToken;
        }

        private AccessTokenType ParseAccessTokenType(string str)
        {
            var type = AccessTokenTypeParser.FromString(str);

            if (!type.HasValue)
            {
                throw new InvalidOperationException($"Unknown access token type '{str}'");
            }

            return type.Value;
        }
    }
}
