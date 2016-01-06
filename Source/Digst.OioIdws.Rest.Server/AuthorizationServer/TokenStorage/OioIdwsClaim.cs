namespace Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage
{
    public class OioIdwsClaim
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
        public string Issuer { get; set; }
    }
}