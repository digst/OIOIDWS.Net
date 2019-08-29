package service.saml;

import dk.sds.samlh.model.oiobpp.PrivilegeList;

public class AssertionHolder {
	private static final ThreadLocal<PrivilegeList> privileges = new ThreadLocal<>();

	public static void set(PrivilegeList privilege) {
		privileges.set(privilege);
	}
	
	public static PrivilegeList get() {
		return privileges.get();
	}

	public static void clear() {
		privileges.remove();
	}
}
