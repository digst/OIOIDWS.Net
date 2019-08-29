using System;
using System.Xml.Linq;

namespace Digst.OioIdws.SamlAttributes.AttributeMarshals
{
    /// <summary>
    /// An accessor for accessing an integer (Int32) attribute values encoded as a string of digits within the {AttributeValue} element
    /// </summary>
    public class IntSamlAttributeMarshal : EncodedStringSamlAttributeMarshal<int>
    {
        public IntSamlAttributeMarshal(string name, Uri nameFormat = null, XName xsiType = null) : base(name, nameFormat, xsiType)
        {
        }

        /// <inheritdoc />
        protected override int Decode(string encodedString)
        {
            return int.Parse(encodedString.Trim());
        }

        /// <inheritdoc />
        protected override string Encode(int value)
        {
            return value.ToString("D");
        }

    }
}