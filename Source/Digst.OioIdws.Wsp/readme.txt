Welcome to Digst.OioIdws.Wsp

Introduction:
Digst.OioIdws.Wsp is a .Net-based reference implementation of the OIOIDWS 1.1 profile which is described at http://digitaliser.dk/resource/526486.
This package can be used by services to act as a Web Service Producer (WSP).
The goal of this component is to make it easy for Web Service Providers (WSP) to support the OIO Identity-based Web Services (OIOIDWS) profile.

The implementation is based on the following standards for communication with a web service consumer (WSC). 
[OIO-IDWS] - OIO Identity-Based Web Services 1.1: This document describes the overall business goals and requirements and shows how the different OIO profiles are combined to achieve these. Scenario 1 specifies that either WS-Security or a Liberty WSF-Profile can be used. Scenario 4 mandates OIO-IDWS-SOAP between WSC and WSP.

[OIO-IDWS-SOAP] - OIO IDWS SOAP Binding Profile Version 1.1: OIO-IDWS-SOAP is a scaled-down version of the "Liberty ID-WSF 2.0 profile" from 2006 and can be used without reading the "Liberty ID-WSF 2.0 profile". Liberty ID-WSF 2.0 profiles WS-Security, WS-Addressing and SAML. OIO-IDWS-SOAP specifies the use of SOAP 1.1, WS-Adressing 1.0 and WS-Security.WS-Security 1.0 namespaces are used in the examples but the reference list points to WS-Security 1.1. This implementation uses WS-Secuity 1.0 in order to be compliant with the examples. Furtermore, it mandates the use of SAML 2.0 assertions.

[OIO-IDT] - OIO SAML Profile for Identity Tokens 1.0: Specifies that only "Holder of key" confirmation method should be allowed.

All above specifications can be found through https://test-nemlog-in.dk/Testportal/ or http://digitaliser.dk/resource/526486. They are also located in the "Misc\Specifications" folder on Softwarebørsen. It is the copies on Softwarebørsen that this implementation follows.

Requirements:
- .Net 4.7.2 Framework.

- Transport Layer Security (TLS):
  * The "OIO IDWS SOAP 1.1" specification states that in order to maintain "Message Confidentiality", "a secure transport protocol with strong encryption such as 'TLS 1.2' MUST be used.".

  * As '.NET' doesn't have support to enforce this setting for a 'WCF Services', this must bet done on an 'Operating System' level by using a tool like IIS Crypto (https://www.nartac.com/Products/IISCrypto) (freeware) where a template, for example 'PCI 3.1', can be chosen and afterwards by unmarking all protocols, except 'TLS 1.2', will ensure to enforce this requirement.

  * It's important that this is done centrally at the '.NET WSP' as we can't limit the 'WSC''s to only use 'TLS 1.2' as they communicate with the 'STS' over 'TLS 1.0'.

How to use:
Download package through NuGet. Open configuration file and fill out all {REQUIRED} attributes. Also fill out all {OPTIONAL} attributes or remove these if not needed.

In order to use OIOIDWS.Net with production certificates ... the WSC and WSP must be registered in the NemLog-in administration module and the following certificates must be in place:
- The public certificate of the STS must be acquired. This certificate must be distributed out-of-band to both WSC and WSP. WSC in order to trust responses from STS and WSP in order to trust tokens from STS.

- The WSC must acquire a FOCES certificate. This certificate does not need to be distributed out-of-band to either STS or WSP. WSP indirectly trusts the WSC through the holder-of-key mechanism and STS trusts all FOCES certificates.

- The WSP must acquire a FOCES certificate. This certificate (the public part without the private key) must be distributed out-of-band to both WSC and STS. WSC needs it in order to trust responses from the WSP and STS needs it in order to encrypt the token. The service must also be registered in STS (through "NemLog-in administration") with an endpoint ID. This ID is used in both configurations of the WSC and WSP. The WSC needs the endpoint ID in order to request a token for a specific WSP. The WSP needs the endpoint ID in order to verify that the token is issued to the right WSP.

- Information about how to order FOCES certificates from NETS DANID can be found here: http://www.nets.eu/dk-da/Produkter/Sikkerhed/Funktionssignatur/Pages/default.aspx.
 
- In order to register a WSC and WSP you must contact Digitaliseringsstyrelsen at nemlogin@digst.dk. See also "NemLog-in administration" which can be found at https://digitaliser.dk/resource/2561041, but at the moment it is not possible to create WSC's and WSP's yourself. 

- In order to use 'Digst.OioIdws.Wsp.Wsdl' to provides cross-platform capabilities for the exposed `ServiceMetadata` (WSDL file), simply add `[WsdlExportExtension]` below `[ServiceContract]` and that's it:

Logging:
The component itself does not make any custom logging. Use system.diagnostics to see WCF logging. The WspExample shows how this can be set up.

Replay attack:
As default WCF handles replay attacks. WCF's replay detection does not guarantee detecting replays in a load balanced setup and when the process is recycled. In these situations custom action must be taken. See https://msdn.microsoft.com/en-us/library/hh598927(v=vs.110).aspx

Test:
Manuel man-in-the-middle attacks has been made using Fiddler. The following tests has been executed:
	- Tampering request so that request signature does not validate.
	- Removing signature in request ensuring that WSP does not accept the request.
	- Replay attack has been tested.
	- Sending a request that has expired is not accepted by WSP.
	- Sending a request where the SAML token has expired is not accepted by WSP.

Examples:
Please checkout the complete OIOIDWS.Net reference implementation at Softwarebørsen (https://svn.softwareborsen.dk/OIOIDWS/trunk). Here is a project called Digst.OioIdws.WspExample that illustrates how a WSP can use this component.
Digst.OioIdws.WspExample illustrates how a token can be fetched and used to call a WSP.

The following are issues that Digst.OioIdws.Wsp takes care of because WCF did not support them out of the box:
WSP<->WSC communication
- Incoming request:
	- Support of encrypted assertions. According to the two statements beneath the "Web Services Security SAML Token Profile" says that the key identifier must reference the id of the SAML assertion and the SubjectConfirmation is the proof key. In this case the SAML assertion is encrypted and therefore the key identifier is referencing the id of the encrypted assertion. When WCF encounters an encrypted assertion it is automatically replaced with the decrypted assertion. The result is that the key identifier now references an id that is not present anymore. It seems strange that WIF does not take this into account when looking after the decrypted SAML assertion (when they have putted effort into decrypting the assertion), and makes me wonder if it could be solved by configuration. However, I could only make it work with the custom DecryptedSaml2SecurityToken implementation. It basically just says "yes I have an assertion with the id 'encryptedassertion' even if it is not true". The implementation will fail if NemLog-in STS changes strategy and does not use the static identifier 'encryptedassertion'.
		- When a key identifier is used to reference a SAML assertion, it MUST contain as its element value the corresponding SAML assertion identifier.
		- A SAML assertion may be referenced from a <ds:KeyInfo> element of a <ds:Signature> element in a <wsse:Security> header. In this case, the assertion contains a SubjectConfirmation element that identifies the key used in the signature calculation.
	- Adds header as required by [OIO-IDWS-SOAP]

- Outgoing response:
	- Added WS-Addressing MessageID as required by [OIO-IDWS-SOAP]. E.g. WCF does not default insert MessageID headers in responses.
	- Ensure that body is signed even if ProtectionLevel has been set to None. Body must be signed as required by [OIO-IDWS-SOAP].

- Digst.OioIdws.Wsp.Wsdl provides cross-platform capabilities for the exposed ServiceMetadata (WSDL) by the .NET WSP. Usage is "optional", but highly recommened as it will ease and minimize the amout of manual task for non-.NET WSC consuming the .NET WSP. Usage, just add [WsdlExportExtension] below [ServiceContract] and that's it.

The following is compatibillity issues solved in the Java implementation:
- Interoperability with the OIOIDWS Java implementation. .Net and Java currently makes two different digest values based on the STR-TRANSFORM. Examples has been puttet into the Misc\SOAP examples\OioIdWsSoap folder. In the examples it can been seen that:
	- .Net uses the EncryptedAssertion as root element and Java uses EncryptedData as root element.
	- .Net modifies the XML and inserts missing namespace declarations so the XML taken out of context is valid as standalone XML ... Java does not do this. Hence, .Net adds namespace xmlns:o=http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd to o:SecurityTokenReference to make the XML valid.
