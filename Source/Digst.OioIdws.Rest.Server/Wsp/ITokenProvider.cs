using System.Threading.Tasks;
using Microsoft.Owin.Logging;
using Owin;

namespace Digst.OioIdws.Rest.Server.Wsp
{
    /// <summary>
    /// Handles retrieval of token information.
    /// </summary>
    public interface ITokenProvider
    {
        /// <summary>
        /// Invoked during startup of OWIN pipeline
        /// </summary>
        /// <param name="app">the OWIN app</param>
        /// <param name="options">middleware options</param>
        /// <param name="logger">logger used by middleware</param>
        void Initialize(IAppBuilder app, OioIdwsAuthenticationOptions options, ILogger logger);
        /// <summary>
        /// Returns token information stored for a given accesstoken.
        /// Token expiration MUST be checked and stored in the resulting object
        /// If the token doesn't exist/is unknown, it's expected to return a empty <see cref="RetrieveTokenResult"/>.
        /// </summary>
        /// <param name="accessToken">Access token presented by the client</param>
        /// <returns>Token information if possible, else empty/expiry state</returns>
        Task<RetrieveTokenResult> RetrieveTokenAsync(string accessToken);
    }
}
