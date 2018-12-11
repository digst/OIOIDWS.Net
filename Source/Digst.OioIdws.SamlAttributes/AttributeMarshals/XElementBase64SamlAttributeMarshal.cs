using System;
using System.Text;
using System.Xml.Linq;

namespace Digst.OioIdws.SamlAttributes.AttributeMarshals
{
    /// <summary>
    /// Describes an attribute which is represented an an XElement within the application but represented as Base84+UTF8 encoded XML within the SAML attribute value.
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
            return XElement.Parse(xml.Trim());
        }

        /// <inheritdoc />
        protected override string Encode(XElement value)
        {
            var xml = value.ToString(SaveOptions.DisableFormatting);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(xml));
        }

    }
}