using System;
using System.Text;
using System.Xml.Linq;

namespace Digst.OioIdws.SamlAttributes.AttributeMarshals
{
    /// <summary>
    /// An marshal for a string valued attribute which is Base64 encoded within the {AttributeValue} SAML element.
    /// By Base64 encoding a string value, the SAML value will not contain any "special" characters that would need further XML escaping.
    /// </summary>
    public class StringBase64SamlAttributeMarshal : EncodedStringSamlAttributeMarshal<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringBase64SamlAttributeMarshal"/> class.
        /// </summary>
        /// <param name="name">The saml attribute name</param>
        /// <param name="nameFormat">The name format.</param>
        /// <param name="xsiType">Explicit xsi:type, if specified</param>
        public StringBase64SamlAttributeMarshal(string name, Uri nameFormat = null, XName xsiType = null) : base(name, nameFormat, xsiType)
        {
        }

        /// <inheritdoc />
        protected override string Encode(string value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

        /// <inheritdoc />
        protected override string Decode(string encodedString)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encodedString));
        }
    }
}