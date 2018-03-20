namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Transport.Tokens
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
    )]
    public sealed class TransportTokenPolicy : BindingNameItem<TransportToken>
    {
        BindingCollection<HttpsToken, TransportTokenPolicy> httpsToken;

        [XmlElement]
        public BindingCollection<HttpsToken, TransportTokenPolicy> HttpsToken
        {
            get
            {
                if (httpsToken == null) httpsToken =
                        new BindingCollection<HttpsToken, TransportTokenPolicy>(this);
                return httpsToken;
            }
        }
    }
}

