namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Trust
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
    )]
    public sealed class Trust13Policy
                : BindingNameItem<Trust13>
    {
        RequireServerEntropy requireServerEntropy;
        MustSupportIssuedTokens mustSupportIssuedTokens;
        RequireClientEntropy requireClientEntropy;

        [XmlElement]
        public RequireServerEntropy RequireServerEntropy
        {
            get { return this.requireServerEntropy; }
            set { this.requireServerEntropy = value; }
        }
        [XmlElement]
        public MustSupportIssuedTokens MustSupportIssuedTokens
        {
            get { return this.mustSupportIssuedTokens; }
            set { this.mustSupportIssuedTokens = value; }
        }
        [XmlElement]
        public RequireClientEntropy RequireClientEntropy
        {
            get { return this.requireClientEntropy; }
            set { this.requireClientEntropy = value; }
        }
    }
}