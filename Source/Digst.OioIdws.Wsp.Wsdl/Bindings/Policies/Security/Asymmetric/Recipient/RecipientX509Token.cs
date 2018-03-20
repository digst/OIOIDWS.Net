namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Recipient
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://www.w3.org/ns/ws-policy"
    )]
    public sealed class RecipientX509Token : BindingNameItem<RecipientTokenPolicy>
    {
        string includeToken =
            //"http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702/IncludeToken/AlwaysToRecipient";
            "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702/IncludeToken/AlwaysToInitiator";
        BindingCollection<RecipientX509TokenPolicy, RecipientX509Token> nestedPolicy;

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
        public BindingCollection<RecipientX509TokenPolicy, RecipientX509Token> NestedPolicy
        {
            get
            {
                if (nestedPolicy == null) nestedPolicy =
                        new BindingCollection<RecipientX509TokenPolicy, RecipientX509Token>(this);
                return nestedPolicy;
            }
        }
    }
}

