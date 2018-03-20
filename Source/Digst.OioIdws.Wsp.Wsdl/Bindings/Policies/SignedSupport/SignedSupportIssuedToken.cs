namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.SignedSupport
{
    using System.Collections;
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Common;

    [XmlType(
        Namespace = "http://www.w3.org/ns/ws-policy"
    )]
    public sealed class SignedSupportIssuedToken :
        BindingNameItem<SignedSupportingTokensPolicy>
    {
        string includeToken =
            "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702/IncludeToken/AlwaysToRecipient";
        RequestSecurityTokenTemplate requestSecurityTokenTemplate;

        ArrayList policy;

        [XmlAttribute(
            Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
        )]
        public string IncludeToken
        {
            get { return includeToken; }
            set { includeToken = value; }
        }

        [XmlElement(
            Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
        )]
        public RequestSecurityTokenTemplate RequestSecurityTokenTemplate
        {
            get { return this.requestSecurityTokenTemplate; }
            set { this.requestSecurityTokenTemplate = value; }
        }

        [XmlArray(
            ElementName = "Policy"
        )]
        public ArrayList Policy
        {
            get
            {
                if (policy == null) policy =
                      new ArrayList();
                return policy;
            }
        }
    }
}