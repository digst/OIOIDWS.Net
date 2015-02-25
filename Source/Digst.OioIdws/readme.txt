Welcome to OIOIDWS.Net

Introduction:
OIOIDWS.Net is a .Net-based reference implementation of the OIOIDWS 1.0.1a profile which is described at http://digitaliser.dk/resource/526486.
The Toolkit can be used by service providers to act as a Web Service Consumer (WSC) on behalf of a logged in user.
The goal of this component is to make it easy for Web Service Consumers (WSC) to support the OIO Identity-based Web Services (OIOIDWS) profile. 
OIOIDWS defines five scenarios but it is only "Scenario 5: Rich client and external IdP / STS" that is supported in this version.

The implementation is based on [NEMLOGIN-STSRULES]. [NEMLOGIN-STS] is used for testing the implementation. The remaining proprietary standards that are directly or indirectly referenced through [NEMLOGIN-STSRULES] are also shortly described in order to get an overview of their internal dependencies.

[NEMLOGIN-STS] - Nemlog-in STS 1.2: The purpose of this document is to describe how service providers and web service consumers can test the integration to the Security Token Service from here on named STS in the Nemlog-in integration test environment.

[NEMLOGIN-STSRULES] - Security Token Service DS – Processing rules 2.0: This document mandate requirements to message interface, processing and formatting that Nemlog-in STS must comply with. The document operationalize requirements that originate from the Danish WS-Trust 1.3 [WST] interoperability profile [OIO-WST] and the associated deployment profile [OIO-WST-DEPL].

[OIO-IDWS] - OIO Identity-Based Web Services 1.1: This document describes the overall business goals and requirements and shows how the different OIO profiles are combined to achieve these. Scenario 1 specifies that either WS-Security or a Liberty WSF-Profile can be used. Scenario 4 mandates LIB-BAS between WSC and WSP.

[OIO-WST-DEP] - OIO WS-Trust Deployment Profile Version 1.0: Mandates the use of LIB-BAS without the <sbf:Framework> element. Specifies that tokens SHOULD follow the OIO-IDT profile.

[LIB-BAS] - Liberty Basic SOAP Binding Profile Version 1.0: LIB-BAS is a scaled-down version of the "Liberty ID-WSF 2.0 profile" from 2006 and can be used without reading the "Liberty ID-WSF 2.0 profile". Liberty ID-WSF 2.0 profiles WS-Security, WS-Addressing and SAML. LIB-BAS specifies the use of SOAP 1.1, WS-Adressing 1.0 and WS-Security (no specific version is mentioned). Furtermore, it mandates the use of SAML 2.0 assertions.

[OIO-WST] - OIO WS-Trust Profile 1.0.1: This profile is a true subset of WS-Trust 1.3 with the addition of the element <wst14:ActAs> from WS-Trust 1.4.

[OIO-IDT] - OIO SAML Profile for Identity Tokens 1.0: Specifies that only "Holder of key" confirmation method is allowed.

All above specifications can be found through https://test-nemlog-in.dk/Testportal/ or http://digitaliser.dk/resource/526486. They are also located in the "Misc\Specifications" folder on Softwarebørsen. It is the copies on Softwarebørsen that this implementation follows.

Requirements:
.Net 4.5 Framework.

How to use:
Download package through NuGet. Open configuration file and fill out all {REQUIRED} attributes. Also fill out all {OPTIONAL} attributes or remove these if not needed. See configuration file Digst.OioIdws.Configurations.OioIdwsConfiguration for details about each configuration element.

The component exposes the following two method through the interface Digst.OioIdws.ITokenService:

- SecurityToken GetToken();
- SecurityToken GetToken(OioIdwsConfiguration config);

They can be used to obtain a token from STS and afterwards used for calling a WSP.

The first method uses the configuration made in the config file. This method is appropriate when there only exist one client certificate and one WSP.

The second method can be used when runtime configuration is needed. This can be useful in situations where multiple client certificates are in play (e.g. when using MOCES certificates) or WSC is using multiple WSP.

After having fetched a token from STS, the token can be used for calling a WSP. The component does not cache the tokens. Hence, the WSC must itself implement caching support if needed. 
TODO Explain how to do call WSP!

Logging:
The component supports using the WSC' own logging framework. See Digst.OioIdws.Configurations.OioIdwsConfiguration.Logger for details how to do this. Please notice that tokens are written out when using the Debug level. This could expose a security risk when bearer tokens with a valid life times are written to disk. Hence, do not use Debug level in production.

Examples:
Please checkout the complete OIOIDWS.Net project at Softwarebørsen (https://svn.softwareborsen.dk/OIOIDWS/trunk). Here is a project called TestWebServiceConsumer that illustrates how a WSC can use this component.




