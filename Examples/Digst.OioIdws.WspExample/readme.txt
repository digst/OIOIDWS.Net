Welcome to Digst.OioIdws.Wsp

Introduction:
Digst.OioIdws.Wsp is a .Net-based reference implementation of the OIOIDWS 1.0.1a profile which is described at http://digitaliser.dk/resource/526486.
This package can be used by services to act as a Web Service Producer (WSP).
The goal of this component is to make it easy for Web Service Providers (WSP) to support the OIO Identity-based Web Services (OIOIDWS) profile. 
OIOIDWS defines five scenarios but it is only "Scenario 5: Rich client and external IdP / STS" that is supported in this version.

The implementation is based on the following standards for communication with a web service consumer (WSC). 
[OIO-IDWS] - OIO Identity-Based Web Services 1.1: This document describes the overall business goals and requirements and shows how the different OIO profiles are combined to achieve these. Scenario 1 specifies that either WS-Security or a Liberty WSF-Profile can be used. Scenario 4 mandates LIB-BAS between WSC and WSP.

[LIB-BAS] - Liberty Basic SOAP Binding Profile Version 1.0: LIB-BAS is a scaled-down version of the "Liberty ID-WSF 2.0 profile" from 2006 and can be used without reading the "Liberty ID-WSF 2.0 profile". Liberty ID-WSF 2.0 profiles WS-Security, WS-Addressing and SAML. LIB-BAS specifies the use of SOAP 1.1, WS-Adressing 1.0 and WS-Security (no specific version is mentioned). Furtermore, it mandates the use of SAML 2.0 assertions.

[OIO-IDT] - OIO SAML Profile for Identity Tokens 1.0: Specifies that only "Holder of key" confirmation method should be allowed.

All above specifications can be found through https://test-nemlog-in.dk/Testportal/ or http://digitaliser.dk/resource/526486. They are also located in the "Misc\Specifications" folder on Softwarebørsen. It is the copies on Softwarebørsen that this implementation follows.

Requirements:
.Net 4.5 Framework.

How to use:
Download package through NuGet. Open configuration file and fill out all {REQUIRED} attributes. Also fill out all {OPTIONAL} attributes or remove these if not needed.

Logging:
The component itself does not make any custom logging. Use system.diagnostics to see WCF logging. The WspExample shows how this can be set up.s.

Examples:
Please checkout the complete OIOIDWS.Net reference implementation at Softwarebørsen (https://svn.softwareborsen.dk/OIOIDWS/trunk). Here is a project called Digst.OioIdws.WspExample that illustrates how a WSP can use this component.
Digst.OioIdws.WspExample illustrates how a token can be fetched and used to call a WSP.

The following is issues that Digst.OioIdws.Wsp takes care of because WCF did not support them out of the box:

- Support of encrypted assertions. The problem occurs because the key identifier clause does not reference the proof key directly. Instead the key identifier clause references the proof key indirectly through the id of the encrypted assertion (which contains the proof key). This could be solved in two ways. Either the WSC could write a key identifier clause pointing to the proof key or the WSP must support key identifier clauses pointing to an encrypted assertion. The latter has been chosen in order to be more compatible with the OIOIDWS Java reference implementation which also generates key identifier clauses pointing at the encrypted assertion.



