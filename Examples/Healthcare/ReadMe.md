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

For the example to run, it needs access to a Security Token Service (STS). This STS is available as a container on Docker Hub (https://hub.docker.com).

Prerequisites:

* Docker is installed and set to manage Linux containers.
* Docker containers have been composed from the supplied docker-compose file.
* Your .hosts is set up to resolve special host names to localhost.
* Register the web service consumer with the STS (the "backend")
* Registering the Web Service Provider (the "healthcare service" which will be called by the backend)
* Granting access for the backend (WSC) to access the healthcate service (WSP)

The following sections describes how to set up your environment to meet each of these prerequisites.


### Install Docker set to manage Linux containers.

Skip this step if you have already installed Docker.

Download and install Docker CE for Windows from https://www.docker.com/get-started

When installed right-click the docker-whale and switch to Linux containers if not already set to manage Linux containers. Docker for Windows can run both Windows and Linux containers at the same time, but it can only *manage* one of the sets at any one time.


### Create the supporting containers

Included with this project is a *docker-compose.yml* file that will fetch images and create containers for the required STS, IdP and the supporting MySQL database.

Open a command line window in administrator with /Examples/Healthcare as the current directory.

Run this command

    docker-compose up -d

This will create and start the containers.

Your screen should look something like this:

    C:\git\oioidws\Examples\Healthcare>docker-compose up -d
    Creating network "healthcare_default" with the default driver
    Creating healthcare_mysql_1      ... done
    Creating healthcare_sts_1        ... done
    Creating healthcare_simple-idp_1 ... done

    C:\git\oioidws\Examples\Healthcare>


### Registering the Web Service Consumer (the "backend")
*Note:* The certificates mentioned below should be DER encoded .cer files exported from the certificate store. 
This solution includes the necessary files in the folder _/Misc/Certificates/_.

1. Go to https://sts-idws-xua:8181/.
2. Click "Web Service Consumers".
3. Click the "+ New WSC" button in the top right corner.
4. Give the entry the name "backend". The name is not used for anything but display in this list.
5. Click "Browse" to select the certificate your WSP uses for signing. Navigate to the folder /Misc/Certiticates and pick  _WSC (Oiosaml-net.dk TEST (funktionscertifikat)).cer_ and click "Upload". A green checkmark appears on success.
6. Click "Save".

The certificate _WSC (Oiosaml-net.dk TEST (funktionscertifikat)).cer_ has a thumbprint of 0e6dbcc6efaaff72e3f3d824e536381b26deecf5.

### Registering the Web Service Provider (the "healthcare service" which will be called by the backend)
*Note:* The certificates mentioned below should be DER encoded .cer files exported from the certificate store. 
This solution includes the necessary files in the folder _/Misc/Certificates/_.

1. Go to https://sts-idws-xua:8181/.
2. Click "Web Service Providers".
3. Click the "+ New WSP" button in the top right corner.
4. Give the entry a meaningful name. The name is not used for anything but display in this list.
5. Enter "https://digst.oioidws.wsp:9090/helloworld" under EntityId. The value is case-sensitive.
6. Click "Browse" to select the certificate your WSP uses for signing. Choose _WSP (wsp.oioidws-net.dk TEST (funktionscertifikat)).cer_ and click "Upload". A green checkmark appears on success.
7. Click "Save".

The certificate _WSP (wsp.oioidws-net.dk TEST (funktionscertifikat)).cer_ has a thumbprint of 1f0830937c74b0567d6b05c07b6155059d9b10c7.

### Granting access for the backend (WSC) to access the healthcate service (WSP)
1. Go to https://sts-idws-xua:8181/.
2. Click "Web Service Consumers".
3. Click the pencil icon to the right of your recently added WSC.
4. Click the name of your recently added WSP in the "Available" list on the left. This moves the selection to the "Selected" list on the right.
5. Click the arrow button just above the WSC Name field to go back to the WSC list.
6. Restart the STS. It currently reads the database entries on startup.


## Running the example

Run the example from within Visual Studio.

Set the following projects to start from within *Solution Properties* (solution path prefixed):

* Examples/healthCare/ClientServer/Digst.OioIdws.Examples.Healthcare.ClientServer.Frontend
* Examples/healthCare/ClientServer/Digst.OioIdws.Examples.Healthcare.ClientServer.Backend
* Examples/healthCare/Digst.OioIdws.WspHealthcareExample

Start with or without debug (F5 or Ctrl-F5)

The frontend will wait for you to choose an option. When requested, the frontend will sign an AUT token, transmit it to the backend. The backend will exchange the AUT token for a BST token and exchange the BST token for an identity (IDT) token and then invoke the example healthcare service (WSP).