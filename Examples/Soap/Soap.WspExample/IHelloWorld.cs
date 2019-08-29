using System.Net.Security;
using System.ServiceModel;

using Digst.OioIdws.Wsp.Wsdl;

namespace Digst.OioIdws.WspExample
{
    [ServiceContract]
    //[WsdlExportExtension(Token = TokenType.HolderOfKey)] 
    //[WsdlExportExtension(Token = TokenType.Bearer)]
    [WsdlExportExtension] // Default value -> (Token = TokenType.HolderOfKey)
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
