namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Trust
{
    using System.Xml;
    using System.Xml.Serialization;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;

    [XmlType(
        Namespace = "http://www.w3.org/ns/ws-policy"
    )]
    public sealed class Trust13 : BindingNameItem<SecurityPolicy>
    {
        BindingCollection<Trust13Policy, Trust13> nestedPolicy;

        [XmlElement(
            ElementName = "Policy"
        )]
        public BindingCollection<Trust13Policy, Trust13> NestedPolicy
        {
            get
            {
                if (nestedPolicy == null) nestedPolicy =
                        new BindingCollection<Trust13Policy, Trust13>(this);
                return nestedPolicy;
            }
        }
    }
}