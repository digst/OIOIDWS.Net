using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.SamlAttributes.AttributeMarshals
{
    /// <summary>
    /// Base class for descriptors that represent the value as XML within the SAML attribute value element.
    /// </summary>
    public abstract class XmlReaderWriterAttributeMarshal<T> : SamlAttributeMarshal<T>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlReaderWriterAttributeMarshal{T}"/> class.
        /// </summary>
        protected XmlReaderWriterAttributeMarshal(string name, Uri nameFormat=null, XName xsiType=null) : base(name, nameFormat, xsiType)
        {
        }

        /// <inheritdoc />
        public override T GetValue(AttributeAdapter attributeAdapter)
        {
            var rd = GetValues(attributeAdapter).Single().AttributeValueElement.CreateReader();
            rd.MoveToElement();
            return ReadAttributeValue(rd);
        }

        /// <inheritdoc />
        public override void SetValue(AttributeAdapter attributeAdapter, T value)
        {
            if (value == null)
            {
                Remove(attributeAdapter);
            }
            else
            {

                var sw = new StringWriter();
                var xw = XmlWriter.Create(sw, new XmlWriterSettings() {OmitXmlDeclaration = false});
                WriteAttributeValue(xw, value);
                var sr = new StringReader(sw.ToString());
                var element = XElement.Parse(sw.ToString());

                var complexValue = new ComplexSamlAttributeValue(element);
                SetValues(attributeAdapter, new[] {complexValue});
            }
        }

        /// <summary>
        /// Reads the attribute value from an <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        protected abstract T ReadAttributeValue(XmlReader reader);

        /// <summary>
        /// Writes the attribute value to an <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The value.</param>
        protected abstract void WriteAttributeValue(XmlWriter writer, T value);
    }
}