using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.Linq;

namespace Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken
{
    /// <summary>
    /// Represents the AttributeStatement element specified in [Saml2Core, 2.7.3].
    /// </summary>
    public class Saml2ComplexAttributeStatement : Saml2AttributeStatement
    {
        private readonly Collection<ComplexSamlAttribute> _attributes = new Collection<ComplexSamlAttribute>();

        /// <summary>
        /// Creates an instance of Saml2AttributeStatement.
        /// </summary>
        public Saml2ComplexAttributeStatement()
        {
        }

        /// <summary>
        /// Creates an instance of Saml2AttributeStatement.
        /// </summary>
        /// <param name="samlAttribute">The <see cref="Saml2Attribute"/> contained in this statement.</param>
        public Saml2ComplexAttributeStatement(ComplexSamlAttribute samlAttribute) : this(new[] { samlAttribute })
        {
        }

        /// <summary>
        /// Creates an instance of Saml2AttributeStatement.
        /// </summary>
        /// <param name="attributes">The collection of <see cref="Saml2Attribute"/> elements contained in this statement.</param>
        public Saml2ComplexAttributeStatement(IEnumerable<ComplexSamlAttribute> attributes)
        {
            if (attributes == null) throw new ArgumentNullException(nameof(attributes));

            foreach (var attribute in attributes)
            {
                _attributes.Add(attribute);
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="Saml2Attribute"/> of this statement. [Saml2Core, 2.7.3]
        /// </summary>
        public Collection<ComplexSamlAttribute> Attributes => this._attributes;


        public void RemoveAttribute(string name)
        {
            var attribute = _attributes.SingleOrDefault(x => x.Name == name);
            if (attribute != null) _attributes.Remove(attribute);
        }
    }
}