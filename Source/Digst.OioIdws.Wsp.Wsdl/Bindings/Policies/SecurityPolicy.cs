namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Transport;

    [XmlType(
        Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
    )]
    public sealed class SecurityPolicy : BindingNameItem<All>
    {
        BindingCollection<AsymmetricBinding, SecurityPolicy> asymmetricBinding;
        BindingCollection<TransportBinding, SecurityPolicy> transportBinding;

        [XmlElement(
            Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
        )]
        public BindingCollection<AsymmetricBinding, SecurityPolicy> AsymmetricBinding
        {
            get
            {
                if (asymmetricBinding == null) asymmetricBinding =
                        new BindingCollection<AsymmetricBinding, SecurityPolicy>(this);
                return asymmetricBinding;
            }
        }

        [XmlElement]
        public BindingCollection<TransportBinding, SecurityPolicy> TransportBinding
        {
            get
            {
                if (transportBinding == null) transportBinding =
                        new BindingCollection<TransportBinding, SecurityPolicy>(this);
                return transportBinding;
            }
        }
    }
}