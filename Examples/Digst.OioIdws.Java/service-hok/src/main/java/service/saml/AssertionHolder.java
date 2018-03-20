package service.saml;

import service.bpp.PrivilegeListType;

public class AssertionHolder {
	private static final ThreadLocal<PrivilegeListType> privileges = new ThreadLocal<>();

	public static void set(PrivilegeListType privilege) {
		privileges.set(privilege);
	}
	
	public static PrivilegeListType get() {
		return privileges.get();
	}

	public static void clear() {
		privileges.remove();
	}
}
