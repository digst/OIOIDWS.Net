# OIO IDWS SAML Attribute utility library


## SAML Attribute marshals

An attribute marshal responsible for marshalling the attribute value to/from the security token. Marshalling means retrieving, storing, encoding and decoding SAML attributes from/to a .NET type to/from the value it should be represented as according to the relevant specification.

Each instance of an attribute marshal is responsible for a single attribute. 

The following is an examle using the `PrivilegesIntermediate` marshal:

        // Create an attribute adapter for the SAML assertion
        var attributeStatement = assertion.Statements.OfType<Saml2ComplexAttributeStatement>().Single();
        var adapter = new AttributeStatementAttributeAdapter(attributeStatement);

        // Get the PrivilegeList 
        var privileges = adapter.GetValue(CommonOioAttributes.PrivilegesIntermediate);

        // Find the privilege
        var group = privileges.PrivilegeGroups.Single(pg => pg.Scope == "urn:dk:gov:saml:cvrNumberIdentifier:12345678");
        var hasPrivilege = group.Privileges.Contains("urn:dk:some_domain:myPrivilege1A");

The expression `adapter.GetValue(CommonOioAttributes.PrivilegesIntermediate)` retrieves an attribute value. The return type is inferred from the descriptor. The descriptor handles all encoding/decoding and validation.

Note how the actual SAML name of the attribute is never referenced. It is provided by the `CommonOioAttributes.PrivilegesIntermediate`
attribute marshal. The marshal also knows the type of value and how it is encoded within the SAML assertion. The intermediate
privileges has the SAML name `TODO ` and it is represented as Base64 and UTF8 encoded XML within the SAML assertion. The `PrivilegesIntermediate` marshal hides these details and allows strongly typed access to the attribute.



## Attribute adapters

Attributes can live in SamlAssertions. In the case of a WCF service, the attributes are actually translated into *claims* which can be accessed through the ClaimsPrincipal of the OperationContext.

To allow the attribute marshals to be used against both actual SamlAssertions as well as ClaimsPrincipals, this library uses an *adapter pattern* to adapt SAML assertions as well as ClaimsPrincipals into attribute stores which can be used with the attribute marshals.