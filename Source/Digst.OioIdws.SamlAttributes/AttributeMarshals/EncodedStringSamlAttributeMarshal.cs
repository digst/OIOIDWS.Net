using System;
using System.Linq;
using System.Xml.Linq;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.SamlAttributes.AttributeMarshals
{

    /// <summary>
    /// Base class for attribute descriptors that map between a .NET type and the SAML string based representation.
    /// </summary>
    public abstract class EncodedStringSamlAttributeMarshal<T> : SamlAttributeMarshal<T>
    {
        protected EncodedStringSamlAttributeMarshal(string name, Uri nameFormat = null, XName xsiType = null) : base(name, nameFormat, xsiType)
        {
        }

        /// <inheritdoc />
        public override T GetValue(AttributeAdapter attributeAdapter)
        {
            var encodedString = GetValues(attributeAdapter).Single().AttributeValueElement.Value;
            return Decode(encodedString);
        }


        /// <inheritdoc />
        public override void SetValue(AttributeAdapter attributeAdapter, T value)
        {
            if (value == null) Remove(attributeAdapter);
            else
            {
                var encodedString = Encode(value);
                SetValues(attributeAdapter, new[] { new ComplexSamlAttributeValue(encodedString) });
            }
        }

        /// <summary>
        /// Decodes the specified encoded string.
        /// </summary>
        /// <param name="encodedString">The encoded string.</param>
        /// <returns></returns>
        protected abstract T Decode(string encodedString);

        /// <summary>
        /// Encodes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected abstract string Encode(T value);
    }
}