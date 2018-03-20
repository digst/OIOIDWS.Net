namespace Digst.OioIdws.Wsp.Wsdl.Bindings
{
    using System.Xml.Serialization;
    using WSD = System.Web.Services.Description;

    public class BindingNameItem<TParent> : WSD.NamedItem
    {
        TParent parent;
        WSD.ServiceDescriptionFormatExtensionCollection extensions;

        public BindingNameItem() : base() { }

        internal void SetParent(TParent parent)
        {
            this.parent = parent;
        }

        public TParent Parent
        {
            get { return parent; }
        }

        [XmlIgnore]
        public override WSD.ServiceDescriptionFormatExtensionCollection Extensions
        {
            get
            {
                if (extensions == null) extensions =
                        new WSD.ServiceDescriptionFormatExtensionCollection(this);
                return extensions;
            }
        }
    }
}