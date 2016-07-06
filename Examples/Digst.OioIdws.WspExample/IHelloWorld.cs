using System.Net.Security;
using System.ServiceModel;

namespace Digst.OioIdws.WspExample
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

        /// <summary>
        /// Soap faults are not encrypted when predefined as a <see cref="FaultContractAttribute"/>
        /// </summary>
        [OperationContract(ProtectionLevel = ProtectionLevel.Sign)]
        [FaultContract(typeof(string))]
        string HelloSignErrorNotEncrypted(string name);

        [OperationContract]
        string HelloEncryptAndSign(string name);

        [OperationContract]
        string HelloEncryptAndSignError(string name);
    }
}
