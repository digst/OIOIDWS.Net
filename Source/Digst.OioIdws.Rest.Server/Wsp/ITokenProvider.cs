using System.Threading.Tasks;
using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;
using Owin;

namespace Digst.OioIdws.Rest.Server.Wsp
{
    public interface ITokenProvider
    {
        void Initialize(IAppBuilder app, OioIdwsAuthenticationOptions options);
        Task<OioIdwsToken> RetrieveTokenAsync(string accessToken);
    }
}
