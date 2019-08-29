using System.ServiceModel;

namespace Digst.OioIdws.Examples.Healthcare.ClientServer.Contracts
{
    [MessageContract]
    public class HelloResponseMessage
    {
        [MessageBodyMember]
        public string Answer { get; set; }

    }
}