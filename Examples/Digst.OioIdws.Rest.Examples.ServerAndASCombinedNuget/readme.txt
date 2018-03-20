Introduction:
Digst.OioIdws.Rest.Server is a .Net-based reference implementation made as OWIN middleware of the OIOIDWS 1.1 profile which is described at http://digitaliser.dk/resource/526486.
This package can be used by services to act as a Web Service Producer (WSP) and/or Authorization Server (AS).
The goal of this component is to make it easy for Web Service Providers (WSP) to support the OIO Identity-based Web Services (OIOIDWS REST) profile.

The implementation is based on the following standards for communication with a web service consumer (WSC).
[OIO-IDWS] - OIO Identity-Based Web Services 1.1: This document describes the overall business goals and requirements and shows how the different OIO profiles are combined to achieve these. Scenario 1 specifies that either WS-Security or a Liberty WSF-Profile can be used. Scenario 4 mandates OIO-IDWS-SOAP between WSC and WSP.

[OIO-IDT] - OIO SAML Profile for Identity Tokens 1.0: Specifies that only "Holder of key" confirmation method should be allowed.

[OIO-IDWS-REST] - Adds a profile for allowing RESTful services in the OIO IDWS specification, although [OIO-IDWS] has not been updated to reflect the added profile.

All above specifications can be found through https://test-nemlog-in.dk/Testportal/ or http://digitaliser.dk/resource/526486. They are also located in the "Misc\Specifications" folder on Softwarebørsen. It is the copies on Softwarebørsen that this implementation follows.

Requirements:
.Net 4.5 Framework.

Hot to use:
You can use this package to configure either the AS, WSP or both by configuring it into a OWIN pipeline.
Dependendent on your requirements the AS and WSP can either run in the same OWIN pipeline sharing internal data structures or in seperate distributed applications using RESTful communication between them.
The example below shows the simplest possible configuration using in-memory data structures.
Note: OWIN middlewares typically make no assumptions on how you store your configuration. It's advised that you don't hardcode configuration settings, but store them in .NET configuration, database, or whatever suits you.

    public void Configuration(IAppBuilder app)
        {
            app.SetLoggerFactory(new YourLoggerFactory()); //Sets the OWIN logger factory which the middlewares logs to

            .UseOioIdwsAuthentication(new OioIdwsAuthenticationOptions()) //registers the middleware for the WSP that handles authentication, see options for possible configurations
            .UseOioIdwsAuthorizationService(new OioIdwsAuthorizationServiceOptions //registers the middleware for the AS
            {
                AccessTokenIssuerPath = new PathString("/accesstoken/issue"), //path where access tokens are issued
                //possible
                IssuerAudiences = () => Task.FromResult(new[]
                {
                    new IssuerAudiences("STS cert thumbprint", "certificate friendly name")
                        .Audience(new Uri("https://myaudience.dk")),
                }),
            })
            .Use<MyWspService>(); //Your WSP, if it's an OWIN middleware, or whatever else you might implement it as, e.g. web api
        }

The authentication middleware sits in front of the WSP service, which means if authentication fails, either by not providing an access token or if the given access token is invalid/expired/etc, 
he middleware will not halt the execution down the pipeline, but instead the request will not have the User context set, i.e. not authenticated. 
If the service requries authentication, all it has to do is set the response statuscode to 401. The authentication middleware will then generate a proper challenge according to the specification that the client can understand.
The challenge can have three different error types according to the RFC 6750 spec:
invalid_request
invalid_token
insufficient_scope

The first two is handled automatically if the response status code is set to 401.
insufficient_scope is supported by the middleware, but the WSP must communicate this through the pipeline. It can be done such as:

var authProperties = new AuthenticationProperties();
authProperties.Dictionary[AuthenticationErrorCodes.InsufficentScope] = "my_scope"; //the scope you'd require
context.Authentication.Challenge(authProperties);

where context is the IOwinContext for the current request. If running under asp.net it can retrieved by HttpContext.GetOwinContext() extension method or similar.

In order to use OIOIDWS.Net with production certificates ... the WSC and WSP must be registered in the NemLog-in administration module and the following certificates must be in place:
- The public certificate of the STS must be acquired. This certificate must be distributed out-of-band to both WSC and WSP. WSC in order to trust responses from STS and WSP in order to trust tokens from STS.

- The WSC must acquire a FOCES certificate. This certificate does not need to be distributed out-of-band to either STS or WSP. WSP indirectly trusts the WSC through the holder-of-key mechanism and STS trusts all FOCES certificates.

- The WSP must acquire a FOCES certificate. This certificate (the public part without the private key) must be distributed out-of-band to both WSC and STS. WSC needs it in order to trust responses from the WSP and STS needs it in order to encrypt the token. The service must also be registered in STS (through "NemLog-in administration") with an endpoint ID. This ID is used in both configurations of the WSC and WSP. The WSC needs the endpoint ID in order to request a token for a specific WSP. The WSP needs the endpoint ID in order to verify that the token is issued to the right WSP.

- Information about how to order FOCES certificates from NETS DANID can be found here: http://www.nets.eu/dk-da/Produkter/Sikkerhed/Funktionssignatur/Pages/default.aspx.
 
- In order to register a WSC and WSP you must contact Digitaliseringsstyrelsen at nemlogin@digst.dk. See also "NemLog-in administration" which can be found at https://digitaliser.dk/resource/2561041, but at the moment it is not possible to create WSC's and WSP's yourself. 

Logging:
Logging uses OWIN standard logging interface. For configuring the OWIN logger factory see the example in Digst.OioIdws.Rest.Examples.ServerCombined
The middlewares expect the consumer to correlate logs across requests. One example of how to do this can found in Digst.OioIdws.Rest.Examples.ServerCombined as well.

Replay attack is only handled in the AS implementation in a single server setup during issuing an access token. Replay attack is not handled in the WSP.

Examples:
Please checkout the complete OIOIDWS.Net reference implementation at Softwarebørsen (https://svn.softwareborsen.dk/OIOIDWS/trunk). Here is a couple of example projects that shows how you can run the AS and WSP in the OWIN pipeline:
Digst.OioIdws.Rest.Examples.AS - How to configure an application to run the AS and host endpoints for issuing access tokens and delivering token information to the WSP.
Digst.OioIdws.Rest.Examples.Server - How to configure an application to run the WSP with an OWIN authentication middelware that sits in front and authenticates the user using the AS.
Digst.OioIdws.Rest.Examples.ServerAndASCombined - How to configure a single application to host both the AS and WSP where communication between them can be done in memory instead of through web boundaries.