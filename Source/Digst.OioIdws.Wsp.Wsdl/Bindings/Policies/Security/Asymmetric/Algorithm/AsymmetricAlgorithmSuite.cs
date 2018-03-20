namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Algorithm
{
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://www.w3.org/ns/ws-policy"
    )]
    public sealed class AsymmetricAlgorithmSuite : BindingNameItem<AsymmetricBindingPolicy>
    {
        BindingCollection<AsymmetricAlgorithmSuitePolicy, AsymmetricAlgorithmSuite> nestedPolicy;

        [XmlElement(
            ElementName = "Policy"
        )]
        public BindingCollection<AsymmetricAlgorithmSuitePolicy, AsymmetricAlgorithmSuite> NestedPolicy
        {
            get
            {
                if (nestedPolicy == null) nestedPolicy =
                        new BindingCollection<AsymmetricAlgorithmSuitePolicy, AsymmetricAlgorithmSuite>(this);
                return nestedPolicy;
            }
        }
    }
}