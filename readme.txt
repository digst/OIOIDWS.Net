Welcome to the OIOIDWS reference implementation in .Net

Introduction:
OIOIDWS.Net is a .Net-based reference implementation of the OIOIDWS 1.0.1a profile which is described at http://digitaliser.dk/resource/526486.

The Toolkit can be used by service providers to act as a Web Service Consumer (WSC) or Web Service Producer (WSP), using the SOAP or REST standard.

This is what the root folder contains:

- Examples: Contains code that illustrates how to use OIOIDWS.Net.
	- Digst.OioIdws.Rest.Examples.AS: Example on how to run the Authentication Server in the REST variant of OIOIDWS. AS stands for Authorization Server and is the same term used in [OIO-IDWS-REST].
	- Digst.OioIdws.Rest.Examples.Client: Example on how to run the WSC/Client in the REST variant of OIOIDWS in the signature case scenario. In app.config it can be configured whether to use the Digst.OioIdws.Rest.Examples.ServerAndASCombined example or Digst.OioIdws.Rest.Examples.Server combined with Digst.OioIdws.Rest.Examples.AS example. 
	- Digst.OioIdws.Rest.Examples.ClientNuget: Same as Digst.OioIdws.Rest.Examples.Client but based on the latest public available NuGet package.
	- Digst.OioIdws.Rest.Examples.ServerAndASCombined: Example on how to run the WSP/Server in the REST variant of OIOIDWS where the AS is running in the same process	.
	- Digst.OioIdws.Rest.Examples.ServerAndASCombinedNuget: Same as Digst.OioIdws.Rest.Examples.ServerCombined but based on the latest public available NuGet package.
	- Digst.OioIdws.Rest.Examples.Server: Example on how to run the WSP/Server in the REST variant of OIOIDWS.
	- Digst.Oioidws.WscBootstrapExample: Example on how to run the WSC/Client in the SOAP variant of OIOIDWS in the bootstrap token scenario. It shows how to build a small SAML 2.0 Service Provider (SP) that also acts as a Web Service Consumer (WSC). It requires the Digst.OioIdws.WspExample to be up and running. See [OIO-BTP] for more information on configuring the SP to recieve bootstrap tokens. Note that when running the bootstrap scenario, the end-users’ identity is what the WSP sees, whereas in the signature scenario, it is the identity of the WSC that is seen by the WSP. The following things are already setup but is relevant to know when setting up your own combined SP/WSC in production:
		- The "urn:liberty:disco:2006-08:DiscoveryEPR" needs to be part of the RequestedAttributes in the SP metadata that is registered in the IdP. This informs the IdP to return a bootstrap token.
		- The SP and WSC must be registered with the same certificate in the NemLog-in administration module.
	- Digst.OioIdws.WscExample: Example on how to run the WSC/Client in the SOAP variant of OIOIDWS in the signature case scenario. It requires the Digst.OioIdws.WspExample to be up and running. 
	- Digst.OioIdws.WscExampleNuGet: Same as Digst.OioIdws.WscExample but based on the latest public available NuGet package.
	- Digst.OioIdws.WscJavaExample: Example on how to run the WSC/Client in the SOAP variant of OIOIDWS in the signature case scenario against a Java WSP/Server. Requires that a Java WSP/Server is up and running. Checkout OIOIDWS.Java on how to do that.
	- Digst.OioIdws.WspExample: Example on how to run the WSP/Server in the SOAP variant of OIOIDWS.
	- Digst.OioIdws.WspExampleNuGet: Same as Digst.OioIdws.WspExample but based on the latest public available NuGet package.

- Misc: Contains miscellaneous stuff
	- Certificates: Alle certificates needed to run the examples.
	- SOAP examples: Contains examples on requests and responses for both OioWsTrust communication between WSC and STS and LibBas communication between WSC and WSP. 
	- Specifications: All the specifications related to OIOIDWS are located here in PDF. They are only placed here to document how the specifications were at the time of development. All specifications are named [XXX] and are also referenced by that name.

- Setup: Contains powershell script to auto setup the development environment.

- Source: Contains the two main libraries Digst.OioIdws.Wsc and Digst.OioIdws.Wsp. Also contains the two helper libraries Digst.OioIdws.LibBas and Digst.OioIdws.Common. Digst.OioIdws.LibBas contains logic only related to [LIB-BAS] whereas Digst.OioIdws.Common contains the remaining logic that is in common between WSC and WSP.
	- Digst.OioIdws.Common: Contains common stuff for the SOAP variant
	- Digst.OioIdws.LibBas: Contains the implementation of the [LIB-BAS] specification.
	- Digst.OioIdws.OioWsTrust: Contains the implementation of the [OIO-WST] specification.
	- Digst.OioIdws.Rest.Client: Contains the client implementation of the [OIO-IDWS-REST] specification. It handles the communication between STS, AS and WSP
	- Digst.OioIdws.Rest.Common: Contains common stuff for the REST variant
	- Digst.OioIdws.Rest.Server: Contains the server and AS implementation of the [OIO-IDWS-REST] specification. The AS stores information from the security token and issues access tokens. The WSP contains the authentication middleware that logs the user into the WSP
	- Digst.OioIdws.Wsc: Encapsulates the usage and configuration of Digst.OioIdws.LibBas and Digst.OioIdws.OioWsTrust 
	- Digst.OioIdws.Wsp: Encapsulates the usage and configuration of Digst.OioIdws.LibBas 

- Tests: Contains various unit and integration tests.
	- Digst.OioIdws.LibBas.LongRunningTest: Contains long running tests of Digst.OioIdws.LibBas
	- Digst.OioIdws.LibBas.Test: Contains tests of Digst.OioIdws.LibBas
	- Digst.OioIdws.OioWsTrust.Test: Contains tests of Digst.OioIdws.OioWsTrust
	- Digst.OioIdws.Rest.Server.Tests: Contains tests of Digst.OioIdws.Rest.Server
	- Digst.OioIdws.Rest.SystemTests: Contains tests ofDigst.OioIdws.Rest.Client and Digst.OioIdws.Rest.Server
	- Digst.OioIdws.Test.Common

- Root: This folder
	- Digst.OioIdws.sln: The Visual Studio solution file
	- This file.

In order to run the examples or integration tests ... the following prerequisites must be in place:

- Run Setup\setup_prerequisites.ps1 in elevated mode.

- The whole solution must be build in order to run the tests as some of the tests depends on the Digst.OioIdws.WspExample example.

- The external IP address must be white listed at NETS in order to be able to make revocation check of the test OCES certificates.

The examples can be run through an elevated command prompt or through Visual Studio in elevated mode.

In order to run the examples with production certificates ... the WSC and WSP must be registered in the NemLog-in administration module and the following certificates must be in place:
- The public certificate of the STS must be acquired. This certificate must be distributed out-of-band to both WSC and WSP. WSC in order to trust responses from STS and WSP in order to trust tokens from STS.

- The WSC must acquire a FOCES certificate. This certificate does not need to be distributed out-of-band to either STS or WSP. WSP indirectly trusts the WSC through the holder-of-key mechanism and STS trusts all FOCES certificates.

- The WSP must acquire a FOCES certificate. This certificate (the public part without the private key) must be distributed out-of-band to both WSC and STS. WSC needs it in order to trust responses from the WSP and STS needs it in order to encrypt the token. The service must also be registered in STS (through "NemLog-in administration") with an endpoint ID. This ID is used in both configurations of the WSC and WSP. The WSC needs the endpoint ID in order to request a token for a specific WSP. The WSP needs the endpoint ID in order to verify that the token is issued to the right WSP.

- Information about how to order FOCES certificates from NETS DANID can be found here: http://www.nets.eu/dk-da/Produkter/Sikkerhed/Funktionssignatur/Pages/default.aspx.
 
- In order to register a WSC and WSP you must contact Digitaliseringsstyrelsen at nemlogin@digst.dk. See also "NemLog-in administration" which can be found at https://digitaliser.dk/resource/2561041, but at the moment it is not possible to create WSC's and WSP's yourself. 