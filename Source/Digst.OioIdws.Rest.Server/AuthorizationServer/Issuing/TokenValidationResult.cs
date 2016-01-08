using System.Security.Claims;
using Digst.OioIdws.Rest.Common;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer.Issuing
{
    public class TokenValidationResult
    {
        public bool Success { get; set; }
        public AccessTokenType AccessTokenType { get; set; }
        public string ErrorDescription { get; set; }
        public ClaimsIdentity ClaimsIdentity { get; set; }

        public static TokenValidationResult Error(string errorDescription)
        {
            return new TokenValidationResult
            {
                ErrorDescription = errorDescription,
                Success = false,
            };
        }
    }
}