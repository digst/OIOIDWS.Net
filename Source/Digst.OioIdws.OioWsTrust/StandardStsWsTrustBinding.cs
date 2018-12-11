using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Text;
using Digst.OioIdws.OioWsTrust.ProtocolChannel;

namespace Digst.OioIdws.OioWsTrust
{
    public class StandardStsWsTrustBinding : CustomBinding
    {
        public StandardStsWsTrustBinding(TimeSpan sendTimeout, X509Certificate2 stsCertificate)
        {
        }





        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override BindingElementCollection CreateBindingElements()
        {

            var security = SecurityBindingElement.CreateCertificateOverTransportBindingElement(MessageSecurityVersion.WSSecurity10WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10);
                

            var transport = new HttpsTransportBindingElement()
            {
                AuthenticationScheme = AuthenticationSchemes.Anonymous,
                Realm = "",
            };

            var encoding = new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, Encoding.UTF8)
            {
                
            };

            var coll = new BindingElementCollection
            {
                security,
                encoding,
                transport
            };

            return coll;
        }
    }
}