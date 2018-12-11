using System;
using System.Runtime.Serialization;

namespace Digst.OioIdws.Examples.Healthcare.ClientServer.Contracts
{
    [DataContract]
    public class AuthenticationToken
    {
        [DataMember]
        public string SerializedToken { get; set; }

        [DataMember]
        public DateTime NotBefore { get; set; }

        [DataMember]
        public DateTime NotOnOrAfter { get; set; }

        [DataMember]
        public string TokenId { get; set; }
    }
}