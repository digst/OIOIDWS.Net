using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;
using Newtonsoft.Json;

namespace Digst.OioIdws.Rest.Server.Wsp
{
    internal class TokenProvider : ITokenProvider
    {
        public async Task<OioIdwsToken> RetrieveTokenAsync(string accessToken, Uri accessTokenRetrievalEndpoint)
        {
            if (accessTokenRetrievalEndpoint == null)
            {
                throw new ArgumentNullException(nameof(accessTokenRetrievalEndpoint));
            }

            var client = new HttpClient
            {
                BaseAddress = accessTokenRetrievalEndpoint
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