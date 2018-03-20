namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Initiator.X509
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://www.w3.org/ns/ws-policy"
    )]
    public sealed class InitiatorX509Token : BindingNameItem<InitiatorTokenPolicy>
    {
        string includeToken =
            "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702/IncludeToken/AlwaysToRecipient";
        BindingCollection<InitiatorX509TokenPolicy, InitiatorX509Token> nestedPolicy;

        [XmlAttribute(
            "IncludeToken",
            Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
        )]
        public string IncludeToken
        {
            get { return includeToken; }
            set { includeToken = value; }
        }

        [XmlElement(
            ElementName = "Policy"
        )]
        public BindingCollection<InitiatorX509TokenPolicy, InitiatorX509Token> NestedPolicy
        {
            get
            {
                if (nestedPolicy == null) nestedPolicy =
                        new BindingCollection<InitiatorX509TokenPolicy, InitiatorX509Token>(this);
                return nestedPolicy;
            }
        }
    }
}

