using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Digst.OioIdws.Rest.AuthorizationService.Issuing
{
    internal interface ITokenValidator
    {
        TokenValidationResult ValidateToken(string token, X509Certificate2 clientCertificate, OioIdwsAuthorizationServiceMiddleware.Settings settings);
    }
}
