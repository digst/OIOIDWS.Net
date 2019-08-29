using System.ServiceModel;

namespace Digst.OioIdws.Examples.Healthcare.ClientServer.Contracts
{
    [MessageContract]
    public class HelloRequestMessage
    {
        [MessageHeader]
        public AuthenticationToken AutToken { get; set; }

        [MessageBodyMember]
        public string Greeting { get; set; }
    }
}