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
        BindingCollection<AsymmetricBasic256, AsymmetricAlgorithmSuitePolicy> basic256;

        [XmlElement(ElementName = "Basic256")]
        public BindingCollection<AsymmetricBasic256, AsymmetricAlgorithmSuitePolicy> Basic256
        {
            get
            {
                if (basic256 == null) basic256 =
                        new BindingCollection<AsymmetricBasic256, AsymmetricAlgorithmSuitePolicy>(this);
                return basic256;
            }
        }
    }
}

