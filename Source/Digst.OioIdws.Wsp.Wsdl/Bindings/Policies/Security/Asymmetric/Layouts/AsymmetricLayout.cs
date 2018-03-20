namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Layouts
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://www.w3.org/ns/ws-policy"
    )]
    public sealed class AsymmetricLayout : BindingNameItem<AsymmetricBindingPolicy>
    {
        BindingCollection<AsymmetricLayoutPolicy, AsymmetricLayout> nestedPolicy;

        [XmlElement(
            ElementName = "Policy"
        )]
        public BindingCollection<AsymmetricLayoutPolicy, AsymmetricLayout> NestedPolicy
        {
            get
            {
                if (nestedPolicy == null) nestedPolicy =
                        new BindingCollection<AsymmetricLayoutPolicy, AsymmetricLayout>(this);
                return nestedPolicy;
            }
        }
    }
}