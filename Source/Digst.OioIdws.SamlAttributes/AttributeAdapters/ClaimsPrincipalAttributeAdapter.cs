using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Xml.Linq;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.SamlAttributes.AttributeAdapters
{
    /// <summary>
    /// Attribute manager which represents attribute values as claims of a <see cref="ClaimsProvider"/>
    /// </summary>
    public class ClaimsPrincipalAttributeAdapter : AttributeAdapter
    {
        private readonly ClaimsPrincipal _principal;

        public ClaimsPrincipalAttributeAdapter(ClaimsPrincipal principal)
        {
            _principal = principal;
        }

        /// <inheritdoc />
        public override void SetAttributeValues(string attributeName, Uri nameFormat, XName xsiType, IEnumerable<ComplexSamlAttributeValue> values, string friendlyname)
        {
            throw new NotSupportedException($"Modifying the underlying attributes are not supported for {typeof(ClaimsPrincipalAttributeAdapter).FullName}");
        }

        /// <inheritdoc />
        public override IEnumerable<ComplexSamlAttributeValue> GetAttributeValues(string attributeName)
        {
            return _principal.Claims.Where(x => x.Type == attributeName).Select(x => new ComplexSamlAttributeValue(x.Value));
        }

        /// <inheritdoc />
        public override bool HasAttribute(string attributeName)
        {
            return _principal.HasClaim(x => x.Type == attributeName);
        }

        /// <inheritdoc />
        public override void RemoveAttribute(string attributeName)
        {
            throw new NotSupportedException($"Modifying the underlying attributes are not supported for {typeof(ClaimsPrincipalAttributeAdapter).FullName}");
        }
    }
}