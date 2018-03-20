namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Transport.Algorithm
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
    )]
    public sealed class TransportAlgorithmSuitePolicy : BindingNameItem<TransportAlgorithmSuite>
    {
        BindingCollection<TransportBasic256, TransportAlgorithmSuitePolicy> basic256;

        [XmlElement(ElementName = "Basic256")]
        public BindingCollection<TransportBasic256, TransportAlgorithmSuitePolicy> Basic256
        {
            get
            {
                if (basic256 == null) basic256 =
                        new BindingCollection<TransportBasic256, TransportAlgorithmSuitePolicy>(this);
                return basic256;
            }
        }
    }
}

