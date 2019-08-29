using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.SamlAttributes.AttributeAdapters
{
    /// <summary>
    ///   <para>An AttributeAdapter which manages the attributes in a RequestClaimsCollection. Use this class to prepare attributes/claims to be sent through a RequestClaimCollection. This is typically used when requesting a token from a Security Token Service (STS).</para>
    /// </summary>
    /// <seealso cref="Digst.OioIdws.SamlAttributes.AttributeAdapters.AttributeAdapter" />
    public class RequestClaimCollectionAttributeAdapter : AttributeAdapter
    {
        private readonly RequestClaimCollection _claims;
        private readonly bool _isOptional;

        /// <summary>Initializes a new instance of the <see cref="RequestClaimCollectionAttributeAdapter"/> class.</summary>
        /// <param name="claims">The claims collection where the claims are stored. The adapter will represent attributes/claims as claims in this collection.</param>
        /// <param name="isOptional">Indicates whether the <em>optional</em> flag should be set for claims within the RequestClaimCollection.
        /// If you need to send both optional and non-optional claims requests, you should use two RequestClaimCollectionAdapter's based on the same RequestClaimCollection.
        /// </param>
        public RequestClaimCollectionAttributeAdapter(RequestClaimCollection claims, bool isOptional)
        {
            _claims = claims;
            _isOptional = isOptional;
        }

        /// <summary>
        /// Adds or replaces a SAML attribute. If the attribute already exists, it is replaced with the an new attribute with the new values.
        /// If the attribute does not exist it is created with the specified values
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="nameFormat"></param>
        /// <param name="xsiType"></param>
        /// <param name="values">A collection of <see cref="ComplexSamlAttributeValue"/> objects representing the values of the attribute.</param>
        /// <param name="friendlyName"></param>
        public override void SetAttributeValues(string attributeName, Uri nameFormat, XName xsiType, IEnumerable<ComplexSamlAttributeValue> values, string friendlyName)
        {
            if (HasAttribute(attributeName)) RemoveAttribute(attributeName);
            foreach (var value in values)
            {
                if (value.AttributeValueElement.HasElements)
                {
                    // The attribute value consists of an XML element (as opposed to a simple string).
                    var element = value.AttributeValueElement.Elements().Single();

                    // Hack: The request claim collection can only contain simple strings. When a SAML attribute value contains structured
                    // content (an XML element) we represent it as the serialized XML element. When forming the message when transmitting
                    // the collection we will look for strings that are well-formed XML and then *assume* that they represent XML elements.
                    _claims.Add(new RequestClaim(attributeName, _isOptional, element.ToString(SaveOptions.DisableFormatting).Trim()));
                }
                else
                {
                    // store as text
                    var s = value.AttributeValueElement.Value;
                    _claims.Add(new RequestClaim(attributeName, _isOptional, s));
                }
            }
        }

        /// <summary>Gets all the values of a specific attribute</summary>
        /// <param name="attributeName"></param>
        /// <returns>A sequence of <see cref="ComplexSamlAttributeValue"/> objects.</returns>
        public override IEnumerable<ComplexSamlAttributeValue> GetAttributeValues(string attributeName)
        {
            var values = _claims.Where(x => x.ClaimType == attributeName);
            foreach (var value in values)
            {
                var claimValue = value.Value.Trim();
                if (claimValue.StartsWith("<") && claimValue.EndsWith(">"))
                {
                    // The claim value looks like it *could* be XML; try to parse it as XML
                    XNode valueNode;
                    try
                    {
                        valueNode = XElement.Parse(claimValue);
                    }
                    catch (XmlException)
                    {
                        // The claimValue could not be parsed as XML, fall back to string.
                        valueNode = new XText(value.Value);
                    }
                    yield return new ComplexSamlAttributeValue(valueNode);
                }
                else yield return new ComplexSamlAttributeValue(new XText(value.Value));
            }
        }

        public override bool HasAttribute(string attributeName)
        {
            return _claims.Any(x => x.ClaimType == attributeName);
        }

        public override void RemoveAttribute(string attributeName)
        {
            var toDelete = _claims.Where(x => x.ClaimType == attributeName).ToList();
            foreach (var claim in toDelete)
            {
                _claims.Remove(claim);
            }
        }
    }
}