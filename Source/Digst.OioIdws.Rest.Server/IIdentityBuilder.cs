using System.Security.Claims;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;

namespace Digst.OioIdws.Rest.Server
{
    /// <summary>
    /// Identity Builder
    /// </summary>
    public interface IIdentityBuilder
    {
        /// <summary>
        /// Invoked during authentication when token information was retrieved and the user is going to be logged in. 
        /// By customizing the process it's possible to add custom claim information during the login process
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<ClaimsIdentity> BuildIdentityAsync(OioIdwsToken token);
    }
}