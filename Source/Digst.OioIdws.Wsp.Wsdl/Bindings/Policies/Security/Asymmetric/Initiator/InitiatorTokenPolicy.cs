namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Initiator
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Initiator.X509;

    [XmlType(
        Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
    )]
    public sealed class InitiatorTokenPolicy : BindingNameItem<InitiatorToken>
    {
        InitiatorIssuedToken issuedToken;

        BindingCollection<InitiatorX509Token, InitiatorTokenPolicy> x509Token;

        [XmlElement(ElementName = "IssuedToken")]
        public InitiatorIssuedToken IssuedToken
        {
            get { return issuedToken; }
            set { issuedToken = value; }
        }

        [XmlElement(ElementName = "X509Token")]
        public BindingCollection<InitiatorX509Token, InitiatorTokenPolicy> X509Token
        {
            get
            {
                if (x509Token == null) x509Token =
                        new BindingCollection<InitiatorX509Token, InitiatorTokenPolicy>(this);
                return x509Token;
            }
        }
    }
}