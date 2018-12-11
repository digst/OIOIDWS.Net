using System;
using System.Xml.Linq;

namespace Digst.OioIdws.SamlAttributes.AttributeMarshals
{
    /// <summary>
    /// An marshal for accessing boolean attribute values encoded as "true" or "false" within the {AttributeValue} element
    /// </summary>
    public class BooleanSamlAttributeMarshal : EncodedStringSamlAttributeMarshal<bool>
    {
        public BooleanSamlAttributeMarshal(string name, Uri nameFormat = null, XName xsiType = null) : base(name, nameFormat, xsiType)
        {
        }

        protected override bool Decode(string encodedString)
        {
            switch (encodedString)
            {
                case "true": return true;
                case "false": return false;
                default: throw new ArgumentOutOfRangeException(nameof(encodedString), @"Expected either ""true"" or ""false""");
            }
        }

        protected override string Encode(bool value)
        {
            return value ? "true" : "false";
        }

    }
}