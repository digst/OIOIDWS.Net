using System;
using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Security.Authentication;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Threading;
using System.Threading.Tasks;
using Digst.OioIdws.Soap.Behaviors;
using Digst.OioIdws.Soap.StrCustomization;

namespace Digst.OioIdws.Soap.Bindings
{


    public class SoapBinding : CustomBinding
    {
        /// <summary>
        /// True specifies that transport layer security is required.
        /// </summary>
        internal bool UseHttps { get; set; } = true;

        /// <summary>
        /// True specifies that STRTransform is used.
        /// </summary>
        internal bool UseSTRTransform { get; set; } = true;
        
        /// <summary>
        /// Specifies max size of message recieved in bytes. If not set, default value on <see cref="TransportBindingElement.MaxReceivedMessageSize"/> are used.
        /// </summary>
        internal int? MaxReceivedMessageSize { get; set; }

        public override BindingElementCollection CreateBindingElements()
        {
            var transport =
                UseHttps
                    ? new HttpsTransportBindingElement() 
                    : new HttpTransportBindingElement();

            if (MaxReceivedMessageSize.HasValue)
            {
                transport.MaxReceivedMessageSize =
                    MaxReceivedMessageSize.Value;
            }

            
            var encoding = new TextMessageEncodingBindingElement();
            // [OIO IDWS SOAP 1.1] requires SOAP 1.2 and WS-Adressing 1.0
            encoding.MessageVersion = MessageVersion.Soap12WSAddressing10;

            // AlwaysToInitiator is required by the [OIO IDWS SOAP 1.1] profile. This specifies that the server certificate must be embedded in the response.
            var recipientTokenParameters = new X509SecurityTokenParameters(
                X509KeyIdentifierClauseType.Any,
                SecurityTokenInclusionMode.AlwaysToInitiator);

            var initiatorTokenParameters =
                new CustomizedIssuedSecurityTokenParameters(
                    "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0"
                )
                {
                    UseStrTransform = UseSTRTransform,
                    
                };

            //Local Java STS: false, NemLog-in STS: true

            var asymmetric = new AsymmetricSecurityBindingElement(recipientTokenParameters, initiatorTokenParameters)
            {
            };

            //DefaultAlgorithmSuite not set for NemLog-in STS but should always be Basic256Sha256
            asymmetric.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256Sha256; 

            // Must be true in order for client to accept embedded server certificates instead of references. This is required by the [OIO IDWS SOAP 1.1] profile.
            // However, the client must still specify the server certificate explicitly.
            // Have not figured out how the client can use the embedded server certificate and make trust to it through a CA certificate and a CN (Common Name). This way the client should not need the server certificate.
            asymmetric.AllowSerializedSigningTokenOnReply = true;

            // No need for derived keys when both parties has a certificate. Also OIO-IDWS-SOAP does not make use of derived keys.
            asymmetric.SetKeyDerivation(false);

            // Include token (encrypted assertion from NemLog-in STS) in signature
            asymmetric.ProtectTokens = true;

            // Specifies that WCF can send and receive unsecured responses to secured requests.
            // Concrete this means that SOAP faults are send unencrypted. [OIO IDWS SOAP 1.1] does not specify whether or not SOAP faults can be encrypted but it looks like they should not be encrypted.
            // If encrypted the client is not able to process the encrypted SOAP fault if client is not setup correctly.
            // setting EnableUnsecuredResponse to true makes normal responses unsigned and processed by the client without error. This is not what we want :)
            //asymmetric.EnableUnsecuredResponse = true;

            var elements = new BindingElementCollection();
            elements.Add(asymmetric);
            elements.Add(encoding);
            elements.Add(transport);

            return elements;
        }
    }
}