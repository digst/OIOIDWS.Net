package service.saml;

import java.util.ArrayList;
import java.util.List;

import org.apache.wss4j.common.ext.WSSecurityException;
import org.apache.wss4j.common.saml.SamlAssertionWrapper;
import org.apache.wss4j.dom.handler.RequestData;
import org.apache.wss4j.dom.validate.Credential;
import org.apache.wss4j.dom.validate.SamlAssertionValidator;
import org.opensaml.saml2.core.Assertion;
import org.opensaml.saml2.core.Attribute;
import org.opensaml.saml2.core.AttributeStatement;
import org.opensaml.xml.XMLObject;

import dk.sds.samlh.model.Validate;
import dk.sds.samlh.model.oiobpp.PrivilegeList;
import dk.sds.samlh.model.resourceid.ResourceId;

public class XuaSamlAssertionValidator extends SamlAssertionValidator {
	// Hard-coded expected audience in token. Multiple values can be added if needed
	private List<String> audienceRestrictions = new ArrayList<String>() {
		private static final long serialVersionUID = 1L;

		{
			add("http://localhost:8080/service/hello");
		}
	};

	@Override
	public Credential validate(Credential credential, RequestData data) throws WSSecurityException {
		// Set the valid audiences for this request
		data.setAudienceRestrictions(audienceRestrictions);

		// Perform the actual validation
		Credential validatedCredential = super.validate(credential, data);

		// Extract the Basic Privilege Profile attribute and place it on a ThreadLocal for later use
		SamlAssertionWrapper samlAssertion = credential.getSamlAssertion();
		if (samlAssertion.getSaml2() != null) {
			Assertion saml2 = samlAssertion.getSaml2();

			for (AttributeStatement attributeStatement : saml2.getAttributeStatements()) {
				for (Attribute attribute : attributeStatement.getAttributes()) {
					parseAttribute(attribute);
				}
			}
		}

		return validatedCredential;
	}

	// samples on how to parse the attributes using SAML-H
	private void parseAttribute(Attribute attribute) throws WSSecurityException {
		if (attribute.getName().equals("dk:gov:saml:attribute:Privileges_intermediate")) {
			parseOIOBPP(attribute);
		}
		else if (attribute.getName().equals("oasis:names:tc:xacml:2.0:resource:resource-id")) {
			parseResourceId(attribute);
		}
		else if (attribute.getName().equals("dk:healthcare:saml:attribute:UserAuthorizations")) {
			parseUserAuthorization(attribute);
		}
	}

	// TEXT-Based attributes we can pull the value of directly
	private void parseResourceId(Attribute attribute) throws WSSecurityException {
		for (XMLObject attributeValue : attribute.getAttributeValues()) {
			if (!attributeValue.isNil()) {
				try {
					String element = attributeValue.getDOM().getTextContent();
					ResourceId resourceId = ResourceId.parse(element, Validate.YES);
					
					System.out.println("resourceId: " + resourceId.generate(Validate.YES));
				}
				catch (Exception ex) {
					throw new WSSecurityException(WSSecurityException.ErrorCode.FAILURE, ex);
				}
			}
		}
	}

	// ELEMENT-based attributes requires a bit more parsing, and we cannot do it
	// here, as it will break the signature which is validated later in the flow
	private void parseUserAuthorization(Attribute attribute) {
		UserAuthorizationHolder.set(attribute);
	}

	// TEXT-Based attributes we can pull the value of directly
	private void parseOIOBPP(Attribute attribute) throws WSSecurityException {
		for (XMLObject attributeValue : attribute.getAttributeValues()) {
			if (!attributeValue.isNil()) {
				try {
					String privilege = attributeValue.getDOM().getTextContent();
					PrivilegeList oiobpp = PrivilegeList.parse(privilege, Validate.YES);

					AssertionHolder.set(oiobpp);
				}
				catch (Exception ex) {
					throw new WSSecurityException(WSSecurityException.ErrorCode.FAILURE, ex);
				}
			}
		}
	}
}
