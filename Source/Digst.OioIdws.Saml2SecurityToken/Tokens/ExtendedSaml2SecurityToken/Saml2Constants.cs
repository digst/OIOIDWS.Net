using System;
using System.Xml.Linq;

namespace Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken
{
    public static class Saml2Constants
    {

        public static class Namespaces
        {
            public const string SamlString = "urn:oasis:names:tc:SAML:2.0:assertion";
            public const string XmlSchemaInstanceString = "http://www.w3.org/2001/XMLSchema-instance";
            public const string XmlSchemaString = "http://www.w3.org/2001/XMLSchema";

            // ReSharper disable once InconsistentNaming using all lowercase to mimic how namespace qualifiers are typically used
            public static readonly XNamespace saml = XNamespace.Get(SamlString);

            // ReSharper disable once InconsistentNaming using all lowercase to mimic how namespace qualifiers are typically used
            public static readonly XNamespace xsi = XNamespace.Get(XmlSchemaInstanceString);

            // ReSharper disable once InconsistentNaming using all lowercase to mimic how namespace qualifiers are typically used
            public static readonly XNamespace xs = XNamespace.Get(XmlSchemaString);

            // ReSharper disable once InconsistentNaming using all lowercase to mimic how namespace qualifiers are typically used
            public static readonly XNamespace claim2009 = XNamespace.Get("http://schemas.xmlsoap.org/ws/2009/09/identity/claims");

        }


        public static class ElementNames
        {
            public static readonly XName AttributeStatementXName = Namespaces.saml + "AttributeStatement";
            public static readonly XName AttributeXName = Namespaces.saml + "Attribute";
            public static readonly XName AttributeValueXName = Namespaces.saml + "AttributeValue";


        }

        public static class AttributeNames { 

            public static readonly XName NameXName = Namespaces.saml + "Name";
            public static readonly XName NameFormatXName = Namespaces.saml + "NameFormat";
            public static readonly XName FriendlyNameXName = Namespaces.saml + "FriendlyName";

            public static readonly XName OriginalIssuerXName = Namespaces.claim2009 + "Issuer";

            public static readonly XName TypeXName = Namespaces.xsi + "type";
            public static readonly XName Nil = Namespaces.xsi + "nil";
        }


        public static class ConfirmationMethods
        {
            public const string BearerString = "urn:oasis:names:tc:SAML:2.0:cm:bearer";
            public const string HolderOfKeyString = "urn:oasis:names:tc:SAML:2.0:cm:holder-of-key";
            public const string SenderVouchesString = "urn:oasis:names:tc:SAML:2.0:cm:sender-vouches";

            public static readonly Uri Bearer = new Uri(BearerString);
            public static readonly Uri HolderOfKey = new Uri(HolderOfKeyString);
            public static readonly Uri SenderVouches = new Uri(SenderVouchesString);
        }


    }
}