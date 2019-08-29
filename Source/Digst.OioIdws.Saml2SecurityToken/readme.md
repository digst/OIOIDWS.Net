# Extended SAML2 Security Token and Token Handler

The SAML2 assertion (token) and -handler built into the .NET base class library does not support "complex" attribute values in SAML attribute statements.
*Complex* attribute values refers to attribute values that are XML based as opposed to simple string based, e.g.

    <AttributeValue>
        <hog:MyOwnXmlElement someAttribute="42" xmlns:hog="urn:heart-of-gold">
            <hog:Passenger>Zaphod Beeblebrox</hog:Passenger>
        </hog:MyOwnXmlElement>
    </AttributeValue>

The .NET SAML2 assertion type *Saml2Assertion* does contain extension points which allows it to be extended with this capability. This library
uses these extension points to allow for complex attribute values.

Note that the library does not define a replacement *token type*, rather it defines a number classes for replacing statements, attributes and values along
with a replacement token handler which can handle these exended statements, attributes and values.

Most of the logic to serialize and deserialize, extract claims etc are places in the corresponding *token handler* ExtendedSaml2SecurityTokenHandler.

To use the token and -handler you must configure the extended token handler to be used instead of the built-in SAML2 security token handler:

    <system.identityModel>
        <identityConfiguration>
            <!--The Saml2SecurityTokenHandler security token handler must be replacded by a custom extended handler which supports tokens with "complex" (xml payload) attribute values -->
            <securityTokenHandlers>
                <remove type="System.IdentityModel.Tokens.Saml2SecurityTokenHandler, System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"/>
                <add type="Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken.ExtendedSaml2SecurityTokenHandler, Digst.OioIdws.SecurityTokens"/>
            </securityTokenHandlers>
        </identityConfiguration>
    </system.identityModel>

