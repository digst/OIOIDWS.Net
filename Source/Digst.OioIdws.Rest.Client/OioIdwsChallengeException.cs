using System;
using Digst.OioIdws.Rest.Common;

namespace Digst.OioIdws.Rest.Client
{
    public class OioIdwsChallengeException : Exception
    {
        public OioIdwsChallengeException(AccessTokenType accessTokenType, string error, string errorDescription, string message) : base(message)
        {
            AccessTokenType = accessTokenType;
            Error = error;
            ErrorDescription = errorDescription;
        }

        public AccessTokenType AccessTokenType { get; }
        public string Error { get; }
        public string ErrorDescription { get; }
    }
}