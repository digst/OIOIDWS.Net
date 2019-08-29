using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Digst.OioIdws.SamlAttributes.AttributeAdapters;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.SamlAttributes.AttributeMarshals
{
    /// <summary>
    /// Describes a SAML attribute which has multiple (zero or more) string values.
    /// </summary>
    /// <seealso cref="SamlAttributeMarshal{T}.Collections.Generic.IEnumerable{System.String}}" />
    public class MultiStringSamlAttributeMarshal : SamlAttributeMarshal<IEnumerable<string>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiStringSamlAttributeMarshal"/> class.
        /// </summary>
        public MultiStringSamlAttributeMarshal(string name, Uri nameFormat = null, XName xsiType = null) : base(name, nameFormat, xsiType)
        {
        }


        /// <inheritdoc />
        public override IEnumerable<string> GetValue(AttributeAdapter attributeAdapter)
        {
            return attributeAdapter.GetAttributeValues(Name).Select(x => x.AttributeValueElement.Value);
        }

        /// <inheritdoc />
        public override void SetValue(AttributeAdapter attributeAdapter, IEnumerable<string> strings)
        {
            if (strings == null) throw new ArgumentNullException(nameof(strings));
            SetValues(attributeAdapter, strings.Select(s => new ComplexSamlAttributeValue(s)));
        }
    }
}