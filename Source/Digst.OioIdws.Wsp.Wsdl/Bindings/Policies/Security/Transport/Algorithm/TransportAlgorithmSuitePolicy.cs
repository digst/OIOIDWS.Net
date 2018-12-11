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
        BindingCollection<TransportBasic256Sha256, TransportAlgorithmSuitePolicy> basic256Sha256;

        [XmlElement(ElementName = "Basic256Sha256")]
        public BindingCollection<TransportBasic256Sha256, TransportAlgorithmSuitePolicy> Basic256Sha256
        {
            get
            {
                if (basic256Sha256 == null) basic256Sha256 =
                        new BindingCollection<TransportBasic256Sha256, TransportAlgorithmSuitePolicy>(this);
                return basic256Sha256;
            }
        }
    }
}

