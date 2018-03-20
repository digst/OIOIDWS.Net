namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Transport.Algorithm
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://www.w3.org/ns/ws-policy"
    )]
    public sealed class TransportAlgorithmSuite : BindingNameItem<TransportBindingPolicy>
    {
        BindingCollection<TransportAlgorithmSuitePolicy, TransportAlgorithmSuite> nestedPolicy;

        [XmlElement(
            ElementName = "Policy"
        )]
        public BindingCollection<TransportAlgorithmSuitePolicy, TransportAlgorithmSuite> NestedPolicy
        {
            get
            {
                if (nestedPolicy == null) nestedPolicy =
                        new BindingCollection<TransportAlgorithmSuitePolicy, TransportAlgorithmSuite>(this);
                return nestedPolicy;
            }
        }
    }
}