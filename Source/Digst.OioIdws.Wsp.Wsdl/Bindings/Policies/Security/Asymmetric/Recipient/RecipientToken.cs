namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Recipient
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric;

    [XmlType(
        Namespace = "http://www.w3.org/ns/ws-policy"
    )]
    public sealed class RecipientToken : BindingNameItem<AsymmetricBindingPolicy>
    {
        BindingCollection<RecipientTokenPolicy, RecipientToken> nestedPolicy;

        [XmlElement(
            ElementName = "Policy"
        )]
        public BindingCollection<RecipientTokenPolicy, RecipientToken> NestedPolicy
        {
            get
            {
                if (nestedPolicy == null) nestedPolicy =
                        new BindingCollection<RecipientTokenPolicy, RecipientToken>(this);
                return nestedPolicy;
            }
        }
    }
}