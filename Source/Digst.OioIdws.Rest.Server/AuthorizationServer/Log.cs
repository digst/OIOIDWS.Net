using System;
using System.Diagnostics;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer
{
    internal static class Log
    {
        public static OioIdwsLogEntry ClientCertificateValdationFailed(string thumbprint, Exception ex)
        {
            return new OioIdwsLogEntry(TraceEventType.Error, 101,
                "Validating client certificate with thumbprint '{CertificateThumbprint} failed")
                .Property("CertificateThumbprint", thumbprint)
                .ExceptionOccured(ex);
        }

        public static OioIdwsLogEntry StartingTokenValidation()
        {
            return new OioIdwsLogEntry(TraceEventType.Start, 102, "Starting token validation");
        }

        public static OioIdwsLogEntry IssuingTokenDenied(string errorDescription, Exception validationException)
        {
            return new OioIdwsLogEntry(TraceEventType.Error, 103, "Issuing token was denied: {ValidationError}")
                .ExceptionOccured(validationException)
                .Property("ValidationError", errorDescription);
        }

        public static OioIdwsLogEntry TokenValidationCompleted()
        {
            return new OioIdwsLogEntry(TraceEventType.Information, 104, "Token validation completed successfully");
        }

        public static OioIdwsLogEntry NewAccessTokenValueGenerator(string accessTokenValue)
        {
            return new OioIdwsLogEntry(TraceEventType.Information, 105, "New access token value '{AccessTokenValue}' was generated")
                .Property("AccessTokenValue", accessTokenValue);
        }

        public static OioIdwsLogEntry TokenIssuedWithExpiration(string accessToken, TimeSpan expiresIn)
        {
            return new OioIdwsLogEntry(TraceEventType.Stop, 106, "Token '{AccessToken}' was issued with an expiration of {ExpiresIn}")
                .Property("AccessToken", accessToken)
                .Property("ExpiresIn", expiresIn);
        }

        public static OioIdwsLogEntry ProcessingToken(string accessToken)
        {
            return new OioIdwsLogEntry(TraceEventType.Start, 107, "Processing token '{AccessToken}'")
                .Property("AccessToken", accessToken);
        }

        public static OioIdwsLogEntry AttemptRetrieveInformationForAccessToken(string tokenValue)
        {
            return new OioIdwsLogEntry(TraceEventType.Information, 108,
                "Attempting to retrieve information for access token {TokenValue}")
                .Property("TokenValue", tokenValue);
        }

        public static OioIdwsLogEntry CouldNotRetrieveTokenInformationFromStore()
        {
            return new OioIdwsLogEntry(TraceEventType.Error, 109, "Token information could not be retrieved from the token store");
        }

        public static OioIdwsLogEntry TokenExpired(string accessToken)
        {
            return new OioIdwsLogEntry(TraceEventType.Error, 110, "The token '{AccessToken}' has expired")
                .Property("AccessToken", accessToken);
        }

        public static OioIdwsLogEntry RetrieveAccessTokenRequestFailed(string reason, string accessToken, Exception exception)
        {
            return new OioIdwsLogEntry(TraceEventType.Error, 111, "Request for retrieving token information failed for accessToken '{AccessToken}': {Reason}")
                .Property("AccessToken", accessToken)
                .Property("Reason", reason)
                .ExceptionOccured(exception);
        }

        public static OioIdwsLogEntry ProcessingTokenCompleted(string accessToken)
        {
            return new OioIdwsLogEntry(TraceEventType.Stop, 112, "Processing token '{AccessToken}' completed")
                .Property("AccessToken", accessToken);
        }
    }
}