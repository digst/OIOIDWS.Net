using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Digst.OioIdws.OioWsTrust.ProtocolChannel;

namespace Digst.OioIdws.OioWsTrust.Bindings
{
    public class OioWsTrustBinding : Binding
    {

        public string OverrideToAddress { get; set; }

        public X509Certificate2 StsCertificate { get; }

        public override BindingElementCollection CreateBindingElements()
        {
            var stsBinding = new BindingElementCollection
            {
                new OioWsTrustBindingElement(StsCertificate, new LocalWsTrustMessageTransformer(OverrideToAddress)),

                new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, Encoding.UTF8),

                new HttpsTransportBindingElement()
                {
                    ManualAddressing = (OverrideToAddress != null)
                }
            };
            return stsBinding;
        }

        public override string Scheme
        {
            get
            {
                TransportBindingElement transportBindingElement = CreateBindingElements().Find<TransportBindingElement>();
                if (transportBindingElement == null)
                    return string.Empty;
                return transportBindingElement.Scheme;
            }
        }
    }
}
