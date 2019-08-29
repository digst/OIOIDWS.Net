using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Digst.OioIdws.Examples.Healthcare.ClientServer.Contracts
{


    [ServiceContract]
    public interface IClientServerService
    {
        [OperationContract]
        HelloResponseMessage HelloClientServer(HelloRequestMessage requestMessage);

    }
}
