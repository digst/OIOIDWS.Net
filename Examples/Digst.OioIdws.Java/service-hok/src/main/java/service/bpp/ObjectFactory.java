package service.bpp;

import javax.xml.bind.JAXBElement;
import javax.xml.bind.annotation.XmlElementDecl;
import javax.xml.bind.annotation.XmlRegistry;
import javax.xml.namespace.QName;

@XmlRegistry
public class ObjectFactory {

    private final static QName _PrivilegeList_QNAME = new QName("http://itst.dk/oiosaml/basic_privilege_profile", "PrivilegeList");

    public ObjectFactory() { }

    public PrivilegeListType createPrivilegeListType() {
        return new PrivilegeListType();
    }

    public PrivilegeGroupType createPrivilegeGroupType() {
        return new PrivilegeGroupType();
    }

    @XmlElementDecl(namespace = "http://itst.dk/oiosaml/basic_privilege_profile", name = "PrivilegeList")
    public JAXBElement<PrivilegeListType> createPrivilegeList(PrivilegeListType value) {
        return new JAXBElement<PrivilegeListType>(_PrivilegeList_QNAME, PrivilegeListType.class, null, value);
    }
}
