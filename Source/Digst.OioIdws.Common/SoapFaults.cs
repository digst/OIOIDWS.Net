using System.ServiceModel;
using System.ServiceModel.Channels;
using Digst.OioIdws.Common.Constants;

namespace Digst.OioIdws.Common
{
    public class SoapFaults
    {
        /// <summary>
        /// The Server class of errors indicate that the message could not be processed for reasons not directly attributable to the contents of the message itself but rather to the processing of the message. For example, processing could include communicating with an upstream processor, which didn't respond. The message may succeed at a later point in time.
        /// </summary>
        /// <param name="faultString"></param>
        /// <returns></returns>
        public static MessageFault CreateServerSoapFault(string faultString)
        {
            return CreateSoapFault("server", faultString);
        }

        /// <summary>
        /// The Client class of errors indicate that the message was incorrectly formed or did not contain the appropriate information in order to succeed. For example, the message could lack the proper authentication or payment information. It is generally an indication that the message should not be resent without change.
        /// </summary>
        /// <param name="faultString"></param>
        /// <returns></returns>
        public static MessageFault CreateClientSoapFault(string faultString)
        {
            return CreateSoapFault("client", faultString);
        }

        /// <summary>
        /// Indicates a problem with the [LIB-BAS] version.
        /// </summary>
        /// <param name="faultString"></param>
        /// <returns></returns>
        public static MessageFault CreateLibBasFrameWorkMisMatchSoapFault(string faultString)
        {
            return CreateSoapFault("FrameworkVersionMismatch", LibBas.HeaderNameSpace, faultString);
        }

        /// <summary>
        /// Creates a soap fault where the fault code is placed in the name space http://schemas.xmlsoap.org/soap/envelope/
        /// </summary>
        /// <param name="faultCode"></param>
        /// <param name="faultString"></param>
        /// <returns></returns>
        private static MessageFault CreateSoapFault(string faultCode, string faultString)
        {
            // "" namespace means the fault code is automatically placed in the http://schemas.xmlsoap.org/soap/envelope/ namespace
            return CreateSoapFault(faultCode, "", faultString);
        }

        private static MessageFault CreateSoapFault(string faultCode, string faultCodeNameSpace, string faultString)
        {
            // xmlLang in FaultReasonText is set to "". This results in the 'lang' atribute not being part of the faultstring element. The attribute is not mentioned in the SOAP 1.1 specification and hence it is removed.
            var messageFault = MessageFault.CreateFault(FaultCode.CreateReceiverFaultCode(faultCode, faultCodeNameSpace),
                new FaultReason(new FaultReasonText(faultString, "")));
            return messageFault;
        }
    }
}
