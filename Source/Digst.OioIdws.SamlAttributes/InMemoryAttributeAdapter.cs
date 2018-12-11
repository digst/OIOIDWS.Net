using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.SamlAttributes
{

    /// <summary>
    /// A transient in-memory implementation of an AttributeAdapter. Intended to be used for testing only
    /// </summary>
    public class InMemoryAttributeAdapter : AttributeAdapter
    {
        private readonly Dictionary<string, ComplexSamlAttribute> _attributes;

        /// <inheritdoc />
        public InMemoryAttributeAdapter()
        {
            _attributes = new Dictionary<string, ComplexSamlAttribute>();
        }

        /// <inheritdoc />
        public override void SetAttributeValues(string attributeName, Uri nameFormat, XName xsiType, IEnumerable<ComplexSamlAttributeValue> values, string friendlyName)
        {
            _attributes[attributeName] = new ComplexSamlAttribute(attributeName, nameFormat, values) { FriendlyName = friendlyName };
        }

        /// <inheritdoc />
        public override IEnumerable<ComplexSamlAttributeValue> GetAttributeValues(string attributeName)
        {
            return _attributes[attributeName].Values;
        }

        /// <inheritdoc />
        public override bool HasAttribute(string attributeName)
        {
            return _attributes.ContainsKey(attributeName);
        }

        /// <inheritdoc />
        public override void RemoveAttribute(string attributeName)
        {
            _attributes.Remove(attributeName);
        }
    }
}