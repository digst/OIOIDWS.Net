# OIOIDWS Quick Getting Started

## Package overview

* Digst.OioIdws.Wsc: Use this package when you are going to implement a web service consumer (WSC), i.e. a client of a identity based web service
* Digst.OioIdws.Wsp: Use this package when you are going to implement a web service provider (WSP), i.e. an identity based web service
* Digst.OioIdws.SamlAttributes: Use this package to send and receive strongly typed .NET claim values of common OIO SAML attributes.
* Digst.OioIdws.Healthcare.Wsc: Use this package in addition to Digst.OioIdws.Wsc when your consumer is a *healthcare* consumer. The package contains additional help for communicating with the STS.
* Digst.OioIdws.Healthcare.SamlAttributes: Use this package in addition to Digst.OioIdws.SamlAttributes when you are using the specific healthcare (HL-7) claims. The package contains additional attribute definitions for common healthcare attributes.
* Digst.OioIdws.Rest.Client: Use this to implement a REST based client.
* Digst.OioIdws.Rest.Server: Use this to implement a REST based service provider.


## Creating a Web Service Consumer (WSC)

To implement a web service consumer (web service client) you install the Nuget package:

    Install-Package Digst.OioIdws.Wsc

If you want to use the additional healthcare support you also install the healthcare WSC package:

    Install-Package Digst.OioIdws.Healthcare.Wsc

The latter package allow you to create Authentication (AUT) tokens and use request claims collections.

If you want to use the support for strongly typed SAML attributes, then also install the Digst.OioIdws.SamlAttributes package. The package contains SAML attribute definitions for common OIO attributes:

    Install-Package Digst.OioIdws.SamlAttributes

The package Digst.OioIdws.Healthcare.SamlAttributes contains additional SAML attribute marshallers for specific healthcare (HL-7) attributes. If you want strongly typed support for HL-7 SAML attributes then install this package also:

    Install-Package Digst.OioIdws.Healthcare.SamlAttributes


Now make sure to configure the service using the special OIOIDWS bindings. Notice the `SoapBinding` and `SoapBehavior` extensions. In the config file:


    <configSections>
        <section name="stsConfiguration" type="Digst.OioIdws.OioWsTrust.SecurityTokenServiceClientConfigurationSection,  Digst.OioIdws.OioWsTrust, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"/>
        <section name="oioIdwsLoggingConfiguration" type="Digst.OioIdws.Common.Logging.Configuration, Digst.OioIdws.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"/>
        <section name="system.identityModel" type="System.IdentityModel.Configuration.SystemIdentityModelSection, System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089"/>
    </configSections>

    ...

    <system.serviceModel>
        <extensions>
            <bindingExtensions>
                <add name="SoapBinding" type="Digst.OioIdws.Soap.Bindings.SoapBindingCollectionElement, Digst.OioIdws.Soap"/>
            </bindingExtensions>
            <behaviorExtensions>
                <add name="SoapBehavior" type="Digst.OioIdws.Soap.Behaviors.SoapClientBehaviorExtensionElement, Digst.OioIdws.Soap"/>
            </behaviorExtensions>
        </extensions>
        <behaviors>
            <endpointBehaviors>
                <behavior name="SoapBehaviourConfiguration">
                <clientCredentials useIdentityConfiguration="true">
                    <serviceCertificate> ... </serviceCertificate>
                </clientCredentials>
                <SoapBehavior/>
                </behavior>
            </endpointBehaviors>
        </behaviors>
        <bindings>
            <SoapBinding>
                <binding name="SoapBindingConfiguration" useHttps="true" useSTRTransform="true" />
            </SoapBinding>
        </bindings>
        <client>
            <endpoint address="...service url..."
                binding="SoapBinding" bindingConfiguration="SoapBindingConfiguration" contract="..." name="...">
                <identity>
                <dns value="..." />
                <certificate ... />
                </identity>
            </endpoint>
        </client>
    </system.serviceModel>


    <stsConfiguration
        stsIdentifier="http://sts.sundhedsdatastyrelsen.dk/"
        bootstrapTokenFromAuthenticationTokenUrl="..."
        identityTokenFromBootstrapTokenUrl="..."
        wscIdentifier="..." >
        <stsCertificate storeLocation="LocalMachine" storeName="TrustedPeople" x509FindType="FindByThumbprint" findValue="..." />
        <wscCertificate storeLocation="LocalMachine" storeName="My" x509FindType="FindByThumbprint" findValue="..." />
    </stsConfiguration>


Your application will need to obtain a bootstrap token (BST) either through a login flow or by exchanging an AUT for the BST (see below).

Having an instance of a BST you can request an identity token from the STS:

    // STS client
    IOioSecurityTokenServiceClient sts = new StandardSecurityTokenServiceClient(config);

    // Get identity token
    var idt = sts.GetIdentityTokenFromBootstrapToken(bst, wspEntityId, keyType, claims);

Now you can issue a request to the web service:

    // Create a WCF client
    var client = ....

    // Create channel
    var channel = client.ChannelFactory.CreateChannelWithIssuedToken(idt, new EndpointAddress(new Uri("..."),EndpointIdentity.CreateDnsIdentity("...")));

`channel` is now a channel to the web service, secured by the token.



### Request Identity Token (IDT) from an STS

You receive a BST either though a claim of a login token or by exchanging an Authentication Token (AUT) for a BST.
To be able to invoke a service, you will need to obtain an IDT by requesting the STS exchange the BST for an IDT.

You communicate with the STS through an `IOioSecurityTokenServiceClient` implementation. You create the STS by passing the configuration. The configuration contains entries for
* The STS identifier (entity ID)
* The specific URL of the service used to get a BST form a AUT. Only used in the healthcare domain.
* The specific URL of the service used to get a IDT from a bootstrap token BST. 
* The STS certificate 
* Requested lifetime of issued tokens
* General send timeout
* The WSC identifier (Entity ID of the client)
* The WSC Certificate 
* Allowable clock skew

Example code:

    // STS client
    IOioSecurityTokenServiceClient sts = new StandardSecurityTokenServiceClient(config);

    // Exchange the AUT for a BST
    var bst = sts.GetBootstrapTokenFromAuthenticationToken(aut);


### Request claims from a Security Token Service (STS) (Healthcare relevance only)

When an STS issues a token it builds claims into the token. In some circumstances you will want to request that the STS builds in certain claims, and even some claim *values* (subject to STS validation, of course).

You request claims from the STS by using an instance of the RequestClaimsCollection class. WS-Trust messages sent to the STS can pass such a collection of requested claims.
To request claims from a healthcare STS which uses a special claims "dialect" you can use this sample code:


    // Prepare request claims collection using the correct dialect.
    var claims = new RequestClaimCollection()
    {
        Dialect = "http://docs.oasis-open.org/wsfed/authorization/200706/authclaims",
    };

    // To help assign structured claims into the collection we use an *adapter*
    var adapter = new RequestClaimCollectionAttributeAdapter(claims, false);
    adapter.SetValue(CommonHealthcareAttributes.SystemVersion, "0.0");
    adapter.SetValue(CommonHealthcareAttributes.SystemName, "Bennys Astralhealing");
    adapter.SetValue(CommonHealthcareAttributes.GivenConsent, GivenConsent.Positive);

    // Exchange a bootstrap token (BST) for an identity token (IDT)
    var idt = sts.GetIdentityTokenFromBootstrapToken(bst, wspEntityId, keyType, claims);




### Creating an Authentication Token (AUT) (Healthcare relevance only)

An authentication token (AUT) is a token that is used to authenticate a user. An AUT is a token that is signed using the user's certificate (MOCES or POCES).

The package `Digst.OioIdws.Sts` contains the helper class `HealthcareAuthenticationTokenFactory`. This is a factory which will create AUT tokens.

The following example code illustrates how to create an AUT token:

    var tokenFactory = new HealthcareAuthenticationTokenFactory();
    var token = tokenFactory.CreateAuthenticationToken(certificate, AssuranceLevel.Level3);

Notice how the `CreateAuthenticationToken` method accepts the certificate and a strongly typed assurance level.

The factory's alternate constructor allow you to control the "issue instant" (timestamp tokens are issued) and the valid duration of the AUT tokens.

