Welcome to Digst.OioIdws.Wsc

Introduction:
Digst.OioIdws.Wsc is a .Net-based reference implementation of the OIOIDWS 1.0.1a profile which is described at http://digitaliser.dk/resource/526486.
This package can be used by service providers to act as a Web Service Consumer (WSC).
The goal of this component is to make it easy for Web Service Consumers (WSC) to support the OIO Identity-based Web Services (OIOIDWS) profile. 
OIOIDWS defines five scenarios but it is only "Scenario 5: Rich client and external IdP / STS" that is supported in this version.

The implementation is based on [NEMLOGIN-STSRULES] for communication with NemLog-in STS and [LIB-BAS] for communication with a web service provider (WSP). 
[NEMLOGIN-STS] is used for testing an implementation. The remaining proprietary standards that are directly or indirectly referenced through [NEMLOGIN-STSRULES] and [LIB-BAS] are also shortly described in order to get an overview of their internal dependencies.

[NEMLOGIN-STS] - NemLog-in STS 1.2: The purpose of this document is to describe how web service providers and web service consumers can test the integration to the Security Token Service from here on named STS in the Nemlog-in integration test environment.

[NEMLOGIN-STSRULES] - Security Token Service DS – Processing rules 2.0: This document mandate requirements to message interface, processing and formatting that Nemlog-in STS must comply with. The document operationalize requirements that originate from the Danish WS-Trust 1.3 [WST] interoperability profile [OIO-WST] and the associated deployment profile [OIO-WST-DEPL].

[OIO-IDWS] - OIO Identity-Based Web Services 1.1: This document describes the overall business goals and requirements and shows how the different OIO profiles are combined to achieve these. Scenario 1 specifies that either WS-Security or a Liberty WSF-Profile can be used. Scenario 4 mandates LIB-BAS between WSC and WSP.

[OIO-WST-DEP] - OIO WS-Trust Deployment Profile Version 1.0: Mandates the use of LIB-BAS without the <sbf:Framework> element. Specifies that tokens SHOULD follow the OIO-IDT profile.

[LIB-BAS] - Liberty Basic SOAP Binding Profile Version 1.0: LIB-BAS is a scaled-down version of the "Liberty ID-WSF 2.0 profile" from 2006 and can be used without reading the "Liberty ID-WSF 2.0 profile". Liberty ID-WSF 2.0 profiles WS-Security, WS-Addressing and SAML. LIB-BAS specifies the use of SOAP 1.1, WS-Adressing 1.0 and WS-Security. WS-Security 1.0 namespaces are used in the examples but the reference list points to WS-Security 1.1. This implementation uses WS-Secuity 1.0 in order to be compliant with the examples. Furtermore, it mandates the use of SAML 2.0 assertions.

[OIO-WST] - OIO WS-Trust Profile 1.0.1: This profile is a true subset of WS-Trust 1.3 with the addition of the element <wst14:ActAs> from WS-Trust 1.4.

[OIO-IDT] - OIO SAML Profile for Identity Tokens 1.0: Specifies that only "Holder of key" confirmation method should be allowed. The implementation has not been tested with bearer tokens.

All above specifications can be found through https://test-nemlog-in.dk/Testportal/ or http://digitaliser.dk/resource/526486. They are also located in the "Misc\Specifications" folder on Softwarebørsen. It is the copies on Softwarebørsen that this implementation follows.

Requirements:
.Net 4.5 Framework.

How to use:
Download package through NuGet. Open configuration file and fill out all {REQUIRED} attributes. Also fill out all {OPTIONAL} attributes or remove these if not needed. See configuration file Digst.OioIdws.Wsc.OioWsTrust.Configuration for details about each configuration element.

The component exposes the following two method through the interface Digst.OioIdws.Wsc.ITokenService:

- SecurityToken GetToken();
- SecurityToken GetToken(Configuration config);

They can be used to obtain a token from STS and afterwards used for calling a WSP.

The first method uses the configuration made in the config file. This method is appropriate when there only exist one client certificate and one WSP.

The second method can be used when runtime configuration is needed. This can be useful in situations where multiple client certificates are in play (e.g. when using MOCES certificates) or WSC is using multiple WSP.

After having fetched a token from STS, the token can be used for calling a WSP. The component does not cache the tokens. Hence, the WSC must itself implement caching support if needed. 

Logging:
The component supports logging using the WSC's own logging framework. See Digst.OioIdws.Common.Logging.Configuration for details how to do this. Please notice that tokens are written out when using the Debug level. This could expose a security risk when bearer tokens with a valid life time are written to disk. Hence, do not use Debug level in production.

Replay attack:

- OioWsTrust: Does not guarantee detecting replays in a load balanced setup due to cache has been implemented in memory.

- LibBas: As default WCF handles replay attacks. WCF's replay detection does not guarantee detecting replays in a load balanced setup and when the process is recycled. In these situations custom action must be taken.

Test:
Manuel man-in-the-middle attacks has been made using Fiddler. The following tests has been executed:
	- Tampering response so that response signature does not validate.
	- Removing signature in response ensuring that WSC does not accept the response.
	- Replay attack has been tested.
	- Sending a response that has expired is not accepted by WSC.

Examples:
Please checkout the complete OIOIDWS.Net reference implementation at Softwarebørsen (https://svn.softwareborsen.dk/OIOIDWS/trunk). Here is a project called Digst.OioIdws.WscExample that illustrates how a WSC can use this component.
Digst.OioIdws.WscExample illustrates how a token can be fetched and used to call a WSP.

Digst.OioIdws.Wsc has been customized in some areas in order to be able to communicate with NemLog-in STS. The following is a list of customizations that probably can be removed if NemLog-in STS is updated:
RST:
- WS-SecurityPolicy namespace is used instead of WS-Security. Conflict with WS-Trust 1.3 spec.

RSTR:
- TokenType is missing even if [NEMLOGIN-STSRULES] states that it will always be included.

Other:
- SOAP Faults does not follow SOAP 1.1 spec.



