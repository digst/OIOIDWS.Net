using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Digst.OioIdws.SamlAttributes.AttributeAdapters;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.SamlAttributes.AttributeMarshals
{
    /// <summary>
    /// Describes how to encode .NET enum type as a SAML attribute.
    ///
    /// The values of the enum type must be specified with  a <see cref="SamlAttributeValueAttribute"/>
    /// metadata attribute to define the string representation within the AttributeValue element.
    /// </summary>
    public sealed class EnumSamlAttributeMarshal<T> : SamlAttributeMarshal<T> where T:struct 
    {
        private readonly T? _magicNoValue;
        private readonly IDictionary<string, T> _stringToEnumValue = new Dictionary<string, T>();
        private readonly IDictionary<T,string> _enumValueToString = new Dictionary<T, string>();

        public EnumSamlAttributeMarshal(string name, Uri nameFormat = null, XName xsiType = null, T? magicNoValue=null) : base(name, nameFormat, xsiType)
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("The type must be an enum type", nameof(T));
            _magicNoValue = magicNoValue;

            var fields = typeof(T).GetFields(BindingFlags.Public|BindingFlags.Static);
            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute<SamlAttributeValueAttribute>();
                var value = (T)field.GetValue(null);
                var stringValue = attr.StringValue ?? field.Name;

                _enumValueToString.Add(value, stringValue);
                _stringToEnumValue.Add(stringValue, value);
            }
        }


        /// <summary>
        /// Gets the strongly typed value.
        /// </summary>
        /// <param name="attributeAdapter">The attribute adapter.</param>
        /// <returns></returns>
        public override T GetValue(AttributeAdapter attributeAdapter)
        {
            var encodedString = GetValues(attributeAdapter).Single().AttributeValueElement.Value;
            return Decode(encodedString);
        }


        /// <summary>
        /// Sets the strongly typed value.
        /// </summary>
        /// <param name="attributeAdapter">The attribute adapter.</param>
        /// <param name="value">The value.</param>
        public override void SetValue(AttributeAdapter attributeAdapter, T value)
        {
            if (_magicNoValue.HasValue && (_magicNoValue.Value.Equals(value)))
            {
                Remove(attributeAdapter);
            }
            else
            {
                var encodedString = Encode(value);
                SetValues(attributeAdapter, new[] { new ComplexSamlAttributeValue(encodedString) });
            }
        }

        /// <summary>
        /// Decodes the specified encoded string into an enum value.
        /// </summary>
        /// <param name="encodedString">The encoded string.</param>
        /// <returns></returns>
        private T Decode(string encodedString)
        {
            return SamlAttributeValueConverter<T>.FromSamlValueString(encodedString);
        }

        /// <summary>
        /// Encodes the specified value into the SAML string representation
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">value</exception>
        private string Encode(T value)
        {
            return SamlAttributeValueConverter<T>.ToSamlValueString(value);
        }
    }
}