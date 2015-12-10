using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Digst.OioIdws.Rest.AuthorizationService
{
    public interface ITokenStore
    {
        Task StoreTokenAsync(string accessToken, SamlToken samlToken);
    }
}
