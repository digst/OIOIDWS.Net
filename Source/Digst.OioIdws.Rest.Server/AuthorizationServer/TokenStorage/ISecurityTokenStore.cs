using System.Threading.Tasks;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage
{
    /// <summary>
    /// Used for storage of security tokens
    /// </summary>
    public interface ISecurityTokenStore
    {
        /// <summary>
        /// Stores the token information for the given access token
        /// </summary>
        /// <param name="key">Access token being issued</param>
        /// <param name="oioIdwsToken">Token information to be stored</param>
        /// <returns></returns>
        Task StoreTokenAsync(string key, OioIdwsToken oioIdwsToken);
        /// <summary>
        /// Retrieves token information for the given access token. If the token doesn't exist or has been removed due to expiration, the method should return null.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<OioIdwsToken> RetrieveTokenAsync(string key);
    }
}
