namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Initiator.X509
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
    )]
    public sealed class InitiatorX509TokenPolicy : BindingNameItem<InitiatorX509Token>
    {
        BindingCollection<InitiatorWssX509V3Token10, InitiatorX509TokenPolicy> wssX509V3Token10;

        [XmlElement(ElementName = "WssX509V3Token10")]
        public BindingCollection<InitiatorWssX509V3Token10, InitiatorX509TokenPolicy> WssX509V3Token10
        {
            get
            {
                if (wssX509V3Token10 == null) wssX509V3Token10 =
                        new BindingCollection<InitiatorWssX509V3Token10, InitiatorX509TokenPolicy>(this);
                return wssX509V3Token10;
            }
        }
    }
}

