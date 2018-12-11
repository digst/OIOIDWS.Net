using System;
using System.Collections.Generic;
using System.Reflection;

namespace Digst.OioIdws.Common.Attributes
{
    /// <summary>
    /// Converter which supports converting between an enum based type and the corresponding SAML value string
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class SamlAttributeValueConverter<T> 
    {
        private static readonly IDictionary<string, T> _stringToEnumValue = new Dictionary<string, T>();
        private static readonly IDictionary<T, string> _enumValueToString = new Dictionary<T, string>();

        static SamlAttributeValueConverter()
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static);
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
        /// Converts an enum based value to the corresponding SAML value string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string ToSamlValueString(T value)
        {
            if (!_enumValueToString.ContainsKey(value)) throw new ArgumentOutOfRangeException(nameof(value), $"Only values defined as explicit enum values (fields) of the type {typeof(T).FullName} are allowed.");
            return _enumValueToString[value];
        }

        /// <summary>
        /// Converts a SAML value string to an enum value
        /// </summary>
        /// <param name="samlValueString">The saml value string.</param>
        /// <returns></returns>
        public static T FromSamlValueString(string samlValueString)
        {
            return _stringToEnumValue[samlValueString];

        }
    }
}