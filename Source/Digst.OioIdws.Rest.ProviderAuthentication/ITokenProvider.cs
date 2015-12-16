using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;

namespace Digst.OioIdws.Rest.ProviderAuthentication
{
    public interface ITokenProvider
    {
        Task<OioIdwsToken> RetrieveTokenAsync(string accessToken);
    }
}
