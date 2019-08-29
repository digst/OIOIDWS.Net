using System;
using System.Xml;
using System.Xml.Linq;

namespace Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken
{
    public class ComplexSamlAttributeValue
    {
        public ComplexSamlAttributeValue(XNode payload, XName xsiType=null) 
        {
            AttributeValueElement = new XElement(Saml2Constants.Namespaces.saml+"AttributeValue", payload);

            if (xsiType != null)
            {
                AttributeValueElement.SetAttributeValue(XNamespace.Xmlns+"t", xsiType.NamespaceName);
                AttributeValueElement.SetAttributeValue(Saml2Constants.Namespaces.xsi+"type", "t:"+xsiType.LocalName);
            }
        }

        public ComplexSamlAttributeValue(string value, XName xsiType=null) : this(new XText(value), xsiType)
        {
        }

        public XElement AttributeValueElement { get; }
    }


}