namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Transport.Layouts
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
    )]
    public sealed class TransportLayoutPolicy : BindingNameItem<TransportLayout>
    {
        BindingCollection<TransportStrict, TransportLayoutPolicy> strict;

        [XmlElement(ElementName = "Strict")]
        public BindingCollection<TransportStrict, TransportLayoutPolicy> Strict
        {
            get
            {
                if (strict == null) strict =
                        new BindingCollection<TransportStrict, TransportLayoutPolicy>(this);
                return strict;
            }
        }
    }
}

