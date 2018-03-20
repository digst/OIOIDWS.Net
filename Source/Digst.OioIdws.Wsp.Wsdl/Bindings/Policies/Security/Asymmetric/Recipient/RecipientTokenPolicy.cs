namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Recipient
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
    )]
    public sealed class RecipientTokenPolicy : BindingNameItem<RecipientToken>
    {
        BindingCollection<RecipientX509Token, RecipientTokenPolicy> x509Token;

        [XmlElement(ElementName = "X509Token")]
        public BindingCollection<RecipientX509Token, RecipientTokenPolicy> X509Token
        {
            get
            {
                if (x509Token == null) x509Token =
                        new BindingCollection<RecipientX509Token, RecipientTokenPolicy>(this);
                return x509Token;
            }
        }
    }
}