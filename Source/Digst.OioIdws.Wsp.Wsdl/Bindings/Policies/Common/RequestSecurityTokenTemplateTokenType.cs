namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Common
{
    using System.Xml.Serialization;
    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://www.w3.org/ns/ws-policy"
    )]
    public sealed class RequestSecurityTokenTemplateTokenType :
        BindingNameItem<RequestSecurityTokenTemplate>
    {
        string text = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0";

        [XmlText]
        public string Text
        {
            get { return this.text; }
            set { this.text = value; }
        }
    }
}