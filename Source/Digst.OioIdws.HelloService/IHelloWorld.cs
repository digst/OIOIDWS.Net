using System.Net.Security;
using System.ServiceModel;

//using Digst.OioIdws.Wsp.Wsdl;

namespace Digst.OioIdws.HelloService
{
     [ServiceContract]
     //[WsdlExportExtension]
     public interface IHelloWorld
     {
          [OperationContract(ProtectionLevel = ProtectionLevel.None)]
          string HelloNone(string name);

          [OperationContract(ProtectionLevel = ProtectionLevel.None)]
          string HelloNoneError(string name);

          [OperationContract(ProtectionLevel = ProtectionLevel.Sign)]
          string HelloSign(string name);

          [OperationContract(ProtectionLevel = ProtectionLevel.Sign)]
          string HelloSignError(string name);

     }
}
