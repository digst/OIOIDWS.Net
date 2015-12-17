using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;

namespace Digst.OioIdws.Rest.ProviderAuthentication
{
    internal interface ITokenProvider
    {
        Task<OioIdwsToken> RetrieveTokenAsync(string accessToken, OioIdwsProviderAuthenticationMiddleware.Settings settings);
    }
}
