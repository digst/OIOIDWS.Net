namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Initiator
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric;

    [XmlType(
        Namespace = "http://www.w3.org/ns/ws-policy"
    )]
    public sealed class InitiatorToken : BindingNameItem<AsymmetricBindingPolicy>
    {
        BindingCollection<InitiatorTokenPolicy, InitiatorToken> nestedPolicy;

        [XmlElement(
            ElementName = "Policy"
        )]
        public BindingCollection<InitiatorTokenPolicy, InitiatorToken> NestedPolicy
        {
            get
            {
                if (nestedPolicy == null) nestedPolicy =
                        new BindingCollection<InitiatorTokenPolicy, InitiatorToken>(this);
                return nestedPolicy;
            }
        }
    }
}