# Digst.OioIdws.SamlAttributes Nuget package

This package contains helpers for marshalling common OIO attributes.

The library contains a number of *marshallers* that perform encoding and decoding 
of SAML attributes.

## How to read an attribute from an assertion

    // Create an adapter (attribute manager) throug which we will access the attributes.
    // Assume that the Saml2Assertion `token` contains a single AttributeStatement
    var attributeManager = new AttributeStatementAttributeAdapter(token.Statements.OfType<Saml2ComplexAttributeStatement>().Single());

    var level = attributeManager.GetValue(CommonOioAttributes.AssuranceLevel);
    // level is now a strongly typed value 

## How to write an attribute to an assertion

    // Get an adapter for the attribute statement within the assertion
    var adapter = new AttributeStatementAttributeAdapter(assertion);

    // Example value, strongly typed
    var level = AssuranceLevel.Level3;

    // Set the value through the adapter
    adapter.SetValue(CommonOioAttributes.AssuranceLevel, level);


## Marshallers

The class `CommonOioAttributes` contains the available marshallers as public members:

* SurName
* CommonName
* Uid
* Email
* AssuranceLevel
* SpecVer
* CvrNumberIdentifier
* UniqueAccountKey
* DiscoveryEpr
* CertificateSerialNumber
* OrganizationName
* OrganizationUnit
* Title
* PostalAddress
* OcesPseudonym
* IsYouthCert
* UserCertificate
* PidNumberIdentifier
* CprNumberIdentifier
* RidNumberIdentifier
* CertificateIssuer
* ProductionUnitIdentifier
* SeNumberIdentifier
* UserAdministratorIndicator
* PrivilegesIntermediate



