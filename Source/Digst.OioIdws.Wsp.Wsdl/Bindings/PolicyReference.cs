namespace Digst.OioIdws.Wsp.Wsdl.Bindings
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using WSC = System.Web.Services.Configuration;
    using WSD = System.Web.Services.Description;

    [WSC.XmlFormatExtension(
        "PolicyReference",
        "http://www.w3.org/ns/ws-policy",
        new Type[] {
                typeof(WSD.Binding),
                typeof(WSD.InputBinding),
                typeof(WSD.OutputBinding),
                typeof(WSD.FaultBinding)
        }
    )]
    [WSC.XmlFormatExtensionPrefix(
        "wsp",
        "http://www.w3.org/ns/ws-policy"
    )]
    public class PolicyReference :
        WSD.ServiceDescriptionFormatExtension
    {
        string uri;

        [XmlAttribute("URI")]
        public string Uri
        {
            get { return uri ?? string.Empty; }
            set { uri = value; }
        }
    }
}