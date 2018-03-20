namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.SignedSupport
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
    )]
    public sealed class SignedSupportingTokensPolicy
        : BindingNameItem<SignedSupportingTokens>
    {
        BindingCollection<SignedSupportIssuedToken, SignedSupportingTokensPolicy> issuedToken;

        [XmlElement]
        public BindingCollection<SignedSupportIssuedToken, SignedSupportingTokensPolicy> IssuedToken
        {
            get
            {
                if (issuedToken == null) issuedToken =
                        new BindingCollection<SignedSupportIssuedToken, SignedSupportingTokensPolicy>(this);
                return issuedToken;
            }
        }
    }
}