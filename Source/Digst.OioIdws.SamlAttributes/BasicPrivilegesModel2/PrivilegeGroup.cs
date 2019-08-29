using System.Collections.Generic;
using System.Xml.Serialization;

namespace Digst.OioIdws.SamlAttributes.BasicPrivilegesModel2
{
    public class PrivilegeGroup
    {
        [XmlAttribute]
        public string Scope { get; set; }

        [XmlElement("Privilege", Namespace = "http://itst.dk/oiosaml/basic_privilege_profile")]
        public List<string> Privileges { get; set; }
    }
}