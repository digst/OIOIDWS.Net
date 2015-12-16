using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Net.Http;
using System.Text;
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

            var sb = new StringBuilder();

            using (var writer = XmlWriter.Create(sb))
            {
                securityToken.TokenXml.WriteTo(writer);
            }


            //todo: Detect token type, only add client certificate if holder-of-key
            var requestHandler = new WebRequestHandler
            {
                ClientCertificates = {Settings.ClientCertificate}
            };

            var client = new HttpClient(requestHandler);
            var response = await client.PostAsync(
                Settings.IssueAccessTokenEndpoint, 
                new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("saml-token", sb.ToString())
                }),
                cancellationToken);

            //todo handle errors related to security token

            var json = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            var jsonValue = JObject.Parse(json);

            return new AccessToken
            {
                Value = (string) jsonValue["access_token"],
                ExpiresIn = TimeSpan.FromSeconds((int) jsonValue["expires_in"]),
                RetrievedAtUtc = DateTime.UtcNow,
                Type = ParseAccessTokenType((string) jsonValue["token_type"]),
                TypeString = (string) jsonValue["token_type"]
            };
        }

        private AccessTokenType ParseAccessTokenType(string type)
        {
            switch (type.ToLowerInvariant())
            {
                case "bearer": return AccessTokenType.Bearer;
                case "holder-of-key": return AccessTokenType.HolderOfKey;
                default: throw new InvalidOperationException($"Unknown access token type '{type}'");
            }
        }
    }
}
