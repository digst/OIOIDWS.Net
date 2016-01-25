Welcome to Digst.OioIdws.Rest.Client

NOTE: This is a pre-release where all required security checks and error handling is not fully completed, although it is feature-complete for testing.

Quick start:
The client internally handles retrieving security token and exchanging them on the AS for an access token, as long as you use the MessageHandler provided by the OioIdwsClient. It will also handle expiration of either token and refreshing them if needed.
OioIdwsClient also exposes methods for handling security token and access token manually, if that's your choice.

	var idwsClient = new OioIdwsClient(new OioIdwsClientSettings
	{
		//TODO: Set settings such as audience, certificate, AS information, STS information, etc.
	}); 
	
	using (var httpClient = new HttpClient(idwsClient.CreateMessageHandler()))
	{
		var response = await httpClient.GetAsync("https://wspendpoint/myendpoint");
        //TODO: handle response
	}

Introduction:
Digst.OioIdws.Rest.Client is a .Net-based reference implementation of the OIOIDWS 1.0.1a REST profile which is described at http://digitaliser.dk/resource/526486.
This package can be used by RESTful service providers to act as a Web Service Consumer (WSC).
The goal of this component is to make it easy for Web Service Consumers (WSC) to support the OIO Identity-based Web Services (OIOIDWS) profile. 
OIOIDWS defines five scenarios but it is only "Scenario 5: Rich client and external IdP / STS" that is supported in this version.
This component does only support encrypted SAML assertions of type holder-of-key. This component has not been tested with unencrypted assertions of any type. However, it it is expected to work.

The implementation is based on [NEMLOGIN-STSRULES] for communication with NemLog-in STS and [OIO-IDWS-REST] for RESTful communication with an authorization server (AS) and web service producer (WSP)
[NEMLOGIN-STS] is used for testing an implementation. The remaining proprietary standards that are directly or indirectly referenced through [NEMLOGIN-STSRULES] are also shortly described in order to get an overview of their internal dependencies.

[NEMLOGIN-STS] - NemLog-in STS 1.2: The purpose of this document is to describe how web service providers and web service consumers can test the integration to the Security Token Service from here on named STS in the Nemlog-in integration test environment.

[NEMLOGIN-STSRULES] - Security Token Service DS – Processing rules 2.0: This document mandate requirements to message interface, processing and formatting that Nemlog-in STS must comply with. The document operationalize requirements that originate from the Danish WS-Trust 1.3 [WST] interoperability profile [OIO-WST] and the associated deployment profile [OIO-WST-DEPL].

[OIO-IDWS] - OIO Identity-Based Web Services 1.1: This document describes the overall business goals and requirements and shows how the different OIO profiles are combined to achieve these. Scenario 1 specifies that either WS-Security or a Liberty WSF-Profile can be used. Scenario 4 mandates LIB-BAS between WSC and WSP.

[OIO-WST] - OIO WS-Trust Profile 1.0.1: This profile is a true subset of WS-Trust 1.3 with the addition of the element <wst14:ActAs> from WS-Trust 1.4.

[OIO-IDT] - OIO SAML Profile for Identity Tokens 1.0: Specifies that only "Holder of key" confirmation method should be allowed. The implementation has not been tested with bearer tokens.

[OIO-IDWS-REST] - Adds a profile for allowing RESTful services in the OIO IDWS specification, although [OIO-IDWS] has not been updated to reflect the added profile.

All above specifications can be found through https://test-nemlog-in.dk/Testportal/ or http://digitaliser.dk/resource/526486. They are also located in the "Misc\Specifications" folder on Softwarebørsen. It is the copies on Softwarebørsen that this implementation follows.

Requirements:
.Net 4.5 Framework.

The component exposes a client for communicating with the STS, AS and WSP: Digst.OioIdws.Rest.Client.OioIdwsClient. It exposes the following methods

- GenericXmlSecurityToken GetToken();
- Task<AccessToken> GetAccessTokenAsync(GenericXmlSecurityToken securityToken, CancellationToken cancellationToken)
- HttpMessageHandler CreateMessageHandler()

The first method is used for retrieving a security token from the STS. It uses the settings given to the client when constructing it.

The second method is for retrieving an access token from the AS, given the security token retrieved from the STS

The third method wraps all of the logic involved in retrieving security token, access token and expiration of these inside i HttpMessageHandler that can be used with a HttpClient.

Logging:
The component supports logging using the WSC's own logging framework. By default it will log to the trace source "Digst.OioIdws". A custom logger can be specified using Digst.OioIdws.Common.Logging.LoggerFactory.SetLogger. See Digst.OioIdws.Common.Logging.Configuration for details how to do this. Please notice that tokens are written out when using the Debug level. This could expose a security risk when bearer tokens with a valid life time are written to disk. Hence, do not use Debug level in production.

Replay attack:

- OioWsTrust: Does not guarantee detecting replays in a load balanced setup due to cache has been implemented in memory.

Test:
Manuel man-in-the-middle attacks has been made using Fiddler. The following tests has been executed:
	- Tampering response so that response signature does not validate.
	- Removing signature in response ensuring that WSC does not accept the response.
	- Replay attack has been tested.
	- Sending a response that has expired is not accepted by WSC.

Examples:
Please checkout the complete OIOIDWS.Net reference implementation at Softwarebørsen (https://svn.softwareborsen.dk/OIOIDWS/trunk). Here is a project called Digst.OioIdws.Rest.Examples.Client that illustrates how a WSC can use this component.
Digst.OioIdws.Rest.Examples.Client illustrates how a token can be fetched and used to call a WSP.

The following is issues that Digst.OioIdws.Wsc takes care of because WCF did not support them out of the box:
WSC<->STS communication
- RST:
	- AppliesTo element is changed from namespace http://schemas.xmlsoap.org/ws/2004/09/policy to http://schemas.xmlsoap.org/ws/2002/12/policy. This is done in order to be compliant with the WS-Trust 1.3 specification.
	- Ensure that "/s:Envelope/s:Body/trust:RequestSecurityToken/wsp:AppliesTo/wsa:EndpointReference/wsa:Address" elements does not contain an ending '/'. NemLog-in STS makes string comparison instead of URI comparison.
	- Change "/s:Envelope/s:Body/trust:RequestSecurityToken/trust:Lifetime/wsu:Expires" from WCF format "yyyy-MM-ddTHH:mm:ss.fffZ" to "yyyy-MM-ddTHH:mm:ssZ" as specified in [NEMLOGIN-STSRULES].

- RSTR:
	- AppliesTo element is changed from namespace http://schemas.xmlsoap.org/ws/2002/12/policy to http://schemas.xmlsoap.org/ws/2004/09/policy. This is done in order to be compliant with the WS-Trust 1.3 specification.
	- The RequestedAttachedReference and RequestedUnattachedReference has been changed from generic references to SAML 2.0 references. This has been done in order for WCF to recognize the encrypted assertion as an SAML 2.0 token. It also ensures that 
	- TokenType is missing if not specified in RST even if [NEMLOGIN-STSRULES] states that it will always be included.
	- Expiry time element "/s:Envelope/s:Header/wsse:Security/wsu:Timestamp/wsu:Expires" is currently not on the format specified by [NEMLOGIN-STSRULES]. [NEMLOGIN-STSRULES] says yyyy-MM-ddTHH:mm:ssZ but yyyy-MM-ddTHH:mm:ss.fffZ is currently retrieved.
	- WS-Addressing Action element contains the value http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue instead of http://docs.oasis-open.org/ws-sx/ws-trust/200512/RSTR/Issue. No code action has been taken here because WCF does not raise any error.
	
- SOAP Faults does not follow SOAP 1.1 spec.

The following is issues not yet solved/supported with this component:
- (Fixed in Java implementation) Interoperability with the OIOIDWS Java implementation. .Net and Java currently makes two different digest values based on the STR-TRANSFORM. Examples has been puttet into the Misc\SOAP examples\LibBas folder. In the examples it can been seen that:
	- .Net uses the EncryptedAssertion as root element and Java uses EncryptedData as root element.
	- .Net modifies the XML and inserts missing namespace declarations so the XML taken out of context is valid as standalone XML ... Java does not do this. Hence, .Net adds namespace xmlns:o=http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd to o:SecurityTokenReference to make the XML valid.
- Replay attack from STS in a load balanced setup
- Revocation check of STS certificate.