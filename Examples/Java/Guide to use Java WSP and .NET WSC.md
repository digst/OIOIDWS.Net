Guide to use Java WSP and .NET WSC
==================================

* `Java` WSC:
    * Get the latest version of `IDWS-JAVA-SOAP` from: 
        https://www.digitaliser.dk/resource/3914450
    * Locate `oioidws-java\oioidws-scenarios\service-hok\pom.xml`
    * And start the `Tomcat` server with `Maven`
      ```
      "C:\Program Files\Apache\maven\bin\mvn.cmd" tomcat7:run-war ^
        -f "..\..\..\..\Examples\Digst.OioIdws.Java\service-hok\pom.xml"
      ```
* `.NET` WSC:
    * Get `trunk` from https://svn.softwareborsen.dk/OIOIDWS/trunk
    * As the `Java WSP` doesn't expose the `WSDL` online, you will have to create
      the `Service Reference` with a static `WSDL` file which you can 
      retrieve from: 
      * `oioidws-java\oioidws-scenarios\service-hok\src\main\resources\HelloWorld-Hok.wsdl`
    * Update the `Service Reference` for `Digst.OioIdws.WscExample` and
      make the neccesary changes in code in order to comply with the new service
      * Replace:
        ```csharp
        Console.WriteLine(channelWithIssuedToken.HelloNone("Schultz")); // Even if the protection level is set to 'None' Digst.OioIdws.Wsc ensures that the body is always at least signed.
        Console.WriteLine(channelWithIssuedToken.HelloSign("Schultz"));
        Console.WriteLine(channelWithIssuedToken.HelloEncryptAndSign("Schultz"));

        ////Checking that SOAP faults can be read. SOAP faults are encrypted in Sign and EncryptAndSign mode if no special care is taken.
        try
        {
            channelWithIssuedToken.HelloSignError("Schultz");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        //Checking that SOAP faults can be read when only being signed. SOAP faults are only signed if special care is taken.
        try
        {
            channelWithIssuedToken.HelloSignErrorNotEncrypted("Schultz");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        ```
      * With:
        ```csharp
        var helloWorldRequestJohn = new HelloWorldRequest("John");
        Console.WriteLine(
            channelWithIssuedToken.HelloWorld(helloWorldRequestJohn).response
        );

        var helloWorldRequestJane = new HelloWorldRequest("Jane");
        Console.WriteLine(
            channelWithIssuedToken.HelloWorld(helloWorldRequestJane).response
        );

        try
        {
            // third call will trigger a SOAPFault
            var helloWorldRequest = new HelloWorldRequest("");
            Console.WriteLine(
                channelWithIssuedToken.HelloWorld(helloWorldRequest).response
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine("Expected SOAPFault caught: " + ex.Message);
        }
        ```
    * Update the `App.config` file:
      ````xml
      ...
      <!--<oioIdwsWcfConfiguration ... wspEndpointID="https://wsp.oioidws-net.dk">-->
      <oioIdwsWcfConfiguration ... wspEndpointID="https://wsp.itcrew.dk">
      ...
      <!--<clientCertificate findValue="0E6DBCC6EFAAFF72E3F3D824E536381B26DEECF5" ... />-->
      <clientCertificate findValue="96A26BF6E07DE6DB74E356472CCA4776FEC9B0DA" ... />
      ...
      <!--
      <add findValue="1F0830937C74B0567D6B05C07B6155059D9B10C7" 
           storeLocation="LocalMachine" 
           storeName="My" 
           targetUri="https://Digst.OioIdws.Wsp:9090/HelloWorld" 
           x509FindType="FindByThumbprint"/>
      -->
      <add findValue="85398FCF737FB76F554C6F2422CC39D3A35EC26F" 
           storeLocation="LocalMachine" 
           storeName="My" 
           targetUri="https://localhost:8443/HelloWorld/services/helloworld" 
           x509FindType="FindByThumbprint"/>
      ...
      <!--
      <client>
        <endpoint address="https://Digst.OioIdws.Wsp:9090/HelloWorld" 
                  behaviorConfiguration="SoapBehaviourConfiguration" 
                  binding="SoapBinding" 
                  bindingConfiguration="SoapBindingConfiguration" 
                  contract="HelloWorldProxy.IHelloWorld">
        <identity>
            <dns value="wsp.oioidws-net.dk TEST (funktionscertifikat)"/>
        </identity>
        </endpoint>
      </client>
      -->
      <client>
        <endpoint address="https://localhost:8443/HelloWorld/services/helloworld" 
                  behaviorConfiguration="SoapBehaviourConfiguration" 
                  binding="SoapBinding" 
                  bindingConfiguration="SoapBindingConfiguration" 
                  contract="HelloWorldProxy.HelloWorldPortType">
        <identity>
            <dns value="eID JAVA test (funktionscertifikat)"/>
        </identity>
        </endpoint>
      </client>
      ...
      ```
    * It's neccesary to install the client certificate in order to sign 
      messages. It can be found at:
      * `oioidws-scenarios\system-user-scenario\src\main\resources\client.pfx`
    * Run it. You should see:
      ```
      Before | Supported Security Protocol(s): Ssl3, Tls
      After  | Supported Security Protocol(s): Tls12
      Hello John
      Hello Jane
      Hello
      Press <Enter> to stop the service.
      ```
