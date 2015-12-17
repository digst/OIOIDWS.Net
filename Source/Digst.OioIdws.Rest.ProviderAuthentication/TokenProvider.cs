using System.Net.Http;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;

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

            var token = await response.EnsureSuccessStatusCode().Content.ReadAsAsync<OioIdwsToken>();
            return token;
        }
    }
}