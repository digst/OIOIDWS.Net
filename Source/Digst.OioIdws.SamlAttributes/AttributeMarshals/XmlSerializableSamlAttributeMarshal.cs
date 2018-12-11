using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Digst.OioIdws.SamlAttributes.AttributeMarshals
{

    /// <summary>
    /// Represents a structured type as XML within the attribute value by serializing the structured type. The type must be serializable to XML.
    /// </summary>
    /// <typeparam name="T">The structured type to be serialized/deserialized to/from XML within the attribute</typeparam>
    public class XmlSerializableSamlAttributeMarshal<T> : XmlReaderWriterAttributeMarshal<T>
    {
        private readonly XmlSerializer _serializer;



        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializableSamlAttributeMarshal{T}"/> class.
        /// </summary>
        public XmlSerializableSamlAttributeMarshal(string name, Uri nameFormat = null, XName xsiType = null) : base(name, nameFormat, xsiType)
        {
            _serializer = new XmlSerializer(typeof(T));
        }

        /// <inheritdoc />
        protected override T ReadAttributeValue(XmlReader reader)
        {
            reader.ReadStartElement();
            var empty = reader.IsEmptyElement;
            var value = (T)_serializer.Deserialize(reader.ReadSubtree());
            if (!empty)
            {
                reader.ReadEndElement();
            }
            return value;
        }

        /// <inheritdoc />
        protected override void WriteAttributeValue(XmlWriter writer, T value)
        {
            _serializer.Serialize(writer, value);
        }
    }
}