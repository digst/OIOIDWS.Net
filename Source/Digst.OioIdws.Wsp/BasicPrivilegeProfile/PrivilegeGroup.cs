using System.Xml.Schema;
using System.Xml.Serialization;

namespace Digst.OioIdws.Wsp.BasicPrivilegeProfile
{
    public class PrivilegeGroup
    {
        [XmlAttribute] public string Scope { get; set; }

        [XmlElement("Privilege", Form = XmlSchemaForm.Unqualified)]
        public string[] Privilege { get; set; }
    }
}