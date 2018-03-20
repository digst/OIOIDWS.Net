package client;

import javax.xml.ws.soap.SOAPFaultException;

import org.tempuri.HelloWorld;
import org.tempuri.IHelloWorld;
import org.tempuri.IHelloWorldHelloSignErrorNotEncryptedStringFaultFaultMessage;

public class WSClient {
    
    private static String trustStoreJavaX = "javax.net.ssl.trustStore";
    private static String trustStorePath = "src/main/resources/trust-ssl.jks";
    private static String trustStorePassword = "Test1234";

    public static void main (String[] args) {

        // This is a hack to support the self-signed SSL certificate used on the
        // WSP in a real production setting, the service would be protected by 
        // a trusted SSL certificate and setting a custom truststore like this 
        // would not be needed
        System.setProperty(trustStoreJavaX, trustStorePath);
        System.setProperty(trustStoreJavaX + "Password", trustStorePassword);
        
        HelloWorld service = new HelloWorld();
        IHelloWorld port = service.getSoapBindingIHelloWorld();

        // first call will also call the STS
        System.out.println(port.helloNone("Schultz")); // Even if the protection level is set to 'None' Digst.OioIdws.Wsc ensures that the body is always at least signed.
    	// second call will reuse the cached token we got from the first call
        System.out.println(port.helloSign("Schultz"));
        
        // Encrypted calls fails client side. However, encryption at message 
        // level is not required and no further investigation has been putted 
        // into this issue yet.
        // Source: Digst.OioIdws.WscJavaExample
    	try {
            System.out.println(port.helloEncryptAndSign("Schultz"));
    	}
    	catch (SOAPFaultException ex) {
            System.out.println(ex.getMessage());
    	}

        // Checking that SOAP faults can be read. SOAP faults are encrypted in 
        // Sign and EncryptAndSign mode if no special care is taken.
    	try {
            // System.out.println(port.helloSignError("Schultz"));
    	}
    	catch (SOAPFaultException ex) {
            System.out.println(ex.getMessage());
    	}
        
        // Checking that SOAP faults can be read when only being signed. SOAP 
        // faults are only signed if special care is taken.
    	try {
            System.out.println(port.helloSignErrorNotEncrypted("Schultz"));
    	}
    	catch (IHelloWorldHelloSignErrorNotEncryptedStringFaultFaultMessage ex) {
            System.out.println(ex.getMessage());
    	}
    }
}