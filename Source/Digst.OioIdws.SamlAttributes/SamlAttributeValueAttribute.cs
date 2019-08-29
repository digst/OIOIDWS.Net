using System;

namespace Digst.OioIdws.SamlAttributes
{


    /// <summary>
    /// Defines the string value that will represent a enum value when represented within a SAML {AttributeValue} element
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Field)]
    public class SamlAttributeValueAttribute : Attribute
    {
        public string StringValue { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SamlAttributeValueAttribute"/> class.
        /// </summary>
        /// <param name="stringValue">The string value which represents the value when serialized as an AttributeValue element.</param>
        public SamlAttributeValueAttribute(string stringValue)
        {
            StringValue = stringValue;
        }


    }
}