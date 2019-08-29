using System;
using System.Linq;
using System.Xml.Linq;
using Digst.OioIdws.SamlAttributes.AttributeAdapters;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.SamlAttributes.AttributeMarshals
{
    /// <summary>
    /// A marshal which maps a .NET value to/from a SAML string representation using specified funktions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="SamlAttributeMarshal{T}" />
    public class MappingSamlAttributeMarshal<T> : SamlAttributeMarshal<T> 
    {
        private readonly Func<T, string> _toSaml;
        private readonly Func<string, T> _fromSaml;

        public MappingSamlAttributeMarshal(string name, Func<T,string> toSaml, Func<string,T> fromSaml, Uri nameFormat = null, XName xsiType = null) : base(name, nameFormat, xsiType)
        {
            _toSaml = toSaml;
            _fromSaml = fromSaml;
        }

        public override T GetValue(AttributeAdapter attributeAdapter)
        {
            var encodedString = GetValues(attributeAdapter).Single().AttributeValueElement.Value;
            return Decode(encodedString);
        }


        public override void SetValue(AttributeAdapter attributeAdapter, T value)
        {
            if (value == null)
            {
                Remove(attributeAdapter);
            }
            else
            {
                var encodedString = Encode(value);
                SetValues(attributeAdapter, new[] {new ComplexSamlAttributeValue(encodedString)});
            }
        }

        protected T Decode(string encodedString)
        {
            return _fromSaml(encodedString);
        }

        protected string Encode(T value)
        {
            return _toSaml(value);
        }
    }


}