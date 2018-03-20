namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Transport
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://www.w3.org/ns/ws-policy"
    )]
    public sealed class TransportBinding : BindingNameItem<SecurityPolicy>
    {
        BindingCollection<TransportBindingPolicy, TransportBinding> nestedPolicy;

        [XmlElement(
            ElementName = "Policy"
        )]
        public BindingCollection<TransportBindingPolicy, TransportBinding> NestedPolicy
        {
            get
            {
                if (nestedPolicy == null) nestedPolicy =
                        new BindingCollection<TransportBindingPolicy, TransportBinding>(this);
                return nestedPolicy;
            }
        }
    }
}