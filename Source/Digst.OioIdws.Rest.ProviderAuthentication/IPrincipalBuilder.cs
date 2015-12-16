using System.Security.Principal;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;

namespace Digst.OioIdws.Rest.ProviderAuthentication
{
    public interface IPrincipalBuilder
    {
        Task<IPrincipal> BuildPrincipalAsync(OioIdwsToken token);
    }
}