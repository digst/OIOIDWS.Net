namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Algorithm;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Initiator;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Recipient;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Layouts;

    [XmlType(
        Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
    )]
    public sealed class AsymmetricBindingPolicy
                : BindingNameItem<AsymmetricBinding>
    {
        BindingCollection<InitiatorToken, AsymmetricBindingPolicy> initiatorToken;
        BindingCollection<RecipientToken, AsymmetricBindingPolicy> recipientToken;
        BindingCollection<AsymmetricAlgorithmSuite, AsymmetricBindingPolicy> algorithmSuite;
        BindingCollection<AsymmetricLayout, AsymmetricBindingPolicy> layout;

        ProtectTokens protectTokens;
        IncludeTimestamp includeTimestamp;
        OnlySignEntireHeadersAndBody onlySignEntireHeadersAndBody;

        [XmlElement]
        public BindingCollection<InitiatorToken, AsymmetricBindingPolicy> InitiatorToken
        {
            get
            {
                if (initiatorToken == null) initiatorToken =
                        new BindingCollection<InitiatorToken, AsymmetricBindingPolicy>(this);
                return initiatorToken;
            }
        }
        [XmlElement]
        public BindingCollection<RecipientToken, AsymmetricBindingPolicy> RecipientToken
        {
            get
            {
                if (recipientToken == null) recipientToken =
                        new BindingCollection<RecipientToken, AsymmetricBindingPolicy>(this);
                return recipientToken;
            }
        }
        [XmlElement(ElementName = "AlgorithmSuite")]
        public BindingCollection<AsymmetricAlgorithmSuite, AsymmetricBindingPolicy> AlgorithmSuite
        {
            get
            {
                if (algorithmSuite == null) algorithmSuite =
                        new BindingCollection<AsymmetricAlgorithmSuite, AsymmetricBindingPolicy>(this);
                return algorithmSuite;
            }
        }
        [XmlElement(ElementName = "Layout")]
        public BindingCollection<AsymmetricLayout, AsymmetricBindingPolicy> Layout
        {
            get
            {
                if (layout == null) layout =
                        new BindingCollection<AsymmetricLayout, AsymmetricBindingPolicy>(this);
                return layout;
            }
        }

        [XmlElement]
        public ProtectTokens ProtectTokens
        {
            get { return this.protectTokens; }
            set { this.protectTokens = value; }
        }
        [XmlElement]
        public IncludeTimestamp IncludeTimestamp
        {
            get { return this.includeTimestamp; }
            set { this.includeTimestamp = value; }
        }
        [XmlElement]
        public OnlySignEntireHeadersAndBody OnlySignEntireHeadersAndBody
        {
            get { return this.onlySignEntireHeadersAndBody; }
            set { this.onlySignEntireHeadersAndBody = value; }
        }
    }
}