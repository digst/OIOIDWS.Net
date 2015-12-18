using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;
using Newtonsoft.Json;

namespace Digst.OioIdws.Rest.ProviderAuthentication
{
    internal class TokenProvider : ITokenProvider
    {
        public async Task<OioIdwsToken> RetrieveTokenAsync(string accessToken, OioIdwsProviderAuthenticationMiddleware.Settings settings)
        {
            var client = new HttpClient
            {
                BaseAddress = settings.AccessTokenRetrievalEndpoint
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