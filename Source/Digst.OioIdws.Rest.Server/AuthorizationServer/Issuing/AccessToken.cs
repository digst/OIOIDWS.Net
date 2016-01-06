namespace Digst.OioIdws.Rest.Server.AuthorizationServer.Issuing
{
    internal class AccessToken
    {
        public string Value { get; set; }
        public string Type { get; set; }
        public int ExpiresInSeconds { get; set; }
    }
}