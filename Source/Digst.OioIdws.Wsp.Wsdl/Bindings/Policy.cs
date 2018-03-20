namespace Digst.OioIdws.Wsp.Wsdl.Bindings
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using WSC = System.Web.Services.Configuration;
    using WSD = System.Web.Services.Description;

    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies;

    [WSC.XmlFormatExtension(
        "Policy",
        "http://www.w3.org/ns/ws-policy",
        new Type[] {
                typeof(WSD.ServiceDescription)
        }
    )]
    public sealed class Policy :
        WSD.ServiceDescriptionFormatExtension
    {
        string id;
        BindingCollection<ExactlyOne, Policy> exactlyOne;

        [XmlAttribute(
            "Id",
            Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"
        )]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        [XmlElement("ExactlyOne")]
        public BindingCollection<ExactlyOne, Policy> ExactlyOne
        {
            get
            {
                if (exactlyOne == null) exactlyOne =
                        new BindingCollection<ExactlyOne, Policy>(this);
                return exactlyOne;
            }
        }
    }
}