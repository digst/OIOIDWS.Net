using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Digst.OioIdws.SamlAttributes.AttributeMarshals
{
    /// <summary>
    /// Represents an marshal which allows setting and getting a value which is XML serializable. The value is represented UTF8+Base64 encoded within the {AttributeValue} element.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="EncodedStringSamlAttributeDescriptEncodedStringSamlAttributeDescriptor{T}" />
    public class XmlSerializableBase64SamlAttributeMarshal<T> : EncodedStringSamlAttributeMarshal<T>
    {
        private readonly XmlSerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializableBase64SamlAttributeMarshal{T}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="nameFormat">The name format.</param>
        /// <param name="xsiType">Type of the xsi.</param>
        public XmlSerializableBase64SamlAttributeMarshal(string name, Uri nameFormat = null, XName xsiType = null) : base(name, nameFormat, xsiType)
        {
            _serializer = new XmlSerializer(typeof(T));
        }

        /// <inheritdoc />
        protected override T Decode(string encodedString)
        {
            var xml = Encoding.UTF8.GetString(Convert.FromBase64String(encodedString));
            using (var xr = XmlReader.Create(new StringReader(xml), new XmlReaderSettings()))
            {
                return (T)_serializer.Deserialize(xr);
            }
        }

        /// <inheritdoc />
        protected override string Encode(T value)
        {
            using (var mem = new MemoryStream())
            {
                using (var xw = XmlWriter.Create(mem, new XmlWriterSettings() { Encoding = Encoding.UTF8 }))
                {
                    _serializer.Serialize(xw, value);
                }
                return Convert.ToBase64String(mem.ToArray());
            }
        }

    }
}