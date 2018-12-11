using System;
using System.Linq;
using System.Xml.Linq;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.SamlAttributes.AttributeMarshals
{


    /// <summary>
    /// Describes a simple string based SAML attribute. Using this marshal the .NET string values is the same as the SAML value, i.e. no the value is not transformed or serialized.
    /// </summary>
    public sealed class StringSamlAttributeMarshal : SamlAttributeMarshal<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringSamlAttributeMarshal"/> class.
        /// </summary>
        public StringSamlAttributeMarshal(string name, Uri nameFormat = null, XName xsiType = null) : base(name, nameFormat, xsiType)
        {
        }

        /// <inheritdoc />
        public override string GetValue(AttributeAdapter attributeAdapter)
        {
            return GetValues(attributeAdapter).SingleOrDefault()?.AttributeValueElement?.Value;
        }

        /// <inheritdoc />
        public override void SetValue(AttributeAdapter attributeAdapter, string value)
        {
            if (value == null)
            {
                Remove(attributeAdapter);
            }
            else
            {
                SetValues(attributeAdapter, new[] { new ComplexSamlAttributeValue(value) });
            }
        }
    }
}