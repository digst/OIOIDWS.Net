package client.callback;

import java.io.IOException;

import javax.security.auth.callback.Callback;
import javax.security.auth.callback.CallbackHandler;
import javax.security.auth.callback.UnsupportedCallbackException;

import org.apache.cxf.ws.security.trust.claims.ClaimsCallback;
import org.w3c.dom.Element;

import dk.sds.samlh.model.Validate;
import dk.sds.samlh.model.resourceid.ResourceId;

public class ClaimsCallbackHandler implements CallbackHandler {

	@Override
	public void handle(Callback[] callbacks) throws IOException, UnsupportedCallbackException {
		for (int i = 0; i < callbacks.length; i++) {
			if (callbacks[i] instanceof ClaimsCallback) {
				ClaimsCallback callback = (ClaimsCallback) callbacks[i];
				callback.setClaims(createClaims());
			} else {
				throw new UnsupportedCallbackException(callbacks[i], "Unrecognized Callback");
			}
		}
	}

	private static Element createClaims() {
		ResourceId resourceIdClaim = ResourceId.builder()
				.oid("1.2.208.176.1.2")
				.patientId("2512484916")
				.build();

		try {
			return resourceIdClaim.generateClaim(Validate.YES);
		} catch (Exception ex) {
			ex.printStackTrace();
		}

		return null;
	}
}