using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Digst.OioIdws.SamlAttributes.AttributeMarshals;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.SamlAttributes.AttributeAdapters
{



    /// <summary>
    /// Manages a set of SAML attributes. Supports using <see cref="SamlAttributeMarshal{T}"/>s to access the attribute values in a type safe manner.
    /// </summary>
    public abstract class AttributeAdapter
    {
        /// <summary>
        /// Adds or replaces a SAML attribute. If the attribute already exists, it is replaced with the an new attribute with the new values.
        /// If the attribute does not exist it is created with the specified values
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="nameFormat"></param>
        /// <param name="xsiType"></param>
        /// <param name="values">A collection of <see cref="ComplexSamlAttributeValue"/> objects representing the values of the attribute.</param>
        /// <param name="friendlyName"></param>
        public abstract void SetAttributeValues(string attributeName, Uri nameFormat, XName xsiType, IEnumerable<ComplexSamlAttributeValue> values, string friendlyName);

        /// <summary>
        /// Gets all the values of a specific attribute
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns>A sequence of <see cref="ComplexSamlAttributeValue"/> objects.</returns>
        public abstract IEnumerable<ComplexSamlAttributeValue> GetAttributeValues(string attributeName);

        /// <summary>
        /// Returns a boolean value indicating whether an attribute with the given name exists or not.
        /// </summary>
        public abstract bool HasAttribute(string attributeName);

        /// <summary>
        /// Removes an attribute it it exists.
        /// </summary>
        /// <param name="attributeName"></param>
        public abstract void RemoveAttribute(string attributeName);


        /// <summary>
        /// Gets a structured value of a SAML attribute. The marshal specifies the type of the value and how it is encoded within the <see cref="ComplexSamlAttributeValue"/>
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="marshal">The marshal which specifies the name and encoding og the attribute and value</param>
        /// <returns>The value of the attribute</returns>
        public T GetValue<T>(SamlAttributeMarshal<T> marshal)
        {
            return marshal.GetValue(this);
        }

        /// <summary>
        /// Sets the value of an attribute using an <see cref="SamlAttributeMarshal{T}"/>
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="marshal">The accessot specifying the name and encoding/formatting of the value</param>
        /// <param name="value"></param>
        public void SetValue<T>(SamlAttributeMarshal<T> marshal, T value)
        {
            marshal.SetValue(this, value);
        }

        /// <summary>
        /// Indicates whether the provider has an attribute specified by the marshal nor not
        /// </summary>
        /// <param name="marshal">The marshal which specifies the name of the attribute</param>
        /// <returns></returns>
        public bool HasAttribute(SamlAttributeMarshal marshal)
        {
            return HasAttribute(marshal.Name);
        }


        /// <summary>
        /// Removes an attribute and all of its values, if it exists
        /// </summary>
        /// <param name="marshal"></param>
        public void RemoveAttribute(SamlAttributeMarshal marshal)
        {
            RemoveAttribute(marshal.Name);
        }
    }
}