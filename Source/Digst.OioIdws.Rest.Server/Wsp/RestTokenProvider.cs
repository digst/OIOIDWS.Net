using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;
using Microsoft.Owin.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Owin;

namespace Digst.OioIdws.Rest.Server.Wsp
{
    public class RestTokenProvider : ITokenProvider
    {
        private readonly Func<HttpClient> _clientFactory;
        private readonly ITokenCache _tokenCache;
        private ILogger _logger;

        /// <summary>
        /// Uses a web request towards the Authorization Server for retrieving token information.
        /// Uses <see cref="InMemoryTokenCache"/> for caching tokens
        /// </summary>
        /// <param name="accessTokenRetrievalEndpoint">Path on the AuthorizationService server where token information can be resolved by giving an access token</param>
        /// <param name="clientCertificate">Certificate used for authenticating towards the Authorization Server</param>
        public RestTokenProvider(
            Uri accessTokenRetrievalEndpoint,
            X509Certificate2 clientCertificate)
            : this(accessTokenRetrievalEndpoint, clientCertificate, new InMemoryTokenCache())
        {

        }

        /// <summary>
        /// Uses a web request towards the Authorization Server for retrieving token information
        /// </summary>
        /// <param name="accessTokenRetrievalEndpoint">Path on the AuthorizationService server where token information can be resolved by giving an access token</param>
        /// <param name="clientCertificate">Certificate used for authenticating towards the Authorization Server</param>
        /// <param name="tokenCache">Used for caching tokens. </param>
        public RestTokenProvider(
            Uri accessTokenRetrievalEndpoint,
            X509Certificate2 clientCertificate,
            ITokenCache tokenCache)
            : this(() => BuildClient(accessTokenRetrievalEndpoint, clientCertificate), tokenCache)
        {

        }

        /// <summary>
        /// Intended for testing
        /// </summary>
        /// <param name="clientFactory"></param>
        /// <param name="tokenCache"></param>
        internal RestTokenProvider(
            Func<HttpClient> clientFactory,
            ITokenCache tokenCache)
        {
            if (tokenCache == null)
            {
                throw new ArgumentNullException(nameof(tokenCache));
            }
            _clientFactory = clientFactory;
            _tokenCache = tokenCache;
        }

        private static HttpClient BuildClient(Uri accessTokenRetrievalEndpoint, X509Certificate2 clientCertificate)
        {
            if (accessTokenRetrievalEndpoint == null)
            {
                throw new ArgumentNullException(nameof(accessTokenRetrievalEndpoint));
            }
            if (clientCertificate == null)
            {
                throw new ArgumentNullException(nameof(clientCertificate));
            }

            var handler = new WebRequestHandler();
            handler.ClientCertificates.Add(clientCertificate);

            var client = new HttpClient(handler)
            {
                BaseAddress = accessTokenRetrievalEndpoint
            };

            return client;
        }

        public void Initialize(IAppBuilder app, OioIdwsAuthenticationOptions options, ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;
        }

        public async Task<RetrieveTokenResult> RetrieveTokenAsync(string accessToken)
        {
            var cachedToken = await _tokenCache.RetrieveAsync(accessToken);
            if (cachedToken != null)
            {
                _logger.WriteVerbose("Token was found in cache. Performing expiry validation");
                //WSP doesn't add time skew. If the token is about expire, we'd rather rely on the Authorization Server to validate the token for us.
                if (cachedToken.ExpiresUtc > DateTimeOffset.UtcNow)
                {
                    return new RetrieveTokenResult(cachedToken);
                }

                _logger.WriteEntry(Log.TokenFromCacheExpired(accessToken));
            }

            var client = _clientFactory();

            _logger.WriteEntry(Log.AttemptRetrieveTokenFromAuthorizationServer(accessToken));
            var response = await client.GetAsync($"?{accessToken}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            { 
                _logger.WriteEntry(Log.TokenInformationNotFound(accessToken));
                return new RetrieveTokenResult(null);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var str = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(str);

                var expiry = json["expired"];

                if (expiry != null && expiry.Value<int>() == 1)
                {
                    return RetrieveTokenResult.AsExpired();
                }
                
                var error = json["error"].Value<string>();

                throw new InvalidOperationException(
$@"Unknown error while processing response from access token retrieval.
Got status code '{response.StatusCode}' and error: '{error}'");
            }

            var responseStream = await response.Content.ReadAsStreamAsync();

            using (var reader = new StreamReader(responseStream))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var token = new JsonSerializer().Deserialize<OioIdwsToken>(jsonReader);
                    if (token != null)
                    {
                        //cache on WSP to reduce round trips to AS. 
                        await _tokenCache.StoreAsync(accessToken, token);
                        _logger.WriteVerbose("Cached token");
                    }
                    return new RetrieveTokenResult(token);
                }
            }
        }
    }
}