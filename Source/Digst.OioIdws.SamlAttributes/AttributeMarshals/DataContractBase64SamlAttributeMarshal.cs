using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Digst.OioIdws.SamlAttributes.AttributeMarshals
{
    /// <summary>
    /// Represents a value of a structured type as Base64 encoded XML. The XML is the DataContract representation of the structured type. Thus, the structured type *must* support DataContract serialization.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataContractBase64SamlAttributeMarshal<T> : EncodedStringSamlAttributeMarshal<T>
    {
        private readonly DataContractSerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContractBase64SamlAttributeMarshal{T}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="nameFormat">The name format.</param>
        /// <param name="xsiType">Type of the xsi.</param>
        public DataContractBase64SamlAttributeMarshal(string name, Uri nameFormat = null, XName xsiType = null) : base(name, nameFormat, xsiType)
        {
            if (typeof(T).GetCustomAttribute<DataContractAttribute>() == null) throw new ArgumentException("Type must specify the DataContractAttribute attribute", nameof(T));
            _serializer = new DataContractSerializer(typeof(T));
        }

        /// <inheritdoc />
        protected override T Decode(string encodedString)
        {
            var xml = Encoding.UTF8.GetString(Convert.FromBase64String(encodedString));
            using (var xr = XmlReader.Create(new StringReader(xml), new XmlReaderSettings()))
            {
                return (T) _serializer.ReadObject(xr);
            }
        }

        /// <inheritdoc />
        protected override string Encode(T value)
        {
            using (var sw = new StringWriter())
            {
                using (var xw = XmlWriter.Create(sw, new XmlWriterSettings() { }))
                {
                    _serializer.WriteObject(xw, value);
                }
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(sw.ToString()));
            }
        }

    }
}