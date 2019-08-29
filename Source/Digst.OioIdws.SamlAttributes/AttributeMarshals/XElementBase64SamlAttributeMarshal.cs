using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Digst.OioIdws.SamlAttributes.AttributeMarshals
{
    /// <summary>
    /// A marshal for marshalling XElements through a SAML attribute as Base64 encoded strings.
    ///
    /// An XElement is serialized to a SAML attribute by first serializing the XElement to an XML string
    /// and then base64 encode that string. Thus, the SAML attribute will contain the base64 encoded
    /// XML representation of the XElement.
    ///
    /// A SAML attributes is deserialized to an XElement instance by decoding the base64 string into
    /// an XML string and then deserialize an XElement from that XML string.
    /// </summary>
    public class XElementBase64SamlAttributeMarshal : EncodedStringSamlAttributeMarshal<XElement>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="XElementBase64SamlAttributeMarshal"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="nameFormat">The name format.</param>
        /// <param name="xsiType">Type of the xsi.</param>
        public XElementBase64SamlAttributeMarshal(string name, Uri nameFormat = null, XName xsiType = null) : base(name, nameFormat, xsiType)
        {
        }

        /// <inheritdoc />
        protected override XElement Decode(string encodedString)
        {
            var xml = Encoding.UTF8.GetString(Convert.FromBase64String(encodedString));
            return XElement.Load(new StringReader(xml));
        }

        /// <inheritdoc />
        protected override string Encode(XElement value)
        {
            using (var mem = new MemoryStream())
            {
                var sw = new StringWriter();
                using (var xw = XmlWriter.Create(sw))
                {
                    value.WriteTo(xw);
                }
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(sw.ToString()));
            }
        }

    }
}