namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Common
{
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    public sealed class RequestSecurityTokenTemplate
        : BindingNameItem<object>
    {
        RequestSecurityTokenTemplateKeyType keyType;
        RequestSecurityTokenTemplateTokenType tokenType;

        [XmlElement(
            Namespace = "http://www.w3.org/2006/05/addressing/wsdl"
        )]
        public RequestSecurityTokenTemplateKeyType KeyType
        {
            get { return this.keyType; }
            set { this.keyType = value; }
        }
        [XmlElement(
            Namespace = "http://www.w3.org/2006/05/addressing/wsdl"
        )]
        public RequestSecurityTokenTemplateTokenType TokenType
        {
            get { return this.tokenType; }
            set { this.tokenType = value; }
        }
    }
}