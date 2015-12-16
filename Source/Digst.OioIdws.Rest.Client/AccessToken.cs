using System;
using Digst.OioIdws.Rest.Common;

namespace Digst.OioIdws.Rest.Client
{
    public class AccessToken
    {
        public string Value { get; set; }
        public AccessTokenType Type { get; set; }
        public string TypeString { get; set; }
        public TimeSpan ExpiresIn { get; set; }
        public DateTime RetrievedAtUtc { get; set; }

        public bool IsValid()
        {
            //todo: time skew?
            return (RetrievedAtUtc + ExpiresIn) < DateTime.UtcNow;
        }
    }
}