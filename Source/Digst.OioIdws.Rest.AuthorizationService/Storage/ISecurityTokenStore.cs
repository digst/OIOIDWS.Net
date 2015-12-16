using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;

namespace Digst.OioIdws.Rest.AuthorizationService.Storage
{
    public interface ISecurityTokenStore
    {
        Task StoreTokenAsync(string accessToken, OioIdwsToken oioIdwsToken);
        Task<OioIdwsToken> RetrieveTokenAsync(string accessToken);
    }
}
