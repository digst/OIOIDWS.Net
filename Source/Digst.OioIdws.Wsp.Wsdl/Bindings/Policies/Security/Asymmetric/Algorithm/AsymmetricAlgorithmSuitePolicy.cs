namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Algorithm
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
    )]
    public sealed class AsymmetricAlgorithmSuitePolicy : BindingNameItem<AsymmetricAlgorithmSuite>
    {
        BindingCollection<AsymmetricBasic256Sha256, AsymmetricAlgorithmSuitePolicy> basic256Sha256;

        [XmlElement(ElementName = "Basic256Sha256")]
        public BindingCollection<AsymmetricBasic256Sha256, AsymmetricAlgorithmSuitePolicy> Basic256Sha256
        {
            get
            {
                if (basic256Sha256 == null) basic256Sha256 =
                        new BindingCollection<AsymmetricBasic256Sha256, AsymmetricAlgorithmSuitePolicy>(this);
                return basic256Sha256;
            }
        }
    }
}

