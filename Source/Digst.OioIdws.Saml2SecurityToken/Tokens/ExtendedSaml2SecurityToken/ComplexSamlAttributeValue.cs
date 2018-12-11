using System;
using System.Xml;
using System.Xml.Linq;

namespace Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken
{
    public class ComplexSamlAttributeValue
    {
        public ComplexSamlAttributeValue(XNode payload)
        {
            AttributeValueElement = new XElement(Saml2Constants.Namespaces.saml+"AttributeValue", payload);
        }

        public ComplexSamlAttributeValue(string value, XName xsiType=null) : this(new XText(value))
        {
        }

        public XElement AttributeValueElement { get; }
    }


}