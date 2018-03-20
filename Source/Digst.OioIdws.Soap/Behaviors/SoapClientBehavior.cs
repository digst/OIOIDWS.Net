using System;
using System.IdentityModel.Tokens;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;
using System.Xml;
using Digst.OioIdws.Common.Constants;
using Digst.OioIdws.Common.Logging;
using Digst.OioIdws.Soap.MessageInspectors;
using Digst.OioIdws.Soap.StrCustomization;

namespace Digst.OioIdws.Soap.Behaviors
{
    /// <summary>
    /// This custom behavior class is used to add the liberty framework header to the SOAP message and ensure that it is included in the signature.
    /// </summary>
    public class SoapClientBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint,
            BindingParameterCollection bindingParameters)
        {
            // This is done in order to have the liberty framework header included in the SOAP signaure. This is required by [OIO IDWS SOAP 1.1].
            Logger.Instance.Trace("Specifying that the liberty framework header must be signed in the request to WSP.");
            var requirements = bindingParameters.Find<ChannelProtectionRequirements>();

            // Setting signature requirements for WSC request to WSP
            requirements.IncomingSignatureParts.AddParts(MessagePartSpecificationWsc());

            // Setting signature requirements for WSP repsonse to WSC
            // This is done in order to validate the response. This is required by [OIO IDWS SOAP 1.1].
            requirements.OutgoingSignatureParts.AddParts(SoapServiceBehavior.MessagePartSpecificationWsp());
            
            var clientCredentials = bindingParameters.Find<ClientCredentials>();
            clientCredentials.UseIdentityConfiguration = true; // Use WIF instead of WCF
            
            Logger.Instance.Trace("Adding custom SAML token handlers.");
            var securityTokenHandlerCollectionManager = clientCredentials.SecurityTokenHandlerCollectionManager[
                SecurityTokenHandlerCollectionManager.Usage.Default];
            // This is done in order to have correct STR's (Security Token Reference)
            securityTokenHandlerCollectionManager.AddOrReplace(
                    new StrReferenceSaml2SecurityTokenHandler());
        }

        public static MessagePartSpecification MessagePartSpecificationWsc()
        {
            // WS-Addressing headers. This logic only checks if a given header is part of the signature. It does not fail if the header is not present.
            // Checking if a header is present is done in the message inspectors.
            // MessageID is not automatically set in the response by WCF. Hence, a custom check is necessary to ensure that it is present and is part of the signature.
            // Instead of only checking MessageID all WS-Addressing headers specified by [OIO IDWS SOAP 1.1] has been included ... just to be sure.
            // Required by [OIO IDWS SOAP 1.1]
            var wsAddressingMessageIdQualifiedName = new XmlQualifiedName(WsAdressing.WsAdressingMessageId,
                WsAdressing.WsAdressing10NameSpace);
            var wsAddressingRelatesToQualifiedName = new XmlQualifiedName(WsAdressing.WsAdressingRelatesTo,
                WsAdressing.WsAdressing10NameSpace);
            var wsAddressingToQualifiedName = new XmlQualifiedName(WsAdressing.WsAdressingTo, WsAdressing.WsAdressing10NameSpace); // This one is optional according to [OIO IDWS SOAP 1.1]

            var part = 
                new MessagePartSpecification(
                    wsAddressingMessageIdQualifiedName,
                    wsAddressingRelatesToQualifiedName, 
                    wsAddressingToQualifiedName
                );

            // Setting IsBodyIncluded to true ensures that the body is always signed. Required by [OIO IDWS SOAP 1.1]
            part.IsBodyIncluded = true;
            return part;
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            Logger.Instance.Trace("Adding message inspector on WSC.");
            var inspector = new SoapMessageInspector();
            clientRuntime.MessageInspectors.Add(inspector);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            throw new NotImplementedException();
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}
