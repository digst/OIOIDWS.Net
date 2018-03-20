namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://www.w3.org/ns/ws-policy"
    )]
    public sealed class AsymmetricBinding : BindingNameItem<SecurityPolicy>
    {
        BindingCollection<AsymmetricBindingPolicy, AsymmetricBinding> nestedPolicy;

        [XmlElement(
            ElementName = "Policy"
        )]
        public BindingCollection<AsymmetricBindingPolicy, AsymmetricBinding> NestedPolicy
        {
            get
            {
                if (nestedPolicy == null) nestedPolicy =
                        new BindingCollection<AsymmetricBindingPolicy, AsymmetricBinding>(this);
                return nestedPolicy;
            }
        }
    }
}