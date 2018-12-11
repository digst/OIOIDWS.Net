using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Digst.OioIdws.SamlAttributes.AttributeMarshals
{
    /// <summary>
    /// Describes an attribute which is an XmlDocument within the application but encoded as Base64+UTF8 XML within the {AttributeValue} element
    /// </summary>
    public class XmlDocumentBase64SamlAttributeMarshal : EncodedStringSamlAttributeMarshal<XmlDocument>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDocumentBase64SamlAttributeMarshal"/> class.
        /// </summary>
        public XmlDocumentBase64SamlAttributeMarshal(string name, Uri nameFormat = null, XName xsiType = null) : base(name, nameFormat, xsiType)
        {
        }

        /// <inheritdoc />
        protected override XmlDocument Decode(string encodedString)
        {
            var xml = Encoding.UTF8.GetString(Convert.FromBase64String(encodedString));
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }

        /// <inheritdoc />
        protected override string Encode(XmlDocument doc)
        {
            using (var mem = new MemoryStream())
            {
                using (var xw = XmlWriter.Create(mem, new XmlWriterSettings() {Encoding = Encoding.UTF8}))
                {
                    doc.Save(xw);
                }
                return Convert.ToBase64String(mem.ToArray());
            }
        }

    }
}