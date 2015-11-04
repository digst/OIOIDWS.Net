using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;
using System.Xml;
using Digst.OioIdws.Common.Constants;
using Digst.OioIdws.Common.Logging;
using Digst.OioIdws.LibBas.MessageInspectors;

namespace Digst.OioIdws.LibBas.Behaviors
{
    /// <summary>
    /// This custom behavior class is used to add the liberty framework header to the SOAP message and ensure that it is included in the signature.
    /// </summary>
    public class LibBasServiceBehavior : IEndpointBehavior
    {
        #region IEndpointBehavior Members

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            // This is done in order to have the liberty framework header included in the SOAP signaure. This is required by [LIB-BAS].
            Logger.Instance.Trace("Specifying that the liberty framework header must be signed in the response to WSC.");
            var requirements = bindingParameters.Find<ChannelProtectionRequirements>();

            // Setting signature requirements for WSP response to WSC
            requirements.OutgoingSignatureParts.AddParts(MessagePartSpecificationWsp());

            // Setting signature requirements for WSC request to WSP
            // This is done in order to validate the request. This is required by [LIB-BAS].
            requirements.IncomingSignatureParts.AddParts(LibBasClientBehavior.MessagePartSpecificationWsc());
        }

        public static MessagePartSpecification MessagePartSpecificationWsp()
        {
            // Liberty framework header
            var libertyFrameworkQualifiedName = new XmlQualifiedName(Common.Constants.LibBas.HeaderName,
                Common.Constants.LibBas.HeaderNameSpace);

            // WS-Addressing headers. This logic only checks if a given header is part of the signature. It does not fail if the header is not present.
            // This is just an extra insanity check on top of WCF after realizing that MessageId header was not automatically included in the response from WSP.
            // Instead of only checking MessageID all WS-Addressing headers specified by [LIB-BAS] has been included ... just to be sure.
            // Required by [LIB-BAS]
            var wsAddressingMessageIdQualifiedName = new XmlQualifiedName(WsAdressing.WsAdressingMessageId,
                WsAdressing.WsAdressing10NameSpace);
            var wsAddressingActionQualifiedName = new XmlQualifiedName(WsAdressing.WsAdressingAction,
                WsAdressing.WsAdressing10NameSpace);
            var wsAddressingToQualifiedName = new XmlQualifiedName(WsAdressing.WsAdressingTo, WsAdressing.WsAdressing10NameSpace); // This one is optional according to [LIB-BAS]

            var part = new MessagePartSpecification(libertyFrameworkQualifiedName,
                wsAddressingMessageIdQualifiedName, wsAddressingActionQualifiedName, wsAddressingToQualifiedName);
            
            // Setting IsBodyIncluded to true ensures that the body is always signed. Required by [LIB-BAS]
            part.IsBodyIncluded = true;
            return part;
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            throw new NotImplementedException();
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            Logger.Instance.Trace("Adding message inspector on WSP.");
            var inspector = new LibBasMessageInspector();
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        #endregion
    }
}
