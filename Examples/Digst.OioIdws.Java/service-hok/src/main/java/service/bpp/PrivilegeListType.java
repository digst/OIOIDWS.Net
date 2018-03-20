package service.bpp;

import java.util.ArrayList;
import java.util.List;

import javax.xml.bind.annotation.XmlAccessType;
import javax.xml.bind.annotation.XmlAccessorType;
import javax.xml.bind.annotation.XmlElement;
import javax.xml.bind.annotation.XmlType;

@XmlAccessorType(XmlAccessType.FIELD)
@XmlType(name = "PrivilegeListType", propOrder = { "privilegeGroup" })
public class PrivilegeListType {

	@XmlElement(name = "PrivilegeGroup", required = true)
	protected List<PrivilegeGroupType> privilegeGroup;

	public List<PrivilegeGroupType> getPrivilegeGroup() {
		if (privilegeGroup == null) {
			privilegeGroup = new ArrayList<PrivilegeGroupType>();
		}

		return this.privilegeGroup;
	}
}
