namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Addressing
{
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies;

    [XmlType(
        Namespace = "http://www.w3.org/2006/05/addressing/wsdl"
    )]
    public sealed class UsingAddressing : BindingNameItem<All>
    { }
}