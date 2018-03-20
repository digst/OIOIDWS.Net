namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Transport.Layouts
{
    using System.Xml.Serialization;
    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://www.w3.org/ns/ws-policy"
    )]
    public sealed class TransportStrict : BindingNameItem<TransportLayoutPolicy>
    { }
}