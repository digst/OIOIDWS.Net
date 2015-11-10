Welcome to Digst.OioIdws.Wsp

Introduction:
Digst.OioIdws.Wsp is a .Net-based reference implementation of the OIOIDWS 1.0.1a profile which is described at http://digitaliser.dk/resource/526486.
This package can be used by services to act as a Web Service Producer (WSP).
The goal of this component is to make it easy for Web Service Providers (WSP) to support the OIO Identity-based Web Services (OIOIDWS) profile.

The implementation is based on the following standards for communication with a web service consumer (WSC). 
[OIO-IDWS] - OIO Identity-Based Web Services 1.1: This document describes the overall business goals and requirements and shows how the different OIO profiles are combined to achieve these. Scenario 1 specifies that either WS-Security or a Liberty WSF-Profile can be used. Scenario 4 mandates LIB-BAS between WSC and WSP.

[LIB-BAS] - Liberty Basic SOAP Binding Profile Version 1.0: LIB-BAS is a scaled-down version of the "Liberty ID-WSF 2.0 profile" from 2006 and can be used without reading the "Liberty ID-WSF 2.0 profile". Liberty ID-WSF 2.0 profiles WS-Security, WS-Addressing and SAML. LIB-BAS specifies the use of SOAP 1.1, WS-Adressing 1.0 and WS-Security.WS-Security 1.0 namespaces are used in the examples but the reference list points to WS-Security 1.1. This implementation uses WS-Secuity 1.0 in order to be compliant with the examples. Furtermore, it mandates the use of SAML 2.0 assertions.

[OIO-IDT] - OIO SAML Profile for Identity Tokens 1.0: Specifies that only "Holder of key" confirmation method should be allowed.

All above specifications can be found through https://test-nemlog-in.dk/Testportal/ or http://digitaliser.dk/resource/526486. They are also located in the "Misc\Specifications" folder on Softwarebørsen. It is the copies on Softwarebørsen that this implementation follows.

Requirements:
.Net 4.5 Framework.

How to use:
Download package through NuGet. Open configuration file and fill out all {REQUIRED} attributes. Also fill out all {OPTIONAL} attributes or remove these if not needed.

Logging:
The component itself does not make any custom logging. Use system.diagnostics to see WCF logging. The WspExample shows how this can be set up.

Replay attack:
As default WCF handles replay attacks. WCF's replay detection does not guarantee detecting replays in a load balanced setup and when the process is recycled. In these situations custom action must be taken.

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

The following is issues that Digst.OioIdws.Wsp takes care of because WCF did not support them out of the box:

- Support of encrypted assertions. According to the two statements beneath the "Web Services Security SAML Token Profile" says that the key identifier must reference the id of the SAML assertion and the SubjectConfirmation is the proof key. In this case the SAML assertion is encrypted and therefore the key identifier is referencing the id of the encrypted assertion. When WCF encounters an encrypted assertion it is automatically replaced with the decrypted assertion. The result is that the key identifier now references an id that is not present anymore. It seems strange that WIF does not take this into account when looking after the decrypted SAML assertion (when they have putted effort into decrypting the assertion), and makes me wonder if it could be solved by configuration. However, I could only make it work with the custom DecryptedSaml2SecurityToken implementation. It basically just says "yes I have an assertion with the id 'encryptedassertion' even if it is not true". The implementation will fail if NemLog-in STS changes strategy and does not use the static identifier 'encryptedassertion'.
	- When a key identifier is used to reference a SAML assertion, it MUST contain as its element value the corresponding SAML assertion identifier.
	- A SAML assertion may be referenced from a <ds:KeyInfo> element of a <ds:Signature> element in a <wsse:Security> header. In this case, the assertion contains a SubjectConfirmation element that identifies the key used in the signature calculation.

The following is issues not yet solved with this component:
- Interoperability with the OIOIDWS Java implementation. .Net and Java currently makes two different digest values based on the STR-TRANSFORM. Examples has been puttet into the Misc\SOAP examples\LibBas folder. In the examples it can been seen that:
	- .Net uses the EncryptedAssertion as root element and Java uses EncryptedData as root element.
	- .Net modifies the XML and inserts missing namespace declarations so the XML taken out of context is valid as standalone XML ... Java does not do this. Hence, .Net adds namespace xmlns:o=http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd to o:SecurityTokenReference to make the XML valid.
- SOAP faults are encrypted. It would be desirable that even if the WSC had misconfigured its certificates, that it would still be able read the SOAP faults.

	



