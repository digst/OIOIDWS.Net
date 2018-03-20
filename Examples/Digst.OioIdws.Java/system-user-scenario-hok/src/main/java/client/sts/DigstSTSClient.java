package client.sts;

import java.util.UUID;

import javax.xml.stream.XMLStreamException;
import javax.xml.stream.XMLStreamWriter;

import org.apache.cxf.Bus;
import org.apache.cxf.ws.security.trust.STSClient;

import client.interceptor.STSAddressingInterceptor;

public class DigstSTSClient extends STSClient {
	public DigstSTSClient(Bus b) {
		super(b);

		this.out.add(new STSAddressingInterceptor());

		createUniqueContextAttribute();
	}

	@Override
    protected void addAppliesTo(XMLStreamWriter writer, String appliesTo) throws XMLStreamException {
		createUniqueContextAttribute();

        if (appliesTo != null && addressingNamespace != null) {
        	// modified the namespace so it uses ws-policy 1.1 namespace (not supported by CXF, so we have to do this hacky thing)
            writer.writeStartElement("wsp", "AppliesTo", "http://schemas.xmlsoap.org/ws/2002/12/policy");
            writer.writeNamespace("wsp", "http://schemas.xmlsoap.org/ws/2002/12/policy");
            writer.writeStartElement("wsa", "EndpointReference", addressingNamespace);
            writer.writeNamespace("wsa", addressingNamespace);
            writer.writeStartElement("wsa", "Address", addressingNamespace);
            writer.writeCharacters(appliesTo);
            writer.writeEndElement();
            writer.writeEndElement();
            writer.writeEndElement();
        }
    }

	// we are required to set a unique @Context attribute on the request
	private void createUniqueContextAttribute() {
		this.context = "urn:uuid:" + UUID.randomUUID().toString();
	}
}
