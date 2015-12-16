using System.Collections.Generic;
using System.Security.Claims;
using Digst.OioIdws.Rest.Common;

namespace Digst.OioIdws.Rest.AuthorizationService.Issuing
{
    public class TokenValidationResult
    {
        public bool Success { get; set; }
        public AccessTokenType AccessTokenType { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public ClaimsIdentity ClaimsIdentity { get; set; }

        public static TokenValidationResult Error(string errorCode, string errorDescription)
        {
            return new TokenValidationResult
            {
                ErrorCode = errorCode,
                ErrorDescription = errorDescription,
                Success = false,
            };
        }
    }
}