namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Transport.Tokens
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://www.w3.org/ns/ws-policy"
    )]
    public sealed class TransportToken : BindingNameItem<TransportBindingPolicy>
    {
        BindingCollection<TransportTokenPolicy, TransportToken> nestedPolicy;

        [XmlElement(
            ElementName = "Policy"
        )]
        public BindingCollection<TransportTokenPolicy, TransportToken> NestedPolicy
        {
            get
            {
                if (nestedPolicy == null) nestedPolicy =
                        new BindingCollection<TransportTokenPolicy, TransportToken>(this);
                return nestedPolicy;
            }
        }
    }
}

