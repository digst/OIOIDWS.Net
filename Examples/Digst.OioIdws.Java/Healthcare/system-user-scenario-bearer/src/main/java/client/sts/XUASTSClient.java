package client.sts;

import java.security.cert.X509Certificate;
import java.util.Base64;

import org.apache.cxf.Bus;
import org.apache.cxf.staxutils.W3CDOMStreamWriter;
import org.apache.cxf.ws.security.trust.STSClient;

public class XUASTSClient extends STSClient {

	public XUASTSClient(Bus b) {
		super(b);
	}

	@Override
	protected void writeElementsForRSTPublicKey(W3CDOMStreamWriter writer, X509Certificate cert) throws Exception {
		writer.writeStartElement("wst", "UseKey", namespace);
		writer.writeStartElement("wsse", "BinarySecurityToken", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
		writer.writeAttribute("ValueType", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3");
		writer.writeAttribute("EncodingType", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary");
		writer.getCurrentNode().setTextContent(Base64.getEncoder().encodeToString(cert.getEncoded()));

		writer.writeEndElement();
		writer.writeEndElement();
	}	
}
