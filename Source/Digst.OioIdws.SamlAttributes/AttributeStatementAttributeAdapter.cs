using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Xml.Linq;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.SamlAttributes
{


    /// <summary>
    /// Manages attributes stored within a <see cref="Saml2ComplexAttributeStatement"/>
    /// </summary>
    public class AttributeStatementAttributeAdapter : AttributeAdapter
    {
        private readonly Saml2ComplexAttributeStatement _attributeStatement;

        public AttributeStatementAttributeAdapter(Saml2ComplexAttributeStatement attributeStatement)
        {
            _attributeStatement = attributeStatement;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeStatementAttributeAdapter"/> class based on an assertion which has a *single* <see cref="Saml2ComplexAttributeStatement"/>.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        public AttributeStatementAttributeAdapter(Saml2Assertion assertion) : this(assertion.Statements.OfType<Saml2ComplexAttributeStatement>().Single())
        {
        }

        /// <inheritdoc />
        public override void SetAttributeValues(string attributeName, Uri nameFormat, XName xsiType, IEnumerable<ComplexSamlAttributeValue> values, string friendlyName)
        {
            _attributeStatement.RemoveAttribute(attributeName);
            var attribute = _attributeStatement.Attributes.SingleOrDefault(x => x.Name == attributeName);
            if (attribute != null)
            {
                _attributeStatement.Attributes.Remove(attribute);
            }

            attribute = new ComplexSamlAttribute(attributeName, nameFormat, values, xsiType) { FriendlyName = friendlyName };
            _attributeStatement.Attributes.Add(attribute);
        }


        /// <inheritdoc />
        public override IEnumerable<ComplexSamlAttributeValue> GetAttributeValues(string attributeName)
        {
            if (!HasAttribute(attributeName)) return Enumerable.Empty<ComplexSamlAttributeValue>();
            var attribute = _attributeStatement.Attributes.SingleOrDefault(x => x.Name == attributeName);
            return attribute.Values;
        }


        /// <inheritdoc />
        public override bool HasAttribute(string attributeName)
        {
            return _attributeStatement.Attributes.Any(x => x.Name == attributeName);
        }


        /// <inheritdoc />
        public override void RemoveAttribute(string attributeName)
        {
            _attributeStatement.RemoveAttribute(attributeName);
        }

    }
}