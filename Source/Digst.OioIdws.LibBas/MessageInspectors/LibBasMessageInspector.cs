using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;
using Digst.OioIdws.Common;
using Digst.OioIdws.Common.Constants;
using Digst.OioIdws.Common.Logging;
using Digst.OioIdws.LibBas.Headers;

namespace Digst.OioIdws.LibBas.MessageInspectors
{
    /// <summary>
    /// This message inspector adds the liberty framework header to the the SOAP message.
    /// </summary>
    public class LibBasMessageInspector : IClientMessageInspector, IDispatchMessageInspector
    {
        #region Message Inspector of the Service

        /// <summary>
        /// This method is called on the server when a request is received from the client.
        /// </summary>
        public object AfterReceiveRequest(ref Message request,
               IClientChannel channel, InstanceContext instanceContext)
        {
            Logger.Instance.Trace("Validating liberty basic framework header on request from WSC.");
            ValidateLibertyBasicFrameworkHeader(request);
            Logger.Instance.Trace("Liberty basic framework header validated fine on request from WSC.");
            Logger.Instance.Trace("Validating WS-Adressing headers on request from WSC.");
            ValidateWsAddressingHeadersCommon(request);
            Logger.Instance.Trace("WS-Adressing headers validated fine on request from WSC.");
            return null;
        }

        /// <summary>
        /// This method is called after processing a method on the server side and just
        /// before sending the response to the client.
        /// </summary>
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            Logger.Instance.Trace("Adding liberty basic framework header on response from WSP.");
            reply.Headers.Add(new LibertyFrameworkHeader());
            Logger.Instance.Trace("Added liberty basic framework header on response from WSP.");

            // WCF does not automatically add a MessageID on responses.
            Logger.Instance.Trace("Adding MessageID header on response from WSP.");
            var header = new MessageHeader<string>("urn:uuid:" + Guid.NewGuid());
            reply.Headers.Add(header.GetUntypedHeader(WsAdressing.WsAdressingMessageId, WsAdressing.WsAdressing10NameSpace));
            Logger.Instance.Trace("Added MessageID header on response from WSP.");
        }

        #endregion

        #region Message Inspector of the Consumer

        /// <summary>
        /// This method will be called from the client side just before any method is called.
        /// </summary>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            Logger.Instance.Trace("Adding liberty basic framework header on request from WSC.");
            request.Headers.Add(new LibertyFrameworkHeader());
            Logger.Instance.Trace("Added liberty basic framework header on request from WSC.");
            return null;
        }

        /// <summary>
        /// This method will be called after completion of a request to the server.
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="correlationState"></param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            Logger.Instance.Trace("Validating liberty basic framework header on response from WSP.");
            ValidateLibertyBasicFrameworkHeader(reply);
            Logger.Instance.Trace("Liberty basic framework header validated fine on response from WSP.");
            Logger.Instance.Trace("Validating WS-Adressing headers on response from WSP.");
            ValidateWsAddressingHeadersRecievedFromWsp(reply);
            Logger.Instance.Trace("WS-Adressing headers validated fine on response from WSP.");
        }

        #endregion

        /// <summary>
        /// WS-Adressing headers are not required in responses according to the WS-Addressing specification. E.g. does WCF not automatically include a MessageId in responses.
        /// Therefore this extra validation has been made on top of WCF in order to always ensure that the WS-Addressing headers specified by [LIB-BAS] are present.
        /// </summary>
        /// <param name="message">The message in which the headers must be.</param>
        private static void ValidateWsAddressingHeadersCommon(Message message)
        {
            var messageIdHeader =
                message.Headers.SingleOrDefault(
                    x =>
                        WsAdressing.WsAdressingMessageId == x.Name &&
                        WsAdressing.WsAdressing10NameSpace == x.Namespace);

            if (messageIdHeader == null)
            {
                const string errorMessage = "WS-Adressing MessageID header was not present";
                Logger.Instance.Error(errorMessage);
                SoapFaults.CreateClientSoapFault(errorMessage);
            }

            var actionHeader =
                message.Headers.SingleOrDefault(
                    x =>
                        WsAdressing.WsAdressingAction == x.Name &&
                        WsAdressing.WsAdressing10NameSpace == x.Namespace);

            if (actionHeader == null)
            {
                const string errorMessage = "WS-Adressing Action header was not present";
                Logger.Instance.Error(errorMessage);
                throw new FaultException(SoapFaults.CreateClientSoapFault(errorMessage));
            }
        }

        private static void ValidateWsAddressingHeadersRecievedFromWsp(Message message)
        {
            // First do the client validation which is common for both server and client.
            ValidateWsAddressingHeadersCommon(message);

            var relatesToHeader =
                message.Headers.SingleOrDefault(
                    x =>
                        WsAdressing.WsAdressingRelatesTo == x.Name &&
                        WsAdressing.WsAdressing10NameSpace == x.Namespace);

            if (relatesToHeader == null)
            {
                var errorMessage = "WS-Adressing RelatesTo header was not present";
                Logger.Instance.Error(errorMessage);
                throw new FaultException(SoapFaults.CreateClientSoapFault(errorMessage));
            }
        }

        private static void ValidateLibertyBasicFrameworkHeader(Message message)
        {
            var headerPosition = message.Headers.FindHeader(Common.Constants.LibBas.HeaderName, Common.Constants.LibBas.HeaderNameSpace);
            if (headerPosition == -1)
            {
                var errorMessage = string.Format(
                    "The liberty basic framework header was not present. SOAP header with name '{0}' and namespace '{1}' was expected!",
                    Common.Constants.LibBas.HeaderName, Common.Constants.LibBas.HeaderNameSpace);
                Logger.Instance.Error(errorMessage);
                throw new FaultException(SoapFaults.CreateLibBasFrameWorkMisMatchSoapFault(errorMessage));
            }
            var content = message.Headers.GetHeader<XmlNode[]>(headerPosition);

            // Check that it is the correct profile
            var profileAttribute = content.Cast<XmlAttribute>()
                .SingleOrDefault(
                    x =>
                        Common.Constants.LibBas.ProfileName == x.LocalName && Common.Constants.LibBas.ProfileNameSpace == x.NamespaceURI);
            if (profileAttribute == null)
            {
                var errorMessage = string.Format("Attribute with name '{0}' in namespace '{1}' was not present in the liberty basic framework header.", Common.Constants.LibBas.ProfileName, Common.Constants.LibBas.ProfileNameSpace);
                Logger.Instance.Error(errorMessage);
                throw new FaultException(SoapFaults.CreateLibBasFrameWorkMisMatchSoapFault(errorMessage));
            }
            if (Common.Constants.LibBas.ProfileValue != profileAttribute.Value)
            {
                var errorMessage = string.Format("Profile name did not match in the liberty basic framework header. Value must be: '{0}' and value was: '{1}'.", Common.Constants.LibBas.ProfileValue, profileAttribute.Value);
                Logger.Instance.Error(errorMessage);
                throw new FaultException(SoapFaults.CreateLibBasFrameWorkMisMatchSoapFault(errorMessage));
            }

            // Check that it is the correct version of the profile
            var versionAttribute = content.Cast<XmlAttribute>()
                .SingleOrDefault(
                    x =>
                        Common.Constants.LibBas.VersionName == x.LocalName && string.Empty == x.NamespaceURI);
            if (versionAttribute == null)
            {
                var errorMessage = string.Format("Attribute with name '{0}' in the empty namespace was not present in the liberty basic framework header.", Common.Constants.LibBas.VersionName);
                Logger.Instance.Error(errorMessage);
                throw new FaultException(SoapFaults.CreateLibBasFrameWorkMisMatchSoapFault(errorMessage));
            }
            if (Common.Constants.LibBas.VersionValue != versionAttribute.Value)
            {
                var errorMessage = string.Format("Version did not match in the liberty basic framework header. Value should be: '{0}' and value was: '{1}'.", Common.Constants.LibBas.VersionValue, versionAttribute.Value);
                Logger.Instance.Error(errorMessage);
                throw new FaultException(SoapFaults.CreateLibBasFrameWorkMisMatchSoapFault(errorMessage));
            }

            // Security header element is marked with the MustUnderstand attribute. Hence, we need to inform the WCF framework that this header element has been taken care of.
            var frameworkHeader =
                message.Headers.Single(
                    x =>
                        Common.Constants.LibBas.HeaderName == x.Name &&
                        Common.Constants.LibBas.HeaderNameSpace == x.Namespace);
            Logger.Instance.Trace("Letting WCF know that liberty framework header has been processed.");
            message.Headers.UnderstoodHeaders.Add(frameworkHeader);
        }
    }

}
