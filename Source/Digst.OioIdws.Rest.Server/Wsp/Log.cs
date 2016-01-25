using System.Diagnostics;

namespace Digst.OioIdws.Rest.Server.Wsp
{
    internal class Log
    {
        public static OioIdwsLogEntry ProcessingToken(string accessToken)
        {
            return new OioIdwsLogEntry(TraceEventType.Start, 201, "Processing token '{AccessToken}' started")
                .Property("AccessToken", accessToken);
        }

        public static OioIdwsLogEntry TokenFromCacheExpired(string accessToken)
        {
            return new OioIdwsLogEntry(TraceEventType.Information, 202, "Token '{AccessToken}' from cache was expired")
                .Property("AccessToken", accessToken);
        }

        public static OioIdwsLogEntry InvalidTokenType(string scheme)
        {
            return new OioIdwsLogEntry(TraceEventType.Error, 203, "Invalid token type '{AuthorizationScheme}' was specified by client")
                .Property("AuthorizationScheme", scheme);
        }

        public static OioIdwsLogEntry HolderOfKeyNoCertificatePresented(string accessToken, string thumbprint)
        {
            return new OioIdwsLogEntry(TraceEventType.Error, 204, "Token '{AccessToken}' was a holder-of-key but no valid certificate was presented (certificate thumbprint: {CertificateThumbprint})")
                .Property("AccessToken", accessToken)
                .Property("CertificateThumbprint", thumbprint);
        }

        public static OioIdwsLogEntry TokenValidatedAndRequestAuthenticated(string accessToken)
        {
            return new OioIdwsLogEntry(TraceEventType.Stop, 205, "The token '{AccessToken}' was successfully validated and the caller has been authenticated")
                .Property("AccessToken", accessToken);
        }

        public static OioIdwsLogEntry TokenExpired(string accessToken)
        {
            return new OioIdwsLogEntry(TraceEventType.Error, 206, "The token '{AccessToken}' was expired")
                .Property("AccessToken", accessToken);
        }

        public static OioIdwsLogEntry TokenWasNotRetrievedFromAuthorizationServer(string accessToken)
        {
            return new OioIdwsLogEntry(TraceEventType.Stop, 207, "The access token '{AccessToken}' could not be retrieved from the Authorization Server")
                .Property("AccessToken", accessToken);
        }

        public static OioIdwsLogEntry AttemptRetrieveTokenFromAuthorizationServer(string accessToken)
        {
            return new OioIdwsLogEntry(TraceEventType.Information, 208, "Attempting to retrieve token '{AccessToken}' from Authorization Server")
                .Property("AccessToken", accessToken);
        }

        public static OioIdwsLogEntry TokenInformationNotFound(string accessToken)
        {
            return new OioIdwsLogEntry(TraceEventType.Information, 209,
                "Access token '{AccessToken}' was unknown to the Authorization Server")
                .Property("AccessToken", accessToken);
        }
    }
}
