using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.SamlAttributes
{
    public class RequestClaimCollectionAttributeAdapter : AttributeAdapter
    {
        private readonly RequestClaimCollection _claims;
        private readonly bool _isOptional;

        public RequestClaimCollectionAttributeAdapter(RequestClaimCollection claims, bool isOptional)
        {
            _claims = claims;
            _isOptional = isOptional;
        }

        public override void SetAttributeValues(string attributeName, Uri nameFormat, XName xsiType, IEnumerable<ComplexSamlAttributeValue> values, string friendlyName)
        {
            if (HasAttribute(attributeName)) RemoveAttribute(attributeName);
            foreach (var value in values)
            {
                if (value.AttributeValueElement.HasElements)
                {
                    // store as xml
                    using (var mem = new MemoryStream())
                    {
                        using (var xw = XmlWriter.Create(mem, new XmlWriterSettings() {Encoding = Encoding.UTF8, OmitXmlDeclaration = true}))
                        {
                            value.AttributeValueElement.Elements().Single().WriteTo(xw);
                        }
                        var s = Encoding.UTF8.GetString(mem.ToArray());
                        _claims.Add(new RequestClaim(attributeName, _isOptional, s));
                    }
                }
                else
                {
                    // store as text
                    var s = value.AttributeValueElement.Value;
                    _claims.Add(new RequestClaim(attributeName, _isOptional, s));
                }
            }
        }

        public override IEnumerable<ComplexSamlAttributeValue> GetAttributeValues(string attributeName)
        {
            var values = _claims.Where(x => x.ClaimType == attributeName);
            foreach (var value in values)
            {
                if (value.Value.StartsWith("<"))
                {
                    // try to parse as XML
                    XNode valueNode;
                    try
                    {
                        valueNode = XElement.Parse(value.Value);
                    }
                    catch (XmlException)
                    {
                        // read as string instead
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