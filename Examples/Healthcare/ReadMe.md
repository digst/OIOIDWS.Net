Healthcare Client/Server example
================================

The projects in this folder implements an example client/server system with a client (frontend) and a server (backend) which consumes a service from a web service provider (WSP).

The example illustrates 

* How a client application running locally on an employee workstation can create an authentication token (AUT) 
  by using a certificate from current user's certificate store to issue a token.
* How the client can transmit the AUT token to the server (backend) by including it in a custom SOAP header.
* How the server (backend) can receive the AUT token through the SOAP header.
* How the server can exchange the AUT token for a bootstrap token (BST) using the Java based example Healthcare STS
* How the server can exchange the BST for an identity token (IDT) using the Java based example Healthcare STS
* How the server can invoke a web service provider (WSP) using the IDT.

## Prerequisites

The examples are pre-configured to run against publicly available test Identity provider (IdP) and Security Token Service (STS).

Prerequisites:

* Your .hosts is set up to resolve special host names to localhost.

The following sections describes how to set up your environment to meet each of these prerequisites.

You need to run the file `setup_prerequisites.ps1` from an elevated PowerShell prompt. The script is located in the `/Setup` folder.
This script will register certificates and add requires hostnames and "localhost" IP addresses to the .hosts file.

## Running the example

Run the example from within Visual Studio.

Set the following projects to start from within *Solution Properties* (solution path prefixed):

* \Examples\Healthcare\Healthcare.ClientServerExample.Frontend\Healthcare.ClientServerExample.Frontend.csproj
* \Examples\Healthcare\Healthcare.ClientServerExample.Backend
* \Examples\Healthcare\Healthcare.StandaloneWscExample\Healthcare.StandaloneWscExample.csproj
* \Examples\Healthcare\Healthcare.WspExample\Healthcare.WspExample.csproj

Start with or without debug (F5 or Ctrl-F5)

The frontend will wait for you to choose an option. When requested, the frontend will sign an AUT token, transmit it to the backend. The backend will exchange the AUT token for a BST token and exchange the BST token for an identity (IDT) token and then invoke the example healthcare service (WSP).

The StandaloneWscExample illustrates all of the steps to invoke a WSP in a single code base. It represents a desktop application which calls healthcare services directly from the application.