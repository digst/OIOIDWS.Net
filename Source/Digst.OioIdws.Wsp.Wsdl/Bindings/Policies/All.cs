namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Addressing;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.SignedSupport;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Trust;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Wss;

    public sealed class All : BindingNameItem<ExactlyOne>
    {
        UsingAddressing usingAddressing;
        BindingCollection<SecurityPolicy, All> nestedPolicy;

        SignedSupportingTokens signedSupportingTokens;

        Trust13 trust13;
        Wss11 wss11;

        [XmlElement(
            Namespace = "http://www.w3.org/2006/05/addressing/wsdl"
        )]
        public UsingAddressing UsingAddressing
        {
            get { return this.usingAddressing; }
            set { this.usingAddressing = value; }
        }
        [XmlElement(ElementName = "ExactlyOne")]
        public BindingCollection<SecurityPolicy, All> NestedPolicy
        {
            get
            {
                if (nestedPolicy == null) nestedPolicy =
                        new BindingCollection<SecurityPolicy, All>(this);
                return nestedPolicy;
            }
        }

        [XmlElement(
            Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
        )]
        public SignedSupportingTokens SignedSupportingTokens
        {
            get { return this.signedSupportingTokens; }
            set { this.signedSupportingTokens = value; }
        }

        [XmlElement(
            Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
        )]
        public Trust13 Trust13
        {
            get { return this.trust13; }
            set { this.trust13 = value; }
        }
        [XmlElement(
            Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
        )]
        public Wss11 Wss11
        {
            get { return this.wss11; }
            set { this.wss11 = value; }
        }
    }
}