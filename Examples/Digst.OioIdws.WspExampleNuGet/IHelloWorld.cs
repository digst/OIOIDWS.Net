using System.Net.Security;
using System.ServiceModel;

namespace Digst.OioIdws.WspExampleNuGet
{
    [ServiceContract]
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

        [OperationContract]
        string HelloEncryptAndSign(string name);

        [OperationContract]
        string HelloEncryptAndSignError(string name);
    }
}
