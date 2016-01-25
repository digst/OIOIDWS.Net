using Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage;

namespace Digst.OioIdws.Rest.Server.Wsp
{
    public class RetrieveTokenResult
    {
        public RetrieveTokenResult(OioIdwsToken result)
        {
            Result = result;
        }

        private RetrieveTokenResult()
        {
            
        }

        public static RetrieveTokenResult AsExpired()
        {
            return new RetrieveTokenResult
            {
                Expired = true
            };
        }

        public OioIdwsToken Result { get; }
        public bool Expired { get; private set; }

        public bool Success => !Expired && Result != null;
    }
}