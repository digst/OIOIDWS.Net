using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Digst.OioIdws.Rest.Common;
using Newtonsoft.Json.Linq;

namespace Digst.OioIdws.Rest.Client.AccessToken
{
    /// <summary>
    /// <see cref="IAccessTokenService"/>
    /// </summary>
    internal class AccessTokenService : IAccessTokenService
    {
        // It is an internal class. So we just reuse the client settings object instead of creating a new.
        private readonly OioIdwsClientSettings _settings;

        /// <summary>
        /// // It is an internal class and it is assumed that the settings has been verified by the client <see cref="OioIdwsClient"/>
        /// </summary>
        internal AccessTokenService(OioIdwsClientSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            _settings = settings;
        }

        public async Task<AccessToken> GetTokenAsync(GenericXmlSecurityToken securityToken, CancellationToken cancellationToken)
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
                ClientCertificates = {_settings.ClientCertificate}
            };

            var formFields = new Dictionary<string, string>
            {
                {"saml-token", samlToken}
            };

            if (_settings.DesiredAccessTokenExpiry.HasValue)
            {
                formFields["should-expire-in"] =
                    ((int) _settings.DesiredAccessTokenExpiry.Value.TotalSeconds).ToString();
            }

            var client = new HttpClient(requestHandler);
            var response = await client.PostAsync(
                _settings.AccessTokenIssuerEndpoint,
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
                        $@"Got unexpected challenge while issuing access token from '{
                                _settings.AccessTokenIssuerEndpoint
                            }'
({response.StatusCode})': {parms["error"]} - {parms["error_description"]}");
                }

                throw new InvalidOperationException(
                    $@"Got unexpected response while issuing access token from '{_settings.AccessTokenIssuerEndpoint}'
{response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var jsonValue = JObject.Parse(json);

            var accessToken = new Rest.Client.AccessToken.AccessToken
            {
                Value = (string) jsonValue["access_token"],
                ExpiresIn = TimeSpan.FromSeconds((int) jsonValue["expires_in"]),
                RetrievedAtUtc = DateTime.UtcNow,
                Type = ParseAccessTokenType((string) jsonValue["token_type"])
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