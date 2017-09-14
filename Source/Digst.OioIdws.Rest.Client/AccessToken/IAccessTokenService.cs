using System.IdentityModel.Tokens;
using System.Threading;
using System.Threading.Tasks;

namespace Digst.OioIdws.Rest.Client.AccessToken
{
    /// <summary>
    /// Used for retrieving a token from Authorization Service (AS). The token can then be used to call WSP's (Web Service Providers).
    /// </summary>
    internal interface IAccessTokenService
    {
       Task<AccessToken> GetTokenAsync(GenericXmlSecurityToken securityToken, CancellationToken cancellationToken);
    }
}