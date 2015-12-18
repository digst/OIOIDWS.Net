using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Digst.OioIdws.Rest.AuthorizationService.Issuing
{
    internal interface ITokenValidator
    {
        Task<TokenValidationResult> ValidateTokenAsync(string token, X509Certificate2 clientCertificate, OioIdwsAuthorizationServiceMiddleware.Settings settings);
    }
}
