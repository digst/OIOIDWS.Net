namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Transport.Tokens
{
    using System.Xml.Serialization;
    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://www.w3.org/ns/ws-policy"
    )]
    public sealed class HttpsToken : BindingNameItem<TransportTokenPolicy>
    {
        private bool requireClientCertificate = false;

        [XmlAttribute]
        public bool RequireClientCertificate
        {
            get { return requireClientCertificate; }
            set { requireClientCertificate = value; }
        }
    }
}