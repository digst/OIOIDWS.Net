namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.SignedSupport
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://www.w3.org/ns/ws-policy"
    )]
    public sealed class SignedSupportingTokens : BindingNameItem<All>
    {
        BindingCollection<SignedSupportingTokensPolicy, SignedSupportingTokens> nestedPolicy;

        [XmlElement(
            ElementName = "Policy"
        )]
        public BindingCollection<SignedSupportingTokensPolicy, SignedSupportingTokens> NestedPolicy
        {
            get
            {
                if (nestedPolicy == null) nestedPolicy =
                        new BindingCollection<SignedSupportingTokensPolicy, SignedSupportingTokens>(this);
                return nestedPolicy;
            }
        }
    }
}