namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Layouts
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702"
    )]
    public sealed class AsymmetricLayoutPolicy : BindingNameItem<AsymmetricLayout>
    {
        BindingCollection<AsymmetricStrict, AsymmetricLayoutPolicy> strict;

        [XmlElement(ElementName = "Strict")]
        public BindingCollection<AsymmetricStrict, AsymmetricLayoutPolicy> Strict
        {
            get
            {
                if (strict == null) strict =
                        new BindingCollection<AsymmetricStrict, AsymmetricLayoutPolicy>(this);
                return strict;
            }
        }
    }
}

