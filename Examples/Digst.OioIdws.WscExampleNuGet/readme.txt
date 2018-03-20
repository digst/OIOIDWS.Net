Welcome to Digst.OioIdws.Wsc

Introduction:
Digst.OioIdws.Wsc is a .Net-based reference implementation of the OIOIDWS 1.1 profile which is described at http://digitaliser.dk/resource/526486.
This package can be used by service providers to act as a Web Service Consumer (WSC).
The goal of this component is to make it easy for Web Service Consumers (WSC) to support the OIO Identity-based Web Services (OIOIDWS) profile. 
OIOIDWS defines five scenarios but it is only "Scenario 5: Rich client and external IdP / STS" that is supported in this version.
This component does only support encrytped SAML assertions of type holder-of-key. This compoenent has not been tested with unencrypted assertions of any type. However, it it is expected to work. Encrypted SAML assertions being of type Bearer key is not supported. This is because the WSC does not know which type the encrypted SAML assertion is. NemLog-in STS is currently issueing holder-of-key tokens and therefore this component is currently configured to statically work with holder-of-key tokens when SAML assertions are encrypted. To support both types dynamically WSC could be changed to always include the WSC public certificate part in the requests to WSP. Then WSP after decrypting the SAML assertion could make a decision whether to use the WSC certificate from the token or the request based on holder-of-key or bearer key scenario.

The implementation is based on [NEMLOGIN-STSRULES] for communication with NemLog-in STS and [OIO-IDWS-SOAP] for communication with a web service provider (WSP). 
[NEMLOGIN-STS] is used for testing an implementation. The remaining proprietary standards that are directly or indirectly referenced through [NEMLOGIN-STSRULES] and [OIO-IDWS-SOAP] are also shortly described in order to get an overview of their internal dependencies.

[NEMLOGIN-STS] - NemLog-in STS 1.2: The purpose of this document is to describe how web service providers and web service consumers can test the integration to the Security Token Service from here on named STS in the Nemlog-in integration test environment.

[NEMLOGIN-STSRULES] - Security Token Service DS – Processing rules 2.0: This document mandate requirements to message interface, processing and formatting that Nemlog-in STS must comply with. The document operationalize requirements that originate from the Danish WS-Trust 1.3 [WST] interoperability profile [OIO-WST] and the associated deployment profile [OIO-WST-DEPL].

[OIO-IDWS] - OIO Identity-Based Web Services 1.1: This document describes the overall business goals and requirements and shows how the different OIO profiles are combined to achieve these. Scenario 1 specifies that either WS-Security or a Liberty WSF-Profile can be used. Scenario 4 mandates OIO-IDWS-SOAP between WSC and WSP.

[OIO-WST-DEP] - OIO WS-Trust Deployment Profile Version 1.0: Mandates the use of OIO-IDWS-SOAP without the <sbf:Framework> element. Specifies that tokens SHOULD follow the OIO-IDT profile.

[OIO-IDWS-SOAP] - OIO IDWS SOAP Binding Profile Version 1.1: OIO-IDWS-SOAP is a scaled-down version of the "Liberty ID-WSF 2.0 profile" from 2006 and can be used without reading the "Liberty ID-WSF 2.0 profile". Liberty ID-WSF 2.0 profiles WS-Security, WS-Addressing and SAML. OIO-IDWS-SOAP specifies the use of SOAP 1.1, WS-Adressing 1.0 and WS-Security. WS-Security 1.0 namespaces are used in the examples but the reference list points to WS-Security 1.1. This implementation uses WS-Secuity 1.0 in order to be compliant with the examples. Furtermore, it mandates the use of SAML 2.0 assertions.

[OIO-WST] - OIO WS-Trust Profile 1.0.1: This profile is a true subset of WS-Trust 1.3 with the addition of the element <wst14:ActAs> from WS-Trust 1.4.

[OIO-IDT] - OIO SAML Profile for Identity Tokens 1.0: Specifies that only "Holder of key" confirmation method should be allowed. The implementation has not been tested with bearer tokens.

All above specifications can be found through https://test-nemlog-in.dk/Testportal/ or http://digitaliser.dk/resource/526486. They are also located in the "Misc\Specifications" folder on Softwarebørsen. It is the copies on Softwarebørsen that this implementation follows.

Requirements:
.Net 4.5 Framework.

How to use:
Download package through NuGet. Open configuration file and fill out all {REQUIRED} attributes. Also fill out all {OPTIONAL} attributes or remove these if not needed. See configuration file Digst.OioIdws.Wsc.OioWsTrust.Configuration for details about each configuration element.

The component has two implementations of the interface Digst.OioIdws.OioWsTrust.ITokenService which can be initialized runtime or through configuration:
- TokenService: Retrieves a token from STS on each call
- TokenServiceCache: Retrieves a token from STS and caches the token for the duration of the token life time. STS is only called again if the token is not present in the cache.

Use the implementations through the Digst.OioIdws.OioWsTrust.ITokenService interface.
- SecurityToken GetToken(): Use this method in the signature case scenario
- SecurityToken GetTokenWithBootstrapToken(SecurityToken bootstrapToken): Use this method in the bootstrap token scenario.

In order to use OIOIDWS.Net with production certificates ... the WSC and WSP must be registered in the NemLog-in administration module and the following certificates must be in place:
- The public certificate of the STS must be acquired. This certificate must be distributed out-of-band to both WSC and WSP. WSC in order to trust responses from STS and WSP in order to trust tokens from STS.

- The WSC must acquire a FOCES certificate. This certificate does not need to be distributed out-of-band to either STS or WSP. WSP indirectly trusts the WSC through the holder-of-key mechanism and STS trusts all FOCES certificates.

- The WSP must acquire a FOCES certificate. This certificate (the public part without the private key) must be distributed out-of-band to both WSC and STS. WSC needs it in order to trust responses from the WSP and STS needs it in order to encrypt the token. The service must also be registered in STS (through "NemLog-in administration") with an endpoint ID. This ID is used in both configurations of the WSC and WSP. The WSC needs the endpoint ID in order to request a token for a specific WSP. The WSP needs the endpoint ID in order to verify that the token is issued to the right WSP.

- Information about how to order FOCES certificates from NETS DANID can be found here: http://www.nets.eu/dk-da/Produkter/Sikkerhed/Funktionssignatur/Pages/default.aspx.
 
- In order to register a WSC and WSP you must contact Digitaliseringsstyrelsen at nemlogin@digst.dk. See also "NemLog-in administration" which can be found at https://digitaliser.dk/resource/2561041, but at the moment it is not possible to create WSC's and WSP's yourself. 

Logging:
The component supports logging using the WSC's own logging framework. See Digst.OioIdws.Common.Logging.Configuration for details how to do this. Please notice that tokens are written out when using the Debug level. This could expose a security risk when bearer tokens with a valid life time are written to disk. Hence, do not use Debug level in production.

Replay attack:

- OioWsTrust: Does not guarantee detecting replays in a load balanced setup due to cache has been implemented in memory.

- OioIdWsSoap: As default WCF handles replay attacks. WCF's replay detection does not guarantee detecting replays in a load balanced setup and when the process is recycled. In these situations custom action must be taken. See https://msdn.microsoft.com/en-us/library/hh598927(v=vs.110).aspx

Test:
Manuel man-in-the-middle attacks has been made using Fiddler. The following tests has been executed:
	- Tampering response so that response signature does not validate.
	- Removing signature in response ensuring that WSC does not accept the response.
	- Replay attack has been tested.
	- Sending a response that has expired is not accepted by WSC.

The following is issues that Digst.OioIdws.Wsc takes care of because WCF did not support them out of the box:
WSC<->STS communication
- RST:
	- AppliesTo element is changed from namespace http://schemas.xmlsoap.org/ws/2004/09/policy to http://schemas.xmlsoap.org/ws/2002/12/policy. This is done in order to be compliant with the WS-Trust 1.3 specification.
	- Ensure that "/s:Envelope/s:Body/trust:RequestSecurityToken/wsp:AppliesTo/wsa:EndpointReference/wsa:Address" elements does not contain an ending '/'. NemLog-in STS makes string comparison instead of URI comparison.
	- Change "/s:Envelope/s:Body/trust:RequestSecurityToken/trust:Lifetime/wsu:Expires" from WCF format "yyyy-MM-ddTHH:mm:ss.fffZ" to "yyyy-MM-ddTHH:mm:ssZ" as specified in [NEMLOGIN-STSRULES].

- RSTR:
	- AppliesTo element is changed from namespace http://schemas.xmlsoap.org/ws/2002/12/policy to http://schemas.xmlsoap.org/ws/2004/09/policy. This is done in order to be compliant with the WS-Trust 1.3 specification.
	- The RequestedAttachedReference and RequestedUnattachedReference has been changed from generic references to SAML 2.0 references. This has been done in order for WCF to recognize the encrypted assertion as an SAML 2.0 token.
	- TokenType is missing if not specified in RST even if [NEMLOGIN-STSRULES] states that it will always be included.
	- Expiry time element "/s:Envelope/s:Header/wsse:Security/wsu:Timestamp/wsu:Expires" is currently not on the format specified by [NEMLOGIN-STSRULES]. [NEMLOGIN-STSRULES] says yyyy-MM-ddTHH:mm:ssZ but yyyy-MM-ddTHH:mm:ss.fffZ is currently retrieved.
	- WS-Addressing Action element contains the value http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue instead of http://docs.oasis-open.org/ws-sx/ws-trust/200512/RSTR/Issue. No code action has been taken here because WCF does not raise any error.
	
- SOAP Faults does not follow SOAP 1.1 spec.

WSC<->WSP communication
- Request:
	- Ensure that body is signed even if ProtectionLevel has been set to None. Body must be signed as required by [OIO-IDWS-SOAP].
	- Adds header as required by [OIO-IDWS-SOAP].
	- Ensures that SecurityTokenReference has a different attribute id than the KeyIdentifier element value as in the examples in [OIO-IDWS-SOAP]. As default they will have the same value. If nothing is done ... it would still work from a technical point of view.

- Response:
	- Added extra check that all [OIO-IDWS-SOAP] required WS-Adressing headers are present. E.g. WCF does not require that responses contains the MessageID header.
	- Support for encrypted responses from WSP when encrypted SAML assertions are in play. The issuse was that WSP uses the decrypted SAML assertion as key in the response, when telling the WSC which SAML assertion must be used for decrypting the response. Solved by having WSC look for a SAML assertion with id 'encryptedassertion' if the key identifier from WSP is unknown. Support for encrypted responses was necessary beceause SOAP faults is encrypted by WCF.

The following is compatibillity issues solved in the Java implementation:
- .Net and Java currently makes two different digest values based on the STR-TRANSFORM. Examples has been puttet into the Misc\SOAP examples\OioIdWsSoap folder. In the examples it can been seen that:
	- .Net uses the EncryptedAssertion as root element and Java uses EncryptedData as root element.
	- .Net modifies the XML and inserts missing namespace declarations so the XML taken out of context is valid as standalone XML ... Java does not do this. Hence, .Net adds namespace xmlns:o=http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd to o:SecurityTokenReference to make the XML valid.

The following is issues not yet solved/supported with this component:
- Replay attack from STS in a load balanced setup
- Revocation check of STS certificate.

