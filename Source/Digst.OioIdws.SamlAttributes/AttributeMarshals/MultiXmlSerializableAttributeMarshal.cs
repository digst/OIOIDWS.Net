using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.SamlAttributes.AttributeMarshals
{

    /// <summary>
    /// Describes a SAML attribute which has potentially multiple values, each an XML value serialized from/to a .NET type using XML serialization. 
    /// </summary>
    /// <typeparam name="T">The .NET type. The type must support XML serialization</typeparam>
    public class MultiXmlSerializableAttributeMarshal<T> : SamlAttributeMarshal<IEnumerable<T>>
    {
        private readonly XmlSerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiXmlSerializableAttributeMarshal{T}"/> class.
        /// </summary>
        public MultiXmlSerializableAttributeMarshal(string name, Uri nameFormat = null, XName xsiType = null) : base(name, nameFormat, xsiType)
        {
            _serializer = new XmlSerializer(typeof(T));
        }


        /// <inheritdoc />
        public override IEnumerable<T> GetValue(AttributeAdapter attributeAdapter)
        {
            foreach (var attributeValue in GetValues(attributeAdapter))
            {
                var xml = attributeValue.AttributeValueElement.Elements().Single();
                using (var xr = xml.CreateReader())
                {
                    yield return (T)_serializer.Deserialize(xr);
                }
            }
        }

        /// <inheritdoc />
        public override void SetValue(AttributeAdapter attributeAdapter, IEnumerable<T> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            var attributeValues = new List<ComplexSamlAttributeValue>();
            foreach (var value in values)
            {
                var sw = new StringWriter();
                var xw = XmlWriter.Create(sw, new XmlWriterSettings() { OmitXmlDeclaration = true });
                _serializer.Serialize(sw, value);
                var element = XElement.Parse(sw.ToString());
                attributeValues.Add(new ComplexSamlAttributeValue(element));
            }
            SetValues(attributeAdapter, attributeValues);
        }
    }
}