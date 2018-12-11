README
======

[OIO] is the national Danish e-Government interoperability framework. It is a set of standards for public-sector/e-government information exchange (Offentlig Information Overførsel).

[OIO]: http://arkitekturguiden.digitaliser.dk/

IDWS is short for **Id**entity based **W**eb **S**ervices. 


To access an identity based web service, you must to present a security token which not only proves that you have legitimate access, but also proves who (the identity) 
you are acting as / on behalf of.

`OIOIDWS.Net` is a `.Net`-based reference implementation of the `OIOIDWS 1.0.1a` profile.


# Getting started with OIOIDWS.Net

For a quick *getting started* guide see [Getting started] below

## Terminology

The `OIOIDWS.Net` components can be used by service providers to act as a Web Service Consumer (`WSC`) or Web Service 
Producer (`WSP`), using the `SOAP` or `REST` standards.

The components have been extended to support healthcare identity based web services.

This is the codebase that the `OIOIDWS.Net` components are built from.

## Resource links

* [Project maintenance][project]
* [Nuget packages (prefixed Digst.OioIdws)][nuget]
* [Code repository][svnrepo]
* [Corresponding Java based implementation][TODO]

[project]: https://digitaliser.dk/group/705156
[nuget]:   https://www.nuget.org/profiles/Digitaliseringsstyrelsen
[svnrepo]: https://svn.softwareborsen.dk/OIOIDWS/


## Repository content

* **Build**: Contains script to create and publish `NuGet` packages:
  
* **Examples**: Contains examples that illustrates how to use `OIOIDWS.Net`.

  * **OIO**: Contains examples on how to use the base OIO IDWS functionality in a number of scenarios.
    Please refer to the section [OIO Examples](#oio-examples) below for a description of the individual examples.
  
  * **Healthcare**: Contains examples on how to use the components when implementing web services that are 
    both OIO and XUA compliant. Please refer to the section [Healthcare Examples](#healthcare-examples) below for a description of the individual examples.

* **Misc**: Contains miscellaneous stuff
  
  * **Certificates**: All certificates needed to run the examples.
  
  * **SOAP examples**: Contains examples on requests and responses for both `OioWsTrust` communication between `WSC <-> STS` and between `WSC <-> WSP`.
  
  * **Specifications**: All the specifications related to `OIOIDWS` are located here in `PDF`. They are only placed here to document how the specifications were at the time of development. All specifications are named [`XXX`] and are also referenced by that name.
  
  * **Token examples**: Contains examples on `IdP` issued bootstrap token and `STS` issued access token.

* **Setup**: Contains `PowerShell` script to auto setup the development environment.

* **Source**: Source code for the `OIOIDWS.Net` framework

  * **Digst.OioIdws.Common**: Contains common stuff for the `SOAP` variant.
  
  * **Digst.OioIdws.Soap**: Contains the implementation of the [`OIO IDWS SOAP 1.1`] specification.
  
  * **Digst.OioIdws.OioWsTrust**: Contains the implementation of the [`OIO-WST`] specification.
  
  * **Digst.OioIdws.Rest.Client**: Contains the client implementation of the [`OIO-IDWS-REST`] specification. It handles the communication between `STS`, `AS` and `WSP`.
  
  * **Digst.OioIdws.Rest.Common**: Contains common stuff for the `REST` variant.
  
  * **Digst.OioIdws.Rest.Server**: Contains the server and `AS` implementation of the [`OIO-IDWS-REST`] specification. The `AS` stores information from the security token and issues access tokens. The `WSP` contains the authentication middleware that logs the user into the `WSP`.
  
  * **Digst.OioIdws.Wsc**: Encapsulates the usage and configuration of `Digst.OioIdws.Soap` and `Digst.OioIdws.OioWsTrust`.
  
  * **Digst.OioIdws.Wsp**: Encapsulates the usage and configuration of `Digst.OioIdws.Soap` 
  
  * **Digst.OioIdws.Wsp.Wsdl**: It's part of `Digst.OioIdws.Wsp` and provides cross-platform capabilities for the exposed `ServiceMetadata` (`WSDL`) by the `.NET WSP`. Usage is _optional_, but highly recommened as it will ease and minimize the amout of manual task for non-`.NET WSC` consuming the `.NET WSP`.

* **Tests**: Contains various unit and integration tests:

  * **Digst.OioIdws.Soap.LongRunningTest**: Contains long running tests of `Digst.OioIdws.Soap`.
  
  * **Digst.OioIdws.Soap.Test**: Contains tests of `Digst.OioIdws.Soap`.
  
  * **Digst.OioIdws.Soap.CrossTest**: Contains cross-tests of `Digst.OioIdws.Soap` combined with `Digst.OioIdws.Java`.
  
  * **Digst.OioIdws.OioWsTrust.Test**: Contains tests of `Digst.OioIdws.OioWsTrust`.
  
  * **Digst.OioIdws.Rest.Server.Tests**: Contains tests of `Digst.OioIdws.Rest.Server`
  
  * **Digst.OioIdws.Rest.SystemTests**: Contains tests of `Digst.OioIdws.Rest.Client` and `Digst.OioIdws.Rest.Server`.
  
  * **Digst.OioIdws.Test.Common**: Common stuff user by the other test libaries.

* **DEVELOPER-NOTES.md**: Information relevant for developers of `OIOIDWS.Net` (updates `.html` when saved in `Visual Studio`).

* **Digst.OioIdws.sln**: `Visual Studio 2017` solution file.

* **README.md**: This file (updates `.html` when saved in `Visual Studio`).


## Getting started

The source code contains everything you need to get a demonstration environment up and running, federating with `NemLog-in IdP` and `NemLog-in STS`,
and federating with example healthcare Idp and STS.

Test NemLog-in IdP and -STS services are publicly available.

There is no publicly available running IdP or STS for healthcare test services. However, docker containers implementing example healthcare IdP and healthcare STS are publicly available. You will need to be running these containers for the healthcare examples to work.

_The full documentation of `OIOIDWS.Net` is a combination of the various readme files, `API` documentation and the examples provided._

For a quick setup, you must do the following:

### 1. Install Docker (optional, relevant required for the healthcare-related examples)

* Make sure that Docker is installed. Docker is used to run a local Security Token Service (STS) and a local Identity provider (IdP) used for the healthcare related examples. The healthcare examples do not use NemLog-in Idp or nemLog-in STS. The Docker-based STS and IdP simulates

### 2. Certificates. 
* Run the script `Setup\setup_prerequisites.ps1` from an elevated `PowerShell`. This installs all required certificates and performs `sslcert` bindings to be able to host local websites using `HTTPS`.
* Open the solution `Digst.OioIdws.sln` in `Visual Studio 2017 (Elevated mode)` and build it (if you get errors on external dependencies, ensure `NuGet` packages are being restored).
* The external `IP` address must be white listed at `NETS` in order to be able to make revocation check of the test `FOCES` certificates.
* Set the projects `Digst.OioIdws.Rest.Examples.ServerAndASCombined`, `Digst.OioIdws.WspExample` and `Digst.Oioidws.WscBootstrapExample` as startup projects by right-clicking solution, select `properties`, selecting `Multiple start projects`.
* For the web project, you must manually set the `Start URL` that `IIS Express` uses. You do this by:
  * right click project `Digst.Oioidws.WscBootstrapExample`, select `properties`, select the tab `Web`, alter the `Start Action` to the radio button `Start URL`, specifying https://oiosaml-net.dk:20002.
* Run the solution which should start a `SOAP WSP`, `REST WSP` and a combined `SOAP/REST WSC`.

This should start one browser window for the `SOAP/REST WSC` `Digst.Oioidws.WscBootstrapExample`, and two console windows for `Digst.OioIdws.WspExample` and `Digst.OioIdws.Rest.Examples.ServerAndASCombined`.

In the web site you should now be able to log in using `NemLog-in`, and make `SOAP` or `REST` calls in either the bootstrap signature case scenario.

You must use an certificate employee certificate from the [NemLog-In testportal][testportal].

[testportal]: https://test-nemlog-in.dk/testportal/


## OIO Examples

  * **Digst.OioIdws.Rest.Examples.AS**: Example on how to run the Authentication Server in the `REST` variant of `OIOIDWS`. `AS` stands for Authorization Server and is the same term used in [`OIO-IDWS-REST`].
  * **Digst.OioIdws.Rest.Examples.Client**: Example on how to run the `WSC/Client` in the `REST` variant of `OIOIDWS` in the signature case scenario. In `app.config` it can be configured whether to use the `Digst.OioIdws.Rest.Examples.ServerAndASCombined` example or `Digst.OioIdws.Rest.Examples.Server` combined with `Digst.OioIdws.Rest.Examples.AS` example.
  * **Digst.OioIdws.Rest.Examples.ClientNuget**: Contains code that illustrates how to use `OIOIDWS.Net`.
  * **Digst.OioIdws.Rest.Examples.ServerAndASCombined**: Same as `Digst.OioIdws.Rest.Examples.Client` but based on the latest public available `NuGet` package.
  * **Digst.OioIdws.Rest.Examples.ServerAndASCombinedNuget**: Example on how to run the `WSP/Server` in the `REST` variant of `OIOIDWS` where the `AS` is running in the same process.
  * **Digst.OioIdws.Rest.Examples.Server**: Same as `Digst.OioIdws.Rest.Examples.ServerCombined` but based on the latest public available `NuGet` package.
  * **Digst.Oioidws.WscBootstrapExample**: Example on how to run the `WSP/Server` in the `REST` variant of `OIOIDWS`.
  * **Digst.OioIdws.WscExample**: Example on how to run the `WSC/Client` in the `SOAP` variant of `OIOIDWS` in the bootstrap token scenario. It shows how to build a small `SAML 2.0` Service Provider (`SP`) that also acts as a Web Service Consumer (`WSC`). It requires the `Digst.OioIdws.WspExample` to be up and running. See [`OIO-BTP`] for more information on configuring the `SP` to recieve bootstrap tokens. Note that when running the bootstrap scenario, the end-users' identity is what the `WSP` sees, whereas in the signature scenario, it is the identity of the `WSC` that is seen by the `WSP`. The following things are already setup but is relevant to know when setting up your own combined `SP/WSC` in production:
    * The `SP` and `WSC` must be registered with the same certificate in the `NemLog-in` administration module.
  * **Digst.OioIdws.WscExampleConfByCode**: Same as `Digst.OioIdws.WscExample` but configured with code instead of `App.config`. It's ideal to be used for `debug` purpouses.
  * **Digst.OioIdws.WscExampleNuGet**: Same as `Digst.OioIdws.WscExample` but based on the latest public available `NuGet` package.
  * **Digst.OioIdws.WspExample**: Example on how to run the `WSP/Server` in the `SOAP` variant of `OIOIDWS`.
  * **Digst.OioIdws.WspExampleNuGet**: Same as `Digst.OioIdws.WspExample` but based on the latest public available `NuGet` package.
  * **Digst.OioIdws.Java**: Contains examples that uses `Java (WSC/WSP) <-> .NET (WSC/WSP)`
    * **Digst.OioIdws.DotnetWscJavaWspExample**: Example on how to run the `WSC/Client` in the `SOAP` variant of `OIOIDWS` in the signature case scenario against a `Java WSP/Server`. Requires that a `Java WSP/Server` is up and running. Checkout `OIOIDWS.Java` and `Guide to use Java WSP and .NET WSC` (in the `Examples\Digst.OioIdws.Java` folder) on how to do that.
    * **Digst.OioIdws.DotnetWscJavaWspExampleConfByCode**: Same as `Digst.OioIdws.DotnetWscJavaWspExample` but configured with code instead of `App.config`. It's ideal to be used for `debug` purpouses.
    * **service-hok**: `Java WSP` example project taken from the lastest `IDWS-JAVA-SOAP`. For more information, please read the `Guide to use Java WSP and .NET WSC` (in the `Examples\Digst.OioIdws.Java` folder).
    * **system-user-scenario-hok**: `Java WSC` example project taken from the lastest `IDWS-JAVA-SOAP` and adapted to work with the `Digst.OioIdws.WspExample` project. For more information, please read the `Guide to use .NET WSP (+custom WSDL) and Java WSC` (in the `Examples\Digst.OioIdws.Java` folder).


## Healthcare Examples

* **ClientServer**: Demonstrates a scenario with a client/server (frontend/backend) system where employee authentication must occur on the client.
 
  The _client_ (frontend) part of a client/server system is running locally on an employee workstation, the _server_ (backend) part is running on a server which in turn interacts with the _healthcare STS_ and using the services of the example _WSP_.
  
  In this scenario, the frontend application uses a MOCES (employee) certificate available locally on 
  the employee workstation (potentially as part of a roaming profile) to issue an authentication token (AUT).
    
  This AUT is transferred to the application server (backend) as part of a request. The application server 
  in turn uses this AUT to retrieve a bootstrap token (BST) from the healthcare STS. The BST is specific to the employee session.

  The application server (backend) then uses the BST to retrieve an _identity token_ (IDT) from the same STS.
  The IDT is specific to the user and the intended WSP. The backend invokes the WSP using the IDT. The WSP response
  is used to generate a response the the client application.

* **Web**: Demonstrates a Web single-signon (SSO) scenario. 

  You have a _website_ which uses an identity provider (IdP) for login. When logging in, the _IdP_ will send a _bootstrap token_ (BST) within the 
  login assertion. The _website_ will use this BST to retrieve an _identity token_ (IDT) from a _security token service_ (STS). Within the
  request for a security token, the _website_ will ask for specific claims and claim values to be included in the IDT.

  Upon receiving the IDT from the STS, the website will use the IDT to invoke a web service at the _web service provider_ (WSP).

* **Local signature**: Demonstrates a standalone application which uses a employee certificate (MOCES) to create an authentication token (AUT) 
  and use this token to exchange it for a bootstrap token (BST) at the security token service (STS). Using the BST, the application
  uses the STS yet again to retrieve an identity token (IDT) for the web service provider (WSP) and finally invoke the WSP with the IDT.




## Cross-platform Examples and Testing

As we have added cross-platform support, if you would like to execute the 
example projects as well as the test projects, you will have to install the
`JAVA JDK/JRE 1.8.2`. Please follow the official guides on how to install 
these components and don't forget to create a `JAVA_HOME` under
`System Properties window` > `Environment Variables` > 
`System variables` and update the `PATH` (still in `System variables`)
with `%JAVA_HOME%\bin`.

You can verify that the Java installation is ok by typing these two lines this from a `cmd.exe`:

    echo %JAVA_HOME%

    "%JAVA_HOME%\bin\javac" -version

Your screen should then look like this:

```
Microsoft Windows [Version 10.0.16299.309]
(c) 2017 Microsoft Corporation. All rights reserved.

C:\Users\user>echo %JAVA_HOME%
C:\Program Files\Java\jdk1.8.0_162

C:\Users\user>"%JAVA_HOME%\bin\javac" -version
javac 1.8.0_162

C:\Users\user>
```
