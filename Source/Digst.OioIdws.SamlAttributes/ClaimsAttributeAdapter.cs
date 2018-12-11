using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Xml.Linq;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.SamlAttributes
{
    /// <summary>
    /// Attribute manager which represents attribute values as claims of a <see cref="ClaimsProvider"/>
    /// </summary>
    public class ClaimsAttributeAdapter : AttributeAdapter
    {
        private readonly List<Claim> _claims;

        public ClaimsAttributeAdapter(IEnumerable<Claim> claims)
        {
            _claims = claims.ToList();
        }

        /// <inheritdoc />
        public override void SetAttributeValues(string attributeName, Uri nameFormat, XName xsiType, IEnumerable<ComplexSamlAttributeValue> values, string friendlyName)
        {
            throw new NotSupportedException($"Modifying the underlying attributes are not supported for {typeof(ClaimsAttributeAdapter).FullName}");
        }

        /// <inheritdoc />
        public override IEnumerable<ComplexSamlAttributeValue> GetAttributeValues(string attributeName)
        {
            return _claims.Where(x => x.Type == attributeName).Select(x => new ComplexSamlAttributeValue(x.Value));
        }

        /// <inheritdoc />
        public override bool HasAttribute(string attributeName)
        {
            return _claims.Any(x => x.Type == attributeName);
        }

        /// <inheritdoc />
        public override void RemoveAttribute(string attributeName)
        {
            throw new NotSupportedException($"Modifying the underlying attributes are not supported for {typeof(ClaimsAttributeAdapter).FullName}");
        }
    }
}