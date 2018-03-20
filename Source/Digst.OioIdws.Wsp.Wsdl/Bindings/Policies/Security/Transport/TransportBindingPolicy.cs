namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Transport
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Transport.Algorithm;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Transport.Layouts;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Transport.Tokens;

    [XmlType(
        Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
    )]
    public sealed class TransportBindingPolicy
                : BindingNameItem<TransportBinding>
    {
        BindingCollection<TransportAlgorithmSuite, TransportBindingPolicy> algorithmSuite;
        BindingCollection<TransportLayout, TransportBindingPolicy> layout;
        BindingCollection<TransportToken, TransportBindingPolicy> transportToken;

        [XmlElement(ElementName = "AlgorithmSuite")]
        public BindingCollection<TransportAlgorithmSuite, TransportBindingPolicy> AlgorithmSuite
        {
            get
            {
                if (algorithmSuite == null) algorithmSuite =
                        new BindingCollection<TransportAlgorithmSuite, TransportBindingPolicy>(this);
                return algorithmSuite;
            }
        }
        [XmlElement(ElementName = "Layout")]
        public BindingCollection<TransportLayout, TransportBindingPolicy> Layout
        {
            get
            {
                if (layout == null) layout =
                        new BindingCollection<TransportLayout, TransportBindingPolicy>(this);
                return layout;
            }
        }
        [XmlElement]
        public BindingCollection<TransportToken, TransportBindingPolicy> TransportToken
        {
            get
            {
                if (transportToken == null) transportToken =
                        new BindingCollection<TransportToken, TransportBindingPolicy>(this);
                return transportToken;
            }
        }
    }
}