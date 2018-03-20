package service.bpp;

import java.util.ArrayList;
import java.util.List;

import javax.xml.bind.annotation.XmlAccessType;
import javax.xml.bind.annotation.XmlAccessorType;
import javax.xml.bind.annotation.XmlAttribute;
import javax.xml.bind.annotation.XmlElement;
import javax.xml.bind.annotation.XmlType;

@XmlAccessorType(XmlAccessType.FIELD)
@XmlType(name = "PrivilegeGroupType", propOrder = { "privilege" })
public class PrivilegeGroupType {

	@XmlElement(name = "Privilege", required = true)
	protected List<String> privilege;

	@XmlAttribute(name = "Scope", required = true)
	protected String scope;

	public List<String> getPrivilege() {
		if (privilege == null) {
			privilege = new ArrayList<String>();
		}

		return this.privilege;
	}

	public String getScope() {
		return scope;
	}

	public void setScope(String value) {
		this.scope = value;
	}
}
