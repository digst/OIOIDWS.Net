README
======


## Getting started with OIOIDWS.Net

`OIOIDWS.Net` is a `.Net`-based reference implementation of the `OIOIDWS 1.0.1a` profile.

The `OIOIDWS.Net` components can be used by service providers to act as a Web 
Service Consumer (`WSC`) or Web Service Producer (`WSP`), using the `SOAP` or 
`REST` standard.

This is the codebase that the `OIOIDWS.Net` components are built from.

### Resource links

* [Project maintenance][project]
* [Nuget packages (prefixed Digst.OioIdws)][nuget]
* [Code repository][svnrepo]

[project]: https://digitaliser.dk/group/705156
[nuget]:   https://www.nuget.org/profiles/Digitaliseringsstyrelsen
[svnrepo]: https://svn.softwareborsen.dk/OIOIDWS/

### Repository content

* **Build**: Contains script to create and publish `NuGet` packages:
* **Examples**: Contains examples that illustrates how to use `OIOIDWS.Net`.
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
  * **Digst.OioIdws.WscLocalTokenExample**: Like **Digst.OioIdws.WscExample** but uses a locally generated token. Thus, this example demonstrates the "Local Token case", where a local security token service issues a token, and NemLog-in STS is used to exchange this token for a valid WSP token. Using local tokens can remove the need to obtain and administer employee certificates for each employee. Instead, NemLog-in can be set up to trust tokens from a local STS. The example does not include a running local security token service (STS). Instead it creates tokens using a faked in-memory service. You can replace calls to this in-memory service to invoke e.g. a local STS such as (for example) a Microsoft Active Directory Federation Server (ADFS).
     The example uses a local STS that is configured with the policy set to "Local STS" in NemLog-in. Also, the WSP is configured to accept the NameID format X509SubjectName.
  * **Digst.OioIdws.WscExampleNuGet**: Same as `Digst.OioIdws.WscExample` but based on the latest public available `NuGet` package.
  * **Digst.OioIdws.WspExample**: Example on how to run the `WSP/Server` in the `SOAP` variant of `OIOIDWS`.
  * **Digst.OioIdws.WspExampleNuGet**: Same as `Digst.OioIdws.WspExample` but based on the latest public available `NuGet` package.
  * **Digst.OioIdws.Java**: Contains examples that uses `Java (WSC/WSP) <-> .NET (WSC/WSP)`
    * **Digst.OioIdws.DotnetWscJavaWspExample**: Example on how to run the `WSC/Client` in the `SOAP` variant of `OIOIDWS` in the signature case scenario against a `Java WSP/Server`. Requires that a `Java WSP/Server` is up and running. Checkout `OIOIDWS.Java` and `Guide to use Java WSP and .NET WSC` (in the `Examples\Digst.OioIdws.Java` folder) on how to do that.
    * **Digst.OioIdws.DotnetWscJavaWspExampleConfByCode**: Same as `Digst.OioIdws.DotnetWscJavaWspExample` but configured with code instead of `App.config`. It's ideal to be used for `debug` purpouses.
    * **service-hok**: `Java WSP` example project taken from the lastest `IDWS-JAVA-SOAP`. For more information, please read the `Guide to use Java WSP and .NET WSC` (in the `Examples\Digst.OioIdws.Java` folder).
    * **system-user-scenario-hok**: `Java WSC` example project taken from the lastest `IDWS-JAVA-SOAP` and adapted to work with the `Digst.OioIdws.WspExample` project. For more information, please read the `Guide to use .NET WSP (+custom WSDL) and Java WSC` (in the `Examples\Digst.OioIdws.Java` folder).
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

### Getting started

The source code contains everything you need to get a demonstration environment up and running, federating with `NemLog-in IdP` and `NemLog-in STS`.

_The full documentation of `OIOIDWS.Net` is a combination of the various readme files, `API` documentation and the examples provided._

For a quick setup, you must do the following:

* Run the script `Setup\setup_prerequisites.ps1` from an elevated `PowerShell`. This installs all required certificates and performs `sslcert` bindings to be able to host local websites using `HTTPS`.
* Open the solution `Digst.OioIdws.sln` in `Visual Studio 2019 (Elevated mode)` and build it (if you get errors on external dependencies, ensure `NuGet` packages are being restored).
* The external `IP` address must be white listed at `NETS` in order to be able to make revocation check of the test `FOCES` certificates.
* Set the projects `Digst.OioIdws.Rest.Examples.ServerAndASCombined`, `Digst.OioIdws.WspExample` and `Digst.Oioidws.WscBootstrapExample` as startup projects by right-clicking solution, select `properties`, selecting `Multiple start projects`.
* For the web project, you must manually set the `Start URL` that `IIS Express` uses. You do this by:
  * right click project `Digst.Oioidws.WscBootstrapExample`, select `properties`, select the tab `Web`, alter the `Start Action` to the radio button `Start URL`, specifying https://oiosaml-net.dk:20002.
* Run the solution which should start a `SOAP WSP`, `REST WSP` and a combined `SOAP/REST WSC`.

This should start one browser window for the `SOAP/REST WSC` `Digst.Oioidws.WscBootstrapExample`, and two console windows for `Digst.OioIdws.WspExample` and `Digst.OioIdws.Rest.Examples.ServerAndASCombined`.

In the web site you should now be able to log in using `NemLog-in`, and make `SOAP` or `REST` calls in either the bootstrap signature case scenario.

You must use an certificate employee certificate from the [NemLog-In testportal][testportal].

[testportal]: https://test-nemlog-in.dk/testportal/

### Cross-platform Examples and Testing

As we have added cross-platform support, if you would like to execute the 
example projects as well as the test projects, you will have to install the
`JAVA JDK/JRE 1.8.2`. Please follow the official guides on how to install 
these components and don't forget to create a `JAVA_HOME` under
`System Properties window` > `Environment Variables` > 
`System variables` and update the `PATH` (still in `System variables`)
with `%JAVA_HOME%\bin`.

If you have done these steps correctly, you should see the following if you 
this from a `cmd.exe`:

```
Microsoft Windows [Version 10.0.16299.309]
(c) 2017 Microsoft Corporation. All rights reserved.

C:\Users\user>echo %JAVA_HOME%
C:\Program Files\Java\jdk1.8.0_162
C:\Users\mon>"%JAVA_HOME%\bin\javac" -version
javac 1.8.0_162

C:\Users\user>
```