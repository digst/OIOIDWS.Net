namespace Digst.OioIdws.Rest.Common
{
    public class AuthenticationErrorCodes
    {
        public static string InvalidRequest = "invalid_request";
        public static string InvalidToken = "invalid_token";
        public static string InsufficentScope = "insufficient_scope";

        public static class Descriptions
        {
            public static string SamlTokenMissing = "saml-token was missing";
        }
    }
}
