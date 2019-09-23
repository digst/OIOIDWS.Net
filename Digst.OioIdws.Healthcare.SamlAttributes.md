# Digst.OioIdws.Healthcare.SamlAttributes Nuget package

Marshalling common HL-7 SAML attributes through SAML assertions. Allows complex attributes, 
like attributes based on XML or Base64 encoded.

The library contains a number of *marshallers* that perform encoding and decoding 
of SAML attributes.

## How to read an attribute from an assertion

    // Create an adapter (attribute manager) throug which we will access the attributes.
    // Assume that the Saml2Assertion `token` contains a single AttributeStatement
    var attributeManager = new AttributeStatementAttributeAdapter(token.Statements.OfType<Saml2ComplexAttributeStatement>().Single());

    var userType = attributeManager.GetValue(CommonHealthcareAttributes.UserType);
    // userType is now a strongly typed value (an enum)

## How to write an attribute to an assertion

    // Get an adapter for the attribute statement within the assertion
    var adapter = new AttributeStatementAttributeAdapter(assertion);

    // Example value, strongly typed
    var subjectRoles = new[]{
        new SubjectRole("7170", "1.2.208.176.1.3", "Autorisationsregister", "Læge"),
        new SubjectRole("5433","1.2.208.176.1.3","Autorisationsregister","Tandlæge"),
    };

    // Set the value through the adapter
    adapter.SetValue(CommonHealthcareAttributes.SubjectRole, subjectRoles);


## Marshallers

The class `CommonHealthcareAttributes` contains the available marshallers as public members.

