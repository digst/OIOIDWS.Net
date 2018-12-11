using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.SamlAttributes.AttributeMarshals
{


    /// <summary>
    /// Base class for all attribute descriptors.
    /// </summary>
    public abstract class SamlAttributeMarshal
    {
        /// <summary>
        /// Gets the SAML name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the name format.
        /// </summary>
        public Uri NameFormat { get; }

        /// <summary>
        /// Gets the xml type of the attribute
        /// </summary>
        public XName XsiType { get; }

        /// <summary>
        /// Gets the friendly name of the attribute
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SamlAttributeMarshal"/> class.
        /// </summary>
        /// <param name="name">The name of the SAML attribute</param>
        /// <param name="nameFormat">The name format of the SAML attribute</param>
        /// <param name="xsiType">The XML type (as in xsi:type="...") of the SAML attribute.</param>
        /// <param name="friendlyName"></param>
        protected SamlAttributeMarshal(string name, Uri nameFormat = null, XName xsiType = null, string friendlyName = null)
        {
            Name = name;
            NameFormat = nameFormat;
            XsiType = xsiType;
            FriendlyName = friendlyName;
        }

        /// <summary>
        /// Determines whether the specified attribute adapter has a value of the SAML attribute described by the marshal.
        /// </summary>
        /// <param name="attributeAdapter">The attribute adapter.</param>
        /// <returns>
        ///   <c>true</c> if the specified attribute adapter has value; otherwise, <c>false</c>.
        /// </returns>
        protected bool HasValue(AttributeAdapter attributeAdapter) => attributeAdapter.HasAttribute(this);

        /// <summary>
        /// Sets the values of the attribute of this marshal within an attribute adapter.
        /// </summary>
        /// <param name="attributeAdapter">The attribute adapter.</param>
        /// <param name="values">The values.</param>
        protected void SetValues(AttributeAdapter attributeAdapter, IEnumerable<ComplexSamlAttributeValue> values)
        {
            attributeAdapter.SetAttributeValues(Name, NameFormat, XsiType, values, FriendlyName);
        }


        /// <summary>
        /// Gets the values of the attribute of this marshal from an attribute adapter.
        /// </summary>
        /// <param name="attributeAdapter">The attribute adapter.</param>
        /// <returns></returns>
        protected IEnumerable<ComplexSamlAttributeValue> GetValues(AttributeAdapter attributeAdapter)
        {
            return attributeAdapter.GetAttributeValues(Name);
        }


        /// <summary>
        /// Removes the attribute and all of its values of this attribute marshal from the attribute adapter
        /// </summary>
        /// <param name="attributeAdapter">The attribute adapter.</param>
        protected void Remove(AttributeAdapter attributeAdapter)
        {
            attributeAdapter.RemoveAttribute(Name);
        }

        /// <summary>
        /// Removes all of the values of this attribute marshal from the attribute adapter
        /// </summary>
        /// <param name="attributeAdapter">The attribute adapter.</param>
        protected void ClearValues(AttributeAdapter attributeAdapter)
        {
            attributeAdapter.RemoveAttribute(Name);
        }

        public abstract object GetAsObject(AttributeAdapter adapter);

        public abstract void SetAsObject(AttributeAdapter adapter, object value);
    }


    /// <summary>
    /// Base class for type specific attribute descriptors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SamlAttributeMarshal<T> : SamlAttributeMarshal
    {
        protected SamlAttributeMarshal(string name, Uri nameFormat = null, XName xsiType = null) : base(name, nameFormat, xsiType)
        {
        }

        /// <summary>
        /// Gets the strongly typed value.
        /// </summary>
        /// <param name="attributeAdapter">The attribute adapter.</param>
        /// <returns></returns>
        public abstract T GetValue(AttributeAdapter attributeAdapter);

        /// <summary>
        /// Sets the strongly typed value.
        /// </summary>
        /// <param name="attributeAdapter">The attribute adapter.</param>
        /// <param name="value">The value.</param>
        public abstract void SetValue(AttributeAdapter attributeAdapter, T value);


        public sealed override object GetAsObject(AttributeAdapter adapter)
        {
            return GetValue(adapter);
        }

        public sealed override void SetAsObject(AttributeAdapter adapter, object value)
        {
            if (!(value is T typedValue)) throw new ArgumentOutOfRangeException($"Value must be assignable to type {typeof(T).FullName}", nameof(value));
            SetValue(adapter, typedValue);
        }
    }
}