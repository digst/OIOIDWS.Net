using System.Security.Cryptography.X509Certificates;

namespace Digst.OioIdws.Rest.AuthorizationService.Issuing
{
    internal interface ITokenValidator
    {
        TokenValidationResult ValidateToken(string token, X509Certificate2 clientCertificate, OioIdwsAuthorizationServiceMiddleware.Settings settings);
    }
}
