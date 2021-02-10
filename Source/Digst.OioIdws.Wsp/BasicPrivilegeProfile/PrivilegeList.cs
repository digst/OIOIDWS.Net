using System.Xml.Schema;
using System.Xml.Serialization;

namespace Digst.OioIdws.Wsp.BasicPrivilegeProfile
{
    public abstract class PrivilegeList
    {
        [XmlElementAttribute("PrivilegeGroup", Form = XmlSchemaForm.Unqualified)]
        public PrivilegeGroup[] PrivilegeGroups { get; set; }
    }

    [XmlRootAttribute("PrivilegeList", Namespace = "http://itst.dk/oiosaml/basic_privilege_profile", IsNullable = false)]
    public class PrivilegeList101 : PrivilegeList { }

    [XmlRootAttribute("PrivilegeList", Namespace = "http://digst.dk/oiosaml/basic_privilege_profile", IsNullable = false)]
    public class PrivilegeList102 : PrivilegeList { }
}