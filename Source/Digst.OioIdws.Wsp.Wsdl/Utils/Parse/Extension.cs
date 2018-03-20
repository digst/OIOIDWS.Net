namespace Digst.OioIdws.Wsp.Wsdl.Utils.Parse
{
    using System.Xml;

    public static class Extension
    {
        public static Element Policy(XmlElement extension)
        {
            return Parse("Policy", extension);
        }

        public static Element EndpointReference(XmlElement extension)
        {
            return Parse("EndpointReference", extension);
        }

        private static Element Parse(
            string localName, XmlElement extension)
        {
            if (extension is XmlElement && extension.LocalName == localName)
            {
                return AuxParse(extension);
            }

            return null;
        }

        private static string TextValue(XmlElement element)
        {
            if(element.FirstChild is XmlText &&
               element.FirstChild.Equals(element.LastChild))
            {
                return element.FirstChild.Value;
            }

            return null;
        }

        private static Element AuxParse(XmlElement element)
        {
            var value = new Element
            {
                Name = element.LocalName,
                Value = TextValue(element),
                NamespaceUri = element.NamespaceURI
            };

            for (int i = 0; i < element.Attributes.Count; ++i)
            {
                if (element.Attributes[i] is XmlAttribute attribute)
                {
                    value.Attributes.Add(
                        new Attribute
                        {
                            Name = attribute.LocalName,
                            Value = attribute.Value,
                            NamespaceUri = attribute.NamespaceURI
                        });
                }
            }

            for (int i = 0; i < element.ChildNodes.Count; ++i)
            {
                if (element.ChildNodes[i] is XmlElement child)
                {
                    value.Children.Add(AuxParse(child));
                }
            }

            return value;

        }
   }
}