using System.Threading.Tasks;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;

namespace Digst.OioIdws.Rest.Server.Wsp
{
    /// <summary>
    /// Cache used by <see cref="RestTokenProvider"/>. 
    /// It's expected to cache no longer than the tokens are valid. The cache is responsible for handling this itself.
    /// </summary>
    public interface ITokenCache
    {
        Task StoreAsync(string accessToken, OioIdwsToken token);
        Task<OioIdwsToken> RetrieveAsync(string accessToken);
    }
}
