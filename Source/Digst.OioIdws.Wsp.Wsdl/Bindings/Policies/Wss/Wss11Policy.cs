namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Wss
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
    )]
    public sealed class Wss11Policy
                : BindingNameItem<Wss11>
    {
        MustSupportRefThumbprint mustSupportRefThumbprint;
        MustSupportRefKeyIdentifier mustSupportRefKeyIdentifier;
        MustSupportRefIssuerSerial mustSupportRefIssuerSerial;

        [XmlElement]
        public MustSupportRefThumbprint MustSupportRefThumbprint
        {
            get { return this.mustSupportRefThumbprint; }
            set { this.mustSupportRefThumbprint = value; }
        }
        [XmlElement]
        public MustSupportRefKeyIdentifier MustSupportRefKeyIdentifier
        {
            get { return this.mustSupportRefKeyIdentifier; }
            set { this.mustSupportRefKeyIdentifier = value; }
        }
        [XmlElement]
        public MustSupportRefIssuerSerial MustSupportRefIssuerSerial
        {
            get { return this.mustSupportRefIssuerSerial; }
            set { this.mustSupportRefIssuerSerial = value; }
        }
    }
}