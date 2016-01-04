using System.Security.Claims;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;

namespace Digst.OioIdws.Rest.Authentication
{
    public interface IIdentityBuilder
    {
        Task<ClaimsIdentity> BuildIdentityAsync(OioIdwsToken token);
    }
}