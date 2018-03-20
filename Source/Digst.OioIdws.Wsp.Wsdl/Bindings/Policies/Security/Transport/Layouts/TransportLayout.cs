namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Transport.Layouts
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://www.w3.org/ns/ws-policy"
    )]
    public sealed class TransportLayout : BindingNameItem<TransportBindingPolicy>
    {
        BindingCollection<TransportLayoutPolicy, TransportLayout> nestedPolicy;

        [XmlElement(
            ElementName = "Policy"
        )]
        public BindingCollection<TransportLayoutPolicy, TransportLayout> NestedPolicy
        {
            get
            {
                if (nestedPolicy == null) nestedPolicy =
                        new BindingCollection<TransportLayoutPolicy, TransportLayout>(this);
                return nestedPolicy;
            }
        }
    }
}