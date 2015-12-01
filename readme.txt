Welcome to OIOIDWS reference implementation in .Net

Introduction:
OIOIDWS.Net is a .Net-based reference implementation of the OIOIDWS 1.0.1a profile which is described at http://digitaliser.dk/resource/526486.
The Toolkit can be used by service providers to act as a Web Service Consumer (WSC) or Web Service Producer (WSP).

The solution is divided into the following folders:

- Examples: Contains code that illustrates how to use the Digst.OioIdws.Wsc and Digst.OioIdws.Wsp libraries.

- Misc: Contains examples on requests and responses for both OioWsTrust communication between WSC and STS and LibBas communication between WSC and WSP. Furthermore, all the specifications related to OIOIDWS are also located here in PDF.

- Source: Contains the two main libraries Digst.OioIdws.Wsc and Digst.OioIdws.Wsp. Also contains the two helper libraries Digst.OioIdws.LibBas and Digst.OioIdws.Common. Digst.OioIdws.LibBas contains logic only related to [LIB-BAS] whereas Digst.OioIdws.Common contains the remaining logic that is in common between WSC and WSP.

- Tests: Contains various unit and integration tests.

In order to run the examples or integration tests ... the following prerequisites must be in place:

- The whole solution must be build in order to run the tests.

- The certificates placed in Misc\Certificates must be installed in certificate store under "Local Computer/Personal". Password are Test1324.

- Host file must be updated with "127.0.0.1 Digst.OioIdws.Wsp" in order for examples and integration tests to work.

- The external IP address must be white listed at NETS in order to be able to make revocation check of the test OCES certificates.

In order to run the examples with production certificates ... the following prerequisites must be in place:
- The public certificate of the STS must be acquired. This certificate must be distributed out-of-band to both WSC and WSP. WSC in order to trust responses from STS and WSP in order to trust tokens from STS.

- The WSC must acquire a test FOCES certificate with a private key. This certificate does not need to be distributed out-of-band to either STS or WSP. WSP indirectly trusts the WSC through the holder-of-key mechanism and STS trusts all FOCES certificates.

- The WSP must acquire a test FOCES certificate with a private key. This certificate (the public part without the private key) must be distributed out-of-band to both WSC and STS. WSC needs it in order to trust responses from the WSP and STS needs it in order encrypt the token. The service must also be registered in STS (through "NemLog-in administration") with an endpoint ID. This ID is used in both configurations of the WSC and WSP. The WSC needs the endpoint ID in order to request a token for a specific WSP. The WSP needs the endpoint ID in order to verify that the token is issued to the right WSP.

- Information about how to order FOCES certificates from NETS DANID can be found here: http://www.nets.eu/dk-da/Produkter/Sikkerhed/Funktionssignatur/Pages/default.aspx.
 
- Information about how to register a WSP in NemLog-in STS through "NemLog-in administration" can be found here: (https://digitaliser.dk/resource/2561041). 






