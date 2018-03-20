namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Wss
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://www.w3.org/ns/ws-policy"
    )]
    public sealed class Wss11 : BindingNameItem<SecurityPolicy>
    {
        BindingCollection<Wss11Policy, Wss11> nestedPolicy;

        [XmlElement(
            ElementName = "Policy"
        )]
        public BindingCollection<Wss11Policy, Wss11> NestedPolicy
        {
            get
            {
                if (nestedPolicy == null) nestedPolicy =
                        new BindingCollection<Wss11Policy, Wss11>(this);
                return nestedPolicy;
            }
        }
    }
}