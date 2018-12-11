# SAML-H Utility Libraries


The SAML Attribute Utility libraries contains utility classes to manage common attributes within tokens and claims requests.

Using the utility classes makes it easier and more robust to manage the SAML attributes of tokens.


## Marshals

The libraries uses the concept of *marshallers*. A marshaller controls how values are stored in and retrieved from tokens, including
encoding, e.g. base64 encoding of XML.

A marshal is used for strongly typed attribute/claim values to be stored/retrieved to/from various targets (tokens, claims principals). 
The marshal handles the encoding/decoding for non-string attribute types.

Each attribute (claim type) has its own marshal.

The class _Digst.OioIdws.SamlAttributes.Oio.CommonOioAttributes_ contains marshals for common Oio attributes, such as 
intermediate privileges (model 2), surname etc.

The class _Digst.OioIdws.Healthcare.SamlAttributes.CommonHealthcareAttributes_ contains marshals for common healthcare attributes,
such as given consent, purpose of use etc.

The marshals are extensible. New marshals can be defined based on the following "base" marshals:

    * BooleanSamlAttributeMarshal
    * DataContractBase64SamlAttributeMarshal
    * EncodedStringSamlAttributeMarshal
    * EnumSamlAttributeMarshal
    * IntSamlAttributeMarshal
    * MappingSamlAttributeMarshal
    * MultiStringSamlAttributeMarshal
    * MultiXmlSerializableAttributeMarshal
    * SamlAttributeMarshal
    * StringBase64SamlAttributeMarshal
    * StringSamlAttributeMarshal
    * XElementBase64SamlAttributeMarshal
    * XElementSamlAttributeMarshal
    * XmlDocumentBase64SamlAttributeMarshal
    * XmlReaderWriterAttributeMarshal
    * XmlSerializableBase64SamlAttributeMarshal
    * XmlSerializableSamlAttributeMarshal


## Setting attributes of a token

If you are creating a token yourself, you can use the utility adapters:

    // Build token 
    var token = OioSecurityTokenFactory.CreateAuthenticationToken(mocesCert, AssuranceLevel.Level3, TimeSpan.FromMinutes(5));
    
    // Set additional attributes of the token
    var mgr = new AttributeStatementAttributeAdapter(token.Assertion);
    mgr.SetValue(CommonHealthcareAttributes.GivenConsent, GivenConsent.None);
    mgr.SetValue(CommonHealthcareAttributes.IsOver18, true);
    mgr.SetValue(CommonHealthcareAttributes.PurposeOfUse, PurposeOfUse.Treatment);

In the above example `mgr` if the attribute adapter/manager. Set `SetValue` method takes two parameters: The marshal and the value to be set.


## Retrieving attributes from a claims principal

Within a service the attributes of the token used to invoke the service will have been mapped as claims of the ClaimsPrincipal of the service operation context.

You can wrap this ClaimsPrincipal in a _ClaimsPrincipalAttributeAdapter_:

    // Wrap the principal in an attribute adapter
    var mgr = new ClaimsPrincipalAttributeAdapter(principal);

    // Retrieve the strongly typed claims from the principal
    var privileges = mgr.GetValue(CommonOioAttributes.PrivilegesIntermediate);
    var rights = privileges.PrivilegeGroups.Where(x => x.Scope == "myScope")

In the above example the privileges are actually stored as base64 and UTF8 encoded XML within a string typed attribute value of the token.
The marshal `CommonOioAttributes.PrivilegesIntermediate` defines both the attribute name (claim type) as well as the encoding of the
attribute value (claim value) within the security token. The marshal is used for storing/encoding as well as decoding/retrieval of the
attribute value (claim value).