package client;

//import org.example.contract.helloworld.HelloWorldPortType;
//import org.example.contract.helloworld.HelloWorldService;

import org.tempuri.HelloWorld;
import org.tempuri.IHelloWorld;

public class WSClient {
    
    public static void main (String[] args) {
    	// This is a hack to support the self-signed SSL certificate used on the WSP
    	// in a real production setting, the service would be protected by a trusted SSL certificate
    	// and setting a custom truststore like this would not be needed
        System.setProperty("javax.net.ssl.trustStore", "src/main/resources/trust-ssl.jks");
        System.setProperty("javax.net.ssl.trustStorePassword", "Test1234");
    	
        HelloWorld service = new HelloWorld();
        IHelloWorld port = service.getSoapBindingIHelloWorld();
        
        // first call will also call the STS
    	hello(port, "John");
    	
    	// second call will reuse the cached token we got from the first call
    	hello(port, "Jane");
    }
    
    public static void hello(IHelloWorld port, String name) {
        String resp = port.helloNone(name);

        System.out.println(resp);
    }
}
