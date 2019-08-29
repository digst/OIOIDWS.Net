package service.saml;

import org.opensaml.Configuration;
import org.opensaml.saml2.core.Attribute;
import org.opensaml.xml.XMLObject;
import org.opensaml.xml.io.Marshaller;
import org.opensaml.xml.io.MarshallerFactory;
import org.w3c.dom.Element;

import dk.sds.samlh.model.Validate;
import dk.sds.samlh.model.userauthorization.UserAuthorizationList;

public class UserAuthorizationHolder {
	private static final ThreadLocal<Attribute> userAuthorization = new ThreadLocal<>();

	public static void set(Attribute auth) {
		userAuthorization.set(auth);
	}
	
	public static UserAuthorizationList get() {
		Attribute attribute = userAuthorization.get();
		
		if (attribute != null) {
			try {
				XMLObject attributeValue = attribute.getAttributeValues().get(0).getOrderedChildren().get(0);
				MarshallerFactory marshallerFactory = Configuration.getMarshallerFactory();
				Marshaller marshaller = marshallerFactory.getMarshaller(attribute);
				
				Element element = marshaller.marshall(attributeValue);

				return UserAuthorizationList.parse(element, Validate.YES);
			}
			catch (Exception ex) {
				System.out.println("Failed to parse UserAuthorization");
				ex.printStackTrace();
			}
		}
		
		return null;
	}

	public static void clear() {
		userAuthorization.remove();
	}
}
