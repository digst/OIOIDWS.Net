Guide to use .NET WSP (+custom WSDL) and Java WSC
=================================================

* `.NET` WSP:
    * Get `trunk` from https://svn.softwareborsen.dk/OIOIDWS/trunk
    * Add existing project `Digst.OioIdws.Wsp.Wsdl` under `Source`
    * Add a project reference from `Examples > Digst.OioIdws.WspExample` to
        `Source > Digst.OioIdws.Wsp.Wsdl`
    * In `Examples > Digst.OioIdws.WspExample` update:
        * `IHelloWorld.cs`:
            ```csharp
            namespace Digst.OioIdws.WspExample
            {
                [ServiceContract]
                //[WsdlExportExtension(Token = TokenType.HolderOfKey)]
                //[WsdlExportExtension(Token = TokenType.Bearer)]
                [WsdlExportExtension] // Default value -> (Token = TokenType.HolderOfKey)
                public interface IHelloWorld
                {
            ```
        * `App.config`:
            ```xml
                </system.identityModel>
                <!-- WSDL Export Extension Types -->
                <system.web>
                <webServices>
                    <serviceDescriptionFormatExtensionTypes>
                    <add type="Digst.OioIdws.Wsp.Wsdl.Bindings.Policy,          Digst.OioIdws.Wsp.Wsdl"/>
                    <add type="Digst.OioIdws.Wsp.Wsdl.Bindings.PolicyReference, Digst.OioIdws.Wsp.Wsdl"/>
                    </serviceDescriptionFormatExtensionTypes>
                </webServices>
                </system.web>
            </configuration>
            ```
    * Build the solution and set `Examples > Digst.OioIdws.WspExample` as
        startup project
    * Run it. You should see:
        ```
        Supported Security Protocol(s): Ssl3, Tls
        The service is ready at https://Digst.OioIdws.Wsp:9090/HelloWorld
        Press <Enter> to stop the service.
        ```
* `Java` WSC:
    * Get the latest version of `IDWS-JAVA-SOAP` from: 
        https://www.digitaliser.dk/resource/3914450
    * Unpack `ZIP` file locally and create a `Maven` project from `POM.xml` 
        file placed in the root of the folder (ex: `Netbeans` or `Eclipse`)
    * In order to make it work with `Examples > Digst.OioIdws.WspExample`
        make the following changes to:
        * `oioidws-scenarios\system-user-scenario\pom.xml`
            ```xml
            <!-- Allow local trusted store to build online from WSDL over HTTPS -->
            <configuration>
              <properties>
                <property>
                  <name>javax.net.ssl.trustStore</name>
                  <value>${basedir}/src/main/resources/trust-ssl.jks</value>
                </property>
                <property>
                  <name>javax.net.ssl.trustPassword</name>
                  <value>Test1234</value>
                </property>
              </properties>
            </configuration>
            ...
            <!-- Generate Java classes from WSDL during build -->
            <wsdlOptions>
                <wsdlOption>
                <!-- Use online build version -->
                <wsdl>
                    https://digst.oioidws.wsp:9090/HelloWorld?wsdl
                </wsdl>
                <!--
                <wsdl>
                    ${basedir}/src/main/resources/HelloWorld-Hok.wsdl
                </wsdl>
                <wsdlLocation>classpath:HelloWorld-Hok.wsdl</wsdlLocation>
                -->
                </wsdlOption>
            </wsdlOptions>
            ```
        * `oioidws-scenarios\system-user-scenario\src\main\resources\cxf.xml`
            ```xml
            <!--
            <jaxws:client name="{http://www.example.org/contract/HelloWorld}HelloWorldPort" createdFromAPI="true">
            -->
            <jaxws:client name="{http://tempuri.org/}SoapBinding_IHelloWorld" createdFromAPI="true">
            ...
            <!--
            <entry key="ws-security.sts.applies-to" value="https://wsp.itcrew.dk" />
            -->
            <entry key="ws-security.sts.applies-to" value="https://wsp.oioidws-net.dk" />
            <!--
            <entry key="ws-security.signature.username" value="java ref. test (funktionscertifikat)" />
            -->
            <entry key="ws-security.signature.username" value="oiosaml-net.dk test (funktionscertifikat)" />
            ...
            <!-- this is used to sign the request to the STS -->
            <!--
            <entry key="ws-security.signature.username" value="java ref. test (funktionscertifikat)" />
            -->
            <entry key="ws-security.signature.username" value="oiosaml-net.dk test (funktionscertifikat)" />
            ```
        * `oioidws-scenarios\system-user-scenario\src\main\resources\client.properties`
            ```text
            org.apache.ws.security.crypto.merlin.keystore.alias=oiosaml-net.dk test (funktionscertifikat)
            org.apache.ws.security.crypto.merlin.file=trust-wsc.jks
            ...
            org.apache.ws.security.crypto.merlin.truststore.file=trust-sts.jks
            ```
        * `oioidws-scenarios\system-user-scenario\src\main\resources\sts.properties`
            ```text
            org.apache.ws.security.crypto.merlin.file=trust-sts.jks
            ```
        * `oioidws-scenarios\system-user-scenario\src\main\java\client\WSClient.java`
            ```Java
            // import org.example.contract.helloworld.HelloWorldPortType;
            // import org.example.contract.helloworld.HelloWorldService;
            import org.tempuri.HelloWorld;
            import org.tempuri.IHelloWorld;

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

    	            // HelloWorldService service = new HelloWorldService();
    	            // HelloWorldPortType port = service.getHelloWorldPort();
                    HelloWorld service = new HelloWorld();
                    IHelloWorld port = service.getLibBasBindingIHelloWorld();

                    // first call will also call the STS
    	            hello(port, "John");
    	
    	            // second call will reuse the cached token we got from the first call
    	            hello(port, "Jane");

                    // third call will trigger a SOAPFault
    	            try {
                        hello(port, "");
                        System.out.println("Call did not fail as expected!");
    	            }
    	            catch (SOAPFaultException ex) {
                        System.out.println("Expected SOAPFault caught: " + ex.getMessage());
    	            }
                }
    
                // public static void hello(HelloWorldPortType port, String name) {
                public static void hello(IHelloWorld port, String name) {
                    // String resp = port.helloWorld(name);
                    String resp = port.helloNone(name);
                    System.out.println(resp);
                }
            }
            ```
    * In order to create the correct `trust stores`, use the following script: 
      `OIOIDWS\trunk\Examples\Digst.OioIdws.Java\certs\create_trust_stores.cmd`
      which will produce the following three files:
        * `trust-ssl.jks`
        * `trust-sts.jks`
        * `trust-wsc.jks`
    * Built the solution and run it