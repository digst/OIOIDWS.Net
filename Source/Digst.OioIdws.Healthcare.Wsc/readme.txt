

Use this library to implement a web service consumer (WSC) which will take part in the danish healthcare federation. 

The main class of this library is the StandardSecurityTokenServiceClient class. This class is effectively a client wrapper
around the security token service (STS). You will typically use this class to obtain a token which allows you to create
a channel to the actual web service hosted at the web service provider (WSP). 

The STS can 

* Obtain a bootstrap token (BST) from an authentication token (AUT)
* Obtain an identity token (IDT) from a bootstrap token (BST)

Authentication tokens (AUTs) are typically created by a client application. 

Bootstrap tokens (BSTs) are typically either received from an identity provider (IdP) as part of a web single-signon scenario (web-SSO)
or from the security token service (STS) from an authentication token (AUT).

The client wrapper reads configuration from the configuration file.

This example configuration connects to the federation test STS:

	<stsConfiguration
		stsIdentifier="http://sts.sundhedsdatastyrelsen.dk/"
		bootstrapTokenFromAuthenticationTokenUrl="https://test1-cnsp.ekstern-test.nspop.dk:8443/sts3/services/employee/bootstrap"
		identityTokenFromBootstrapTokenUrl="https://test1-cnsp.ekstern-test.nspop.dk:8443/sts3/services/employee"
		serviceTokenUrl=""
		wscIdentifier="https://healthcareclient" >
		<stsCertificate storeLocation="LocalMachine" storeName="TrustedPeople" x509FindType="FindByThumbprint" findValue="917b982f9cbd0a0d5cba60d99488f1e02117414e" />
		<wscCertificate storeLocation="LocalMachine" storeName="My" x509FindType="FindByThumbprint" findValue="0e6dbcc6efaaff72e3f3d824e536381b26deecf5" />
	</stsConfiguration>

* stsIdentifier: The entity ID of the STS
* bootstrapTokenFromAuthenticationTokenUrl: Endpoint for obtaining BST from an AUT
* identityTokenFromBootstrapTokenUrl: Endpoint for obtaining an IDT from a BST
* serviceTokenUrl: Endpoint for obtaining a non-identity service token
* wscIdentifier: Uniquely identifies this WSC within the federation
* stsCertificate: Identifies where to find the STS certificate. You typically obtain this certificate from the STS operator
* wscCertificate: The certificate (typically a FOCES or VOCES certificate) of this WSC. The account running the WSC must have access to the private key of this certificate.


## Example code


        //Retrieve the MOCES certificate for signing the locally issued AUT token
        var mocesCertThumbprint = ConfigurationManager.AppSettings["SubjectCertificateThumbprint"];
        var mocesCert = CertificateUtil.GetCertificate(StoreName.My, StoreLocation.CurrentUser, X509FindType.FindByThumbprint, mocesCertThumbprint);
		var config = SecurityTokenServiceClientConfigurationSection.FromConfiguration();
            
        // The STS
        var sts = new StandardSecurityTokenServiceClient(config);

	    //Build AUT token to send to STS
        var factory = new HealthcareAuthenticationTokenFactory(TimeSpan.FromMinutes(5));
        var aut = factory.CreateAuthenticationToken(mocesCert, AssuranceLevel.Level3).ToGenericXmlSecurityToken();

        // Exchange the AUT for a BST
        var bst = sts.GetBootstrapTokenFromAuthenticationToken(aut);

		// You may store the BST in users session, secure cookie or similar. 

		// Set claims
		var claims = new RequestClaimCollection()
        {
            Dialect = "http://docs.oasis-open.org/wsfed/authorization/200706/authclaims",
        };
            
        claims.Add(new RequestClaim(CommonHealthcareAttributes.SystemVersion.Name, true, "0.0"));
        claims.Add(new RequestClaim(CommonHealthcareAttributes.SystemName.Name, true, "Bennys Astralhealing"));
        claims.Add(new RequestClaim(CommonHealthcareAttributes.SystemUsingOrganisationName.Name, true, "Bennys Astralhealing"));
        claims.Add(new RequestClaim(CommonHealthcareAttributes.PatientResourceId.Name, true, "2512484916^^^&1.2.208.176.1.2&ISO"));
        claims.Add(new RequestClaim(CommonHealthcareAttributes.SubjectProviderIdentifier.Name, true, @"<id xmlns=""urn:hl7-org:v3"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:type=""II"" root=""1.2.208.176.1.1"" extension=""8041000016000^Sydvestjysk Sygehus"" assigningAuthorityName=""Sundhedsvæsenets Organisationsregister (SOR)""/>"));

        // Get identity token
        var idt = sts.GetIdentityTokenFromBootstrapToken(bst, wspEntityId, claims, KeyType.HolderOfKey);

		// the IDT is specific for this subject at this service, but may be used for multiple invocations on behalf of the subject

        // WSP proxy
        var client = new HelloWorldClient();

        //Create a channel with the issued token
        var channel = client.ChannelFactory.CreateChannelWithIssuedToken(idt, new EndpointAddress(new Uri(wspUrl), EndpointIdentity.CreateDnsIdentity("wsp.oioidws-net.dk TEST (funktionscertifikat)")));

        //Call the service and log the response to console
        WriteLine(channel.HelloSign("Arthur"));

