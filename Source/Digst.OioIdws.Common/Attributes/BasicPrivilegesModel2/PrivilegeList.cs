using System.Collections.Generic;
using System.Xml.Serialization;

namespace Digst.OioIdws.Common.Attributes.BasicPrivilegesModel2
{
    [XmlRoot("PrivilegeList", Namespace = "http://itst.dk/oiosaml/basic_privilege_profile")]
    public class PrivilegeList
    {
        [XmlElement("PrivilegeGroup", Namespace = "http://itst.dk/oiosaml/basic_privilege_profile")]
        public List<PrivilegeGroup> PrivilegeGroups { get; set; }
    }
}