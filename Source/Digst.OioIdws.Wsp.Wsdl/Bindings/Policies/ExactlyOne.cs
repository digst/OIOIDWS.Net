namespace Digst.OioIdws.Wsp.Wsdl.Bindings.Policies
{
    using System.Xml;
    using System.Xml.Serialization;

    public sealed class ExactlyOne : BindingNameItem<Policy>
    {
        BindingCollection<All, ExactlyOne> all;

        [XmlElement("All")]
        public BindingCollection<All, ExactlyOne> All
        {
            get
            {
                if (all == null) all =
                        new BindingCollection<All, ExactlyOne>(this);
                return all;
            }
        }
    }
}