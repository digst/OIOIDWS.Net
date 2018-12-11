﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Digst.OioIdws.Common;
using Digst.OioIdws.Common.Logging;
using Digst.OioIdws.OioWsTrust.Utils;

namespace Digst.OioIdws.OioWsTrust.ProtocolChannel
{
     /// <summary>
     /// This class implements the signature case part of specification [NEMLOGIN-STSRULES].
     /// It expects a standard WS-Trust message and transforms it to a format that NemLog-in STS understands and vice versa.
     /// The specification does not seem to be SOAP 1.1 compliant as it does not accept the mustUnderstand attribute on wsa:Action and wsa:To SOAP headers.
     /// Furthermore, SOAP Fauls are not SOAP 1.1 compliant. Therefore, we convert them to be.
     /// Request message to STS regarding signature case must be on the following minimum format:
     /// <S11:Envelope>
     ///     <S11:Header>
     ///         <wsa:Action>...</wsa:Action>
     ///         <wsa:MessageID>...</wsa:MessageID>
     ///         <wsa:To>...</wsa:To>
     ///         <wsse:Security>...</wsse:Security>
     ///     </S11:Header>
     ///     <S11:Body>
     ///         <wst:RequestSecurityToken>
     ///             <wst14:ActAs>...</wst14:ActAs> - Only present in bootstrap case scenario
     ///             <wst:RequestType>...</wst:RequestType>
     ///             <wsp:AppliesTo>...</wsp:AppliesTo>
     ///         </wst:RequestSecurityToken>
     ///     </S11:Body>
     /// </S11:Envelope>
     /// </summary>
     public class NemLoginWsTrustMessageTransformer : IOioWsTrustMessageTransformer
     {
        //'Old' .Net STS. Value differs from Java STS
        public const string WspNamespace = "http://schemas.xmlsoap.org/ws/2002/12/policy"; // Corresponds to WS-SecurityPolicy        

        public void ModifyMessageAccordingToStsNeeds(ref Message request, X509Certificate2 clientCertificate)
          {
               // Convert Message into a XML document that can be manipulated
               var xDocument = ConvertMessageToXml(request);

               // Log RST before being manipulated ... if we do not take the above two headers into account :)
               Logger.Instance.Trace("RST send to STS before being manipulated:\n" + xDocument);

               // Manipulate XML
               AddNamespacesToEnvelope(xDocument);
               ManipulateHeader(xDocument, clientCertificate);
               ManipulateBody(xDocument);
               SignMessage(ref xDocument, clientCertificate);

               // Log RST after being manipulated
               Logger.Instance.Trace("RST send to STS after being manipulated:\n" + xDocument);

               // Convert XML back to a Message
               request = ConvertXmlToMessage(request, xDocument);
          }

          public void ModifyMessageAccordingToWsTrust(ref Message response, X509Certificate2 stsCertificate)
          {
               // Convert Message into a XML document that can be manipulated
               var xDocument = ConvertMessageToXml(response);

               // Log RSTR before being manipulated
               Logger.Instance.Trace("RSTR recieved from STS before being manipulated:\n" + xDocument);

               // SOAP 1.1 faults from NemLog-in STS contains two Envelope elements. This hack removes the first element whereafter the .Net framework will see it as a fault.
               RemoveOuterEnvelopeElementIfMessageIsASoapFault(ref xDocument, ref response);

               // Fault response is not SOAP 1.1 compliant. We therefore need to change it so that other channels (e.g. WSTrust channel) are able to read it properly.
               if (response.IsFault)
               {
                    // Remove default namespace in order for fault message to be soap 1.1 compliant.
                    var namespaceManager = new XmlNamespaceManager(new NameTable());
                    namespaceManager.AddNamespace("s", Constants.S11Namespace);
                    namespaceManager.AddNamespace("wst", Constants.Wst13Namespace);
                    var envelopeElement = xDocument.XPathSelectElement("/s:Envelope", namespaceManager);
                    var xmlnsAttribute = envelopeElement.Attribute(XName.Get("xmlns"));
                    xmlnsAttribute.Remove();

                    // faultcode must contain a qualified name. However, NemLog-in has forgotten to specify the wst namespace in order to make the SOAP fault SOAP 1.1 compliant.
                    var xmlnsWstAttribute = envelopeElement.Attribute(XNamespace.Xmlns + Constants.Wst13Prefix);
                    if (xmlnsWstAttribute == null)
                         envelopeElement.Add(new XAttribute(XNamespace.Xmlns + Constants.Wst13Prefix, Constants.Wst13Namespace));

                    // faultcode and faultstring must be in the empty namespace.
                    var faultElement = xDocument.XPathSelectElement("/s:Envelope/s:Body/s:Fault", namespaceManager);
                    var faultcodeElement = xDocument.XPathSelectElement("/s:Envelope/s:Body/s:Fault/wst:faultcode",
                        namespaceManager);
                    faultcodeElement.Remove();
                    var faultstringElement = xDocument.XPathSelectElement("/s:Envelope/s:Body/s:Fault/wst:faultstring",
                        namespaceManager);
                    faultstringElement.Remove();
                    var newFaultCodeElement = new XElement("faultcode") { Value = faultcodeElement.Value };
                    var newFaultStringElement = new XElement("faultstring") { Value = faultstringElement.Value };
                    faultElement.Add(newFaultCodeElement);
                    faultElement.Add(newFaultStringElement);
               }
               // Normal RSTR
               else
               {
                    var namespaceManager = new XmlNamespaceManager(new NameTable());
                    namespaceManager.AddNamespace("s", Constants.S11Namespace);
                    namespaceManager.AddNamespace("wsse", Constants.Wsse10Namespace);
                    namespaceManager.AddNamespace("wsu", Constants.WsuNamespace);
                    namespaceManager.AddNamespace("wst", Constants.Wst13Namespace);
                    namespaceManager.AddNamespace("d", Constants.XmlDigSigNamespace);
                    namespaceManager.AddNamespace("wsa", Constants.WsaNamespace);

                    // Verify signature before making any modifications
                    if (!XmlSignatureUtils.VerifySignature(xDocument, stsCertificate))
                         throw new InvalidOperationException("SOAP signature received from STS does not validate!");

                    // Expiry time is currently not on the format specified by the spec. The spec says yyyy-MM-ddTHH:mm:ssZ but yyyy-MM-ddTHH:mm:ss.fffZ is currently retrieved.
                    // Verify life time of SOAP message
                    var messageExpireTimeElement = xDocument.XPathSelectElement("/s:Envelope/s:Header/wsse:Security/wsu:Timestamp/wsu:Expires", namespaceManager);
                    var messageExpireZuluTime = GetPatchedDateTime(messageExpireTimeElement.Value);
                    var currentZuluTime = DateTime.UtcNow;
                    if (currentZuluTime >= messageExpireZuluTime)
                         throw new InvalidOperationException("SOAP message has expired. Current Zulu time was: " + currentZuluTime + ", message Zulu expiry time was: " + messageExpireZuluTime);

                    // Verify life time of RSTS
                    var rstsExpireTimeElement = xDocument.XPathSelectElement("/s:Envelope/s:Body/wst:RequestSecurityTokenResponseCollection/wst:RequestSecurityTokenResponse/wst:Lifetime/wsu:Expires", namespaceManager);

                    var rstsExpireZuluTime = GetPatchedDateTime(rstsExpireTimeElement.Value);

                    if (currentZuluTime >= rstsExpireZuluTime)
                         throw new InvalidOperationException("RSTS has expired. Current Zulu time was: " + currentZuluTime + ", RSTS Zulu expiry time was: " + rstsExpireZuluTime);

                    // Verify replay attack
                    var signatureValueElement = xDocument.XPathSelectElement("/s:Envelope/s:Header/wsse:Security/d:Signature/d:SignatureValue", namespaceManager);
                    if (ReplayAttackCache.DoesKeyExist(signatureValueElement.Value))
                    {
                         var messageIdElement = xDocument.XPathSelectElement("/s:Envelope/s:Header/wsa:MessageID",
                             namespaceManager);
                         throw new InvalidOperationException("Replay attack detected. Response message id: " + messageIdElement.Value);
                    }
                    else
                    {
                         ReplayAttackCache.Set(signatureValueElement.Value, messageExpireZuluTime);
                    }

                    // response is validated ok
                    ManipulateRstrBody(xDocument);
               }

               // Log RSTR before being manipulated
               Logger.Instance.Trace("RSTR recieved from STS after being manipulated:\n" + xDocument);

               // Convert XML back to a Message
               response = ConvertXmlToMessage(response, xDocument);

               // The Security element is only present in succesfull responses.
               if (!response.IsFault)
               {
                    // Security header element is marked with the MustUnderstand attribute. Hence, we need to inform the WCF framework that this header element has been taken care of.
                    response.Headers.UnderstoodHeaders.Add(
                        response.Headers.Single(x => "Security" == x.Name && Constants.Wsse10Namespace == x.Namespace));
               }
          }

          /// <summary>
          /// NemLogin STS doesn't have a strict datetime format, therefore we patch it ourselves to remove variant millisecond component
          /// </summary>
          /// <param name="dateTimeString"></param>
          /// <returns></returns>
          static DateTime GetPatchedDateTime(string dateTimeString)
          {
               var index = dateTimeString.IndexOf(".", StringComparison.Ordinal);
               if (index != -1)
               {
                    dateTimeString = dateTimeString.Substring(0, index) + "Z";
               }

               return DateTime.ParseExact(dateTimeString, Constants.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
          }

          private static void RemoveOuterEnvelopeElementIfMessageIsASoapFault(ref XDocument xDocument, ref Message response)
          {
               var namespaceManager = new XmlNamespaceManager(new NameTable());
               namespaceManager.AddNamespace("s", Constants.S11Namespace);
               var faultElement = xDocument.XPathSelectElement("/s:Envelope/s:Body/s:Envelope/s:Body/s:Fault",
                   namespaceManager);
               if (faultElement != null)
               {
                    var innerEnvelopeElement = xDocument.XPathSelectElement("/s:Envelope/s:Body/s:Envelope", namespaceManager);
                    xDocument = XDocument.Parse(innerEnvelopeElement.ToString());
                    response = ConvertXmlToMessage(response, xDocument);
               }
          }

          private static void AddNamespacesToEnvelope(XDocument xDocument)
          {
               var namespaceManager = new XmlNamespaceManager(new NameTable());
               namespaceManager.AddNamespace("s", Constants.S11Namespace);
               var envelopeElement = xDocument.XPathSelectElement("/s:Envelope", namespaceManager);

               // This namespace is required. If not added then Id attributes is not correctly prefixed with "wsu" and RST becomes invalid according to standard.
               envelopeElement.Add(new XAttribute(XNamespace.Xmlns + Constants.WsuPrefix, Constants.WsuNamespace));
          }

          private static Message ConvertXmlToMessage(Message request, XNode xNode)
          {
               byte[] messageAsBytes;
               using (var memoryStream = new MemoryStream())
               {
                    using (var xw = XmlWriter.Create(memoryStream))
                    {
                         xNode.WriteTo(xw);
                    }
                    // No need to set position to 0 as ToArray does not use this property.
                    messageAsBytes = memoryStream.ToArray();
               }
               var xmlDictionaryReader = XmlDictionaryReader.CreateTextReader(messageAsBytes,
                   XmlDictionaryReaderQuotas.Max);
               var newMessage = Message.CreateMessage(xmlDictionaryReader, int.MaxValue, request.Version);
               newMessage.Properties.CopyProperties(request.Properties);
               return newMessage;
          }

          private static void ManipulateHeader(XDocument xDocument, X509Certificate2 clientCertificate)
          {
               var namespaceManager = new XmlNamespaceManager(new NameTable());
               namespaceManager.AddNamespace("a", Constants.WsaNamespace);
               namespaceManager.AddNamespace("s", Constants.S11Namespace);
               namespaceManager.AddNamespace("vs", Constants.VsDebuggerNamespace);
               namespaceManager.AddNamespace("vcf", Constants.WcfDiagnosticsNamespace);
               namespaceManager.AddNamespace("wst13", Constants.Wst13Namespace);
               namespaceManager.AddNamespace("wst14", Constants.Wst14Namespace);

               // The spec states that all header elements (also those not used by the STS) must be included in the signature. Hence, we need to remove the debugger element.
               // Remove VS debugger element. It is only present when running in debug mode. So removing the element is just to make life easier for developers.
               var vsDebuggerElement = xDocument.XPathSelectElement("/s:Envelope/s:Header/vs:VsDebuggerCausalityData", namespaceManager);
               if (vsDebuggerElement != null)
               {
                    vsDebuggerElement.Remove();
               }
               // Remove Diagnostics tracing element. It is only present when WCF Diagnostics are enabled. So removing the element is just to make life easier for developers.
               var wcfDiagnosticsElement = xDocument.XPathSelectElement("/s:Envelope/s:Header/vcf:ActivityId", namespaceManager);
               if (wcfDiagnosticsElement != null)
               {
                    wcfDiagnosticsElement.Remove();
               }

               // Because ManuelAddressing is set to true we need to manual add the messageID header
               var actionElement = xDocument.XPathSelectElement("/s:Envelope/s:Header/a:Action", namespaceManager);
               var messageIdElement = new XElement(XName.Get("MessageID", Constants.WsaNamespace));
               messageIdElement.Value = "uuid:" + Guid.NewGuid().ToString("D");
               actionElement.AddAfterSelf(messageIdElement);
            
               // a:To is normally set to the URI of the service endpoint by the framework which is not what we need here and therefore we neeed to set it manually.
               // In order to work ... ManualAddressing must be set to true on HttpsTransportBindingElement or else the a:To is overwritten in the HttpsTransportChannel.
               var toElement = new XElement(XName.Get("To", Constants.WsaNamespace));
               toElement.Value = IsBootstrapScenario(xDocument, namespaceManager)
                   ? Constants.BootstrapTokenCaseEntityId
                   : Constants.SignatureCaseEntityId;

               messageIdElement.AddAfterSelf(toElement);

               // Add Security element
               var createdElement = new XElement(XName.Get("Created", Constants.WsuNamespace));
               var currentTime = DateTime.UtcNow;
               createdElement.Value = currentTime.ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture);
               var expiresElement = new XElement(XName.Get("Expires", Constants.WsuNamespace));
               expiresElement.Value = currentTime.AddMinutes(5).ToString("s") + "Z"; // Make request expire after 5 minutes.
               var timestampElement = new XElement(XName.Get("Timestamp", Constants.WsuNamespace));
               timestampElement.Add(createdElement);
               timestampElement.Add(expiresElement);
               var binarySecurityTokenElement = new XElement(XName.Get("BinarySecurityToken", Constants.Wsse10Namespace));
               binarySecurityTokenElement.Add(new XAttribute("ValueType", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3"));
               binarySecurityTokenElement.Add(new XAttribute("EncodingType", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary"));
               binarySecurityTokenElement.Value = Convert.ToBase64String(clientCertificate.Export(X509ContentType.Cert));
               var securityElement = new XElement(XName.Get("Security", Constants.Wsse10Namespace));
               securityElement.Add(new XAttribute(XName.Get("mustUnderstand", Constants.S11Namespace), "1"));
               securityElement.Add(timestampElement);
               securityElement.Add(binarySecurityTokenElement);
               var headerElement = xDocument.XPathSelectElement("/s:Envelope/s:Header", namespaceManager);
               headerElement.Add(securityElement);
          }

          private static bool IsBootstrapScenario(XDocument xDocument, XmlNamespaceManager namespaceManager)
          {
               return xDocument.XPathSelectElement("/s:Envelope/s:Body/wst13:RequestSecurityToken/wst14:ActAs", namespaceManager) != null;
          }

          private static void ManipulateBody(XDocument xDocument)
          {
               var namespaceManager = new XmlNamespaceManager(new NameTable());
               namespaceManager.AddNamespace("wsp12", Constants.Wsp12Namespace);
               namespaceManager.AddNamespace("s", Constants.S11Namespace);
               namespaceManager.AddNamespace("wsa", Constants.WsaNamespace);
               namespaceManager.AddNamespace("trust", Constants.Wst13Namespace);
               namespaceManager.AddNamespace("wsp", WspNamespace);
               namespaceManager.AddNamespace("wsu", Constants.WsuNamespace);

               // Add Context attribute to wst:RequestSecurityToken with a unique ID as value
               var requestSecurityTokenElement = xDocument.XPathSelectElement("/s:Envelope/s:Body/trust:RequestSecurityToken", namespaceManager);
               requestSecurityTokenElement.Add(new XAttribute("Context", new UniqueId()));

               // Replace namespace http://schemas.xmlsoap.org/ws/2004/09/policy with http://schemas.xmlsoap.org/ws/2002/12/policy
               var appliesToElement = xDocument.XPathSelectElement("/s:Envelope/s:Body/trust:RequestSecurityToken/wsp12:AppliesTo", namespaceManager);
               var endpointReferenceElement = xDocument.XPathSelectElement("/s:Envelope/s:Body/trust:RequestSecurityToken/wsp12:AppliesTo/wsa:EndpointReference", namespaceManager);
               appliesToElement.Remove();
               var newAppliesToElement = new XElement(XName.Get("AppliesTo", WspNamespace));
               newAppliesToElement.Add(endpointReferenceElement);
               requestSecurityTokenElement.Add(newAppliesToElement);

               // Remove last '/' in endpoint address. Due to new URI(...) automatically adds an ending '/'.
               var addressReferenceElement = xDocument.XPathSelectElement("/s:Envelope/s:Body/trust:RequestSecurityToken/wsp:AppliesTo/wsa:EndpointReference/wsa:Address", namespaceManager);
               RemoveEndingForwardSlash(addressReferenceElement);

               // Change lifetime expires format if present from "yyyy-MM-ddTHH:mm:ss.fffZ" to "yyyy-MM-ddTHH:mm:ssZ"
               var lifetimeElement = xDocument.XPathSelectElement("/s:Envelope/s:Body/trust:RequestSecurityToken/trust:Lifetime/wsu:Expires", namespaceManager);
               if (lifetimeElement != null)
               {
                    lifetimeElement.Value = GetPatchedDateTime(lifetimeElement.Value).ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture);
               }
          }

          private static void ManipulateRstrBody(XDocument xDocument)
          {
               var namespaceManager = new XmlNamespaceManager(new NameTable());
               namespaceManager.AddNamespace("wsp12", Constants.Wsp12Namespace);
               namespaceManager.AddNamespace("s", Constants.S11Namespace);
               namespaceManager.AddNamespace("wsa", Constants.WsaNamespace);
               namespaceManager.AddNamespace("trust", Constants.Wst13Namespace);
               namespaceManager.AddNamespace("wsp", WspNamespace);
               namespaceManager.AddNamespace("wsse11", Constants.Wsse11Namespace);
               namespaceManager.AddNamespace("wsse", Constants.Wsse10Namespace);

               // Replace namespace http://schemas.xmlsoap.org/ws/2002/12/policy with http://schemas.xmlsoap.org/ws/2004/09/policy on the AppliesToElement
               var requestSecurityTokenResponseElement = xDocument.XPathSelectElement("/s:Envelope/s:Body/trust:RequestSecurityTokenResponseCollection/trust:RequestSecurityTokenResponse", namespaceManager);
               var appliesToElement = xDocument.XPathSelectElement("/s:Envelope/s:Body/trust:RequestSecurityTokenResponseCollection/trust:RequestSecurityTokenResponse/wsp:AppliesTo", namespaceManager);
               var endpointReferenceElement = xDocument.XPathSelectElement("/s:Envelope/s:Body/trust:RequestSecurityTokenResponseCollection/trust:RequestSecurityTokenResponse/wsp:AppliesTo/wsa:EndpointReference", namespaceManager);
               appliesToElement.Remove();
               var newAppliesToElement = new XElement(XName.Get("AppliesTo", Constants.Wsp12Namespace));
               newAppliesToElement.Add(endpointReferenceElement);
               requestSecurityTokenResponseElement.Add(newAppliesToElement);

               // Replace RequestedAttachedReference and RequestedUnattachedReference with true saml2 SecurityTokenReference elements
               // This is needed because WCF otherwise does not know that the token is an encrypted SAML2 assertion and would therefore not be able to create a correct SecurityTokenReference according to the OIO-IDWS-SOAP profile when making the call from WSC to WSP.
               var referenceElement = xDocument.XPathSelectElement("/s:Envelope/s:Body/trust:RequestSecurityTokenResponseCollection/trust:RequestSecurityTokenResponse/trust:RequestedAttachedReference/wsse:SecurityTokenReference/wsse:Reference", namespaceManager);

               var assertionReferenceId = referenceElement.Attribute(XName.Get("URI")).Value.Substring(1); // Substring is used to convert e.g. '#encryptedassertion' to 'encryptedassertion'

               var requestedAttachedReferenceElement = xDocument.XPathSelectElement("/s:Envelope/s:Body/trust:RequestSecurityTokenResponseCollection/trust:RequestSecurityTokenResponse/trust:RequestedAttachedReference", namespaceManager);
               requestedAttachedReferenceElement.Remove();
               var requestedUnattachedReferenceElement = xDocument.XPathSelectElement("/s:Envelope/s:Body/trust:RequestSecurityTokenResponseCollection/trust:RequestSecurityTokenResponse/trust:RequestedUnattachedReference", namespaceManager);
               requestedUnattachedReferenceElement.Remove();
               // Recreate RequestedAttachedReference
               var newKeyIdentifierElement = new XElement(XName.Get("KeyIdentifier", Constants.Wsse10Namespace));
               newKeyIdentifierElement.Add(new XAttribute("ValueType", Constants.SamlValueType));
               newKeyIdentifierElement.Value = assertionReferenceId;

               var newSecurityTokenReferenceElement = new XElement(XName.Get("SecurityTokenReference", Constants.Wsse10Namespace));
               newSecurityTokenReferenceElement.Add(new XAttribute(XName.Get("TokenType", Constants.Wsse11Namespace), Constants.Saml20TokenType));
               newSecurityTokenReferenceElement.Add(newKeyIdentifierElement);

               var newRequestedAttachedReferenceElement = new XElement(XName.Get("RequestedAttachedReference", Constants.Wst13Namespace));
               newRequestedAttachedReferenceElement.Add(newSecurityTokenReferenceElement);
               // Recreate RequestedUnattachedReference
               var newRequestedUnattachedReferenceElement = new XElement(XName.Get("RequestedUnattachedReference", Constants.Wst13Namespace));
               newRequestedUnattachedReferenceElement.Add(newSecurityTokenReferenceElement);

               // Add the new elements to RequestSecurityTokenResponse
               requestSecurityTokenResponseElement.Add(newRequestedAttachedReferenceElement);
               requestSecurityTokenResponseElement.Add(newRequestedUnattachedReferenceElement);
          }

          private static void RemoveEndingForwardSlash(XElement element)
          {
               var addressValue = element.Value;
               if (addressValue.EndsWith("/"))
               {
                    element.Value = addressValue.Substring(0, addressValue.Length - 1);
               }
          }

          private static void SignMessage(ref XDocument xDocument, X509Certificate2 clientCertificate)
          {
               // Add id's to elements that needs to be signed.
               var namespaceManager = new XmlNamespaceManager(new NameTable());
               namespaceManager.AddNamespace("a", Constants.WsaNamespace);
               namespaceManager.AddNamespace("s", Constants.S11Namespace);
               namespaceManager.AddNamespace("wsse", Constants.Wsse10Namespace);
               namespaceManager.AddNamespace("wsu", Constants.WsuNamespace);
               var actionElement = xDocument.XPathSelectElement("/s:Envelope/s:Header/a:Action", namespaceManager);
               var msgElement = xDocument.XPathSelectElement("/s:Envelope/s:Header/a:MessageID", namespaceManager);
               var toElement = xDocument.XPathSelectElement("/s:Envelope/s:Header/a:To", namespaceManager);
               var timeStampElement = xDocument.XPathSelectElement("/s:Envelope/s:Header/wsse:Security/wsu:Timestamp", namespaceManager);
               var binarySecurityTokenElement = xDocument.XPathSelectElement("/s:Envelope/s:Header/wsse:Security/wsse:BinarySecurityToken", namespaceManager);
               var bodyElement = xDocument.XPathSelectElement("/s:Envelope/s:Body", namespaceManager);
               var idXName = XName.Get("Id", Constants.WsuNamespace);
               actionElement.Add(new XAttribute(idXName, Constants.ActionIdValue));
               msgElement.Add(new XAttribute(idXName, Constants.MessageIdIdValue));
               toElement.Add(new XAttribute(idXName, Constants.ToIdValue));
               timeStampElement.Add(new XAttribute(idXName, Constants.TimeStampIdValue));
               binarySecurityTokenElement.Add(new XAttribute(idXName, Constants.BinarySecurityTokenIdValue));
               bodyElement.Add(new XAttribute(idXName, Constants.BodyIdValue));

               var idOfElementsThatMustBeSigned = new List<string> { Constants.ActionIdValue, Constants.MessageIdIdValue, Constants.ToIdValue, Constants.TimeStampIdValue, Constants.BinarySecurityTokenIdValue, Constants.BodyIdValue };

               xDocument = XmlSignatureUtils.SignDocument(xDocument, idOfElementsThatMustBeSigned, clientCertificate);
          }

          private static XDocument ConvertMessageToXml(Message request)
          {
               XDocument xDocument;
               using (var memoryStream = new MemoryStream())
               {
                    using (var xmlDictionaryWriter = XmlDictionaryWriter.CreateTextWriter(memoryStream))
                    {
                         request.WriteMessage(xmlDictionaryWriter);
                         xmlDictionaryWriter.Flush();
                         memoryStream.Position = 0; // Needed in order for XDocument.Load to read stream from beginning.
                         xDocument = XDocument.Load(memoryStream);
                    }
               }
               return xDocument;
          }

          private static void RemoveMustUnderstandAttribute(XElement element)
          {
               var mustUnderstandAttribute =
                   element.Attribute(XName.Get("mustUnderstand", Constants.S11Namespace));
               if (mustUnderstandAttribute != null)
               {
                    mustUnderstandAttribute.Remove();
               }
          }
     }
}