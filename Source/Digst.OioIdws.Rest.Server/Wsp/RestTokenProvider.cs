using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;
using Newtonsoft.Json;
using Owin;

namespace Digst.OioIdws.Rest.Server.Wsp
{
    public class RestTokenProvider : ITokenProvider
    {
        private readonly Uri _accessTokenRetrievalEndpoint;

        /// <summary>
        /// Uses a web request towards the Authorization Server for retrieving token information
        /// </summary>
        /// <param name="accessTokenRetrievalEndpoint">Path on the AuthorizationService server where token information can be resolved by giving an access token</param>
        public RestTokenProvider(Uri accessTokenRetrievalEndpoint)
        {
            if (accessTokenRetrievalEndpoint == null)
            {
                throw new ArgumentNullException(nameof(accessTokenRetrievalEndpoint));
            }
            _accessTokenRetrievalEndpoint = accessTokenRetrievalEndpoint;
        }

        public void Initialize(IAppBuilder app, OioIdwsAuthenticationOptions options)
        {
            // Nothing to initialize
        }

        public async Task<OioIdwsToken> RetrieveTokenAsync(string accessToken)
        {
            //todo: caching

            var client = new HttpClient
            {
                BaseAddress = _accessTokenRetrievalEndpoint
            };

            var response = await client.GetAsync($"?{accessToken}");

            var responseStream = await response.EnsureSuccessStatusCode().Content.ReadAsStreamAsync();

            using (var reader = new StreamReader(responseStream))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var token = new JsonSerializer().Deserialize<OioIdwsToken>(jsonReader);
                    return token;
                }
            }
        }
    }
}