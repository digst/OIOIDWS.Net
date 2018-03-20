namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Common
{
    using System.Xml.Serialization;
    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://www.w3.org/ns/ws-policy"
    )]
    public sealed class RequestSecurityTokenTemplateKeyType :
        BindingNameItem<RequestSecurityTokenTemplate>
    {
        string text = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/SymmetricKey";

        [XmlText]
        public string Text
        {
            get { return this.text; }
            set { this.text = value; }
        }
    }
}