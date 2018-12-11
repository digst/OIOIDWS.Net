using System;
using System.Xml;
using System.Xml.Linq;

namespace Digst.OioIdws.SamlAttributes.AttributeMarshals
{


    /// <summary>
    /// Describes an attribute that is represented as an XElement within the application and encoded as XML within the attribute value.
    /// </summary>
    /// <seealso cref="XmlReaderWriterAttributeMarshal{T}.Xml.Linq.XElement}" />
    public class XElementSamlAttributeMarshal : XmlReaderWriterAttributeMarshal<XElement>
    {

        public XElementSamlAttributeMarshal(string name, Uri nameFormat = null, XName xsiType = null) : base(name, nameFormat, xsiType)
        {
        }

        protected override XElement ReadAttributeValue(XmlReader reader)
        {
            return (XElement)XNode.ReadFrom(reader);
        }

        protected override void WriteAttributeValue(XmlWriter writer, XElement value)
        {
            value.WriteTo(writer);
        }
    }
}