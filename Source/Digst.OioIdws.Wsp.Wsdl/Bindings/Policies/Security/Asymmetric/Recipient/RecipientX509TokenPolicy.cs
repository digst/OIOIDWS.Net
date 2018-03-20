namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Recipient
{
    using System.Collections;
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
    )]
    public sealed class RecipientX509TokenPolicy : BindingNameItem<RecipientX509Token>
    {
        BindingCollection<RecipientWssX509V3Token10, RecipientX509TokenPolicy> wssX509V3Token10;

        [XmlElement(ElementName = "WssX509V3Token10")]
        public BindingCollection<RecipientWssX509V3Token10, RecipientX509TokenPolicy> WssX509V3Token10
        {
            get
            {
                if (wssX509V3Token10 == null) wssX509V3Token10 =
                        new BindingCollection<RecipientWssX509V3Token10, RecipientX509TokenPolicy>(this);
                return wssX509V3Token10;
            }
        }
    }
}

