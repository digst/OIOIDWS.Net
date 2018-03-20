package client.interceptor;

import java.util.List;
import java.util.UUID;

import javax.xml.bind.JAXBElement;
import javax.xml.bind.JAXBException;
import javax.xml.namespace.QName;

import org.apache.cxf.binding.soap.SoapMessage;
import org.apache.cxf.binding.soap.interceptor.AbstractSoapInterceptor;
import org.apache.cxf.binding.xml.XMLFault;
import org.apache.cxf.headers.Header;
import org.apache.cxf.interceptor.Fault;
import org.apache.cxf.jaxb.JAXBDataBinding;
import org.apache.cxf.phase.Phase;
import org.apache.cxf.ws.addressing.AttributedURIType;
import org.apache.cxf.ws.addressing.ObjectFactory;

public class STSAddressingInterceptor extends AbstractSoapInterceptor {
	public STSAddressingInterceptor() {
		super(Phase.PRE_PROTOCOL);
	}

	@Override
	public void handleMessage(SoapMessage message) throws Fault {
		List<Header> headers = message.getHeaders();
		
		ObjectFactory wsAddressingFactory = new ObjectFactory();

		AttributedURIType to = new AttributedURIType();
		AttributedURIType messageId = new AttributedURIType();
		AttributedURIType action = new AttributedURIType();

		to.setValue("https://signature.sts.nemlog-in.dk/");
		action.setValue("http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue");
		messageId.setValue("uuid:" + UUID.randomUUID().toString());

		JAXBElement<AttributedURIType> createTo = wsAddressingFactory.createTo(to);
		JAXBElement<AttributedURIType> msgId = wsAddressingFactory.createMessageID(messageId);
		JAXBElement<AttributedURIType> newAction = wsAddressingFactory.createAction(action);
		 
		JAXBDataBinding jaxbDataBinding = null;
		try {
			jaxbDataBinding = new JAXBDataBinding(AttributedURIType.class);
		}
		catch (JAXBException ex) {
			throw new XMLFault(ex.getMessage());
		}

		Header toHeader = new Header(new QName("http://www.w3.org/2005/08/addressing"), createTo, jaxbDataBinding);
		Header msgIdHeader = new Header(new QName("http://www.w3.org/2005/08/addressing"), msgId, jaxbDataBinding);
		Header actionHeader = new Header(new QName("http://www.w3.org/2005/08/addressing"), newAction, jaxbDataBinding);
					
		headers.add(actionHeader);
		headers.add(msgIdHeader);
		headers.add(toHeader);

		message.put(Header.HEADER_LIST, headers);
	}
}
