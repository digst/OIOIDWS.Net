using System;
using System.Linq;
using Digst.OioIdws.SamlAttributes;
using Digst.OioIdws.SamlAttributes.AttributeAdapters;

namespace Digst.OioIdws.TestDoubles
{
    public class CompoundAttributeProvider : IAttributeProvider
    {
        private readonly IAttributeProvider[] _innerAttributeProviders;

        public CompoundAttributeProvider(params IAttributeProvider[] innerAttributeProviders)
        {
            _innerAttributeProviders = innerAttributeProviders;
        }

        public bool CanProvideAttribute(string name)
        {
            return _innerAttributeProviders.Any(x => x.CanProvideAttribute(name));
        }

        public void ProvideAttribute(string name, AttributeAdapter attributeAdapter)
        {
            var provider = _innerAttributeProviders.FirstOrDefault(x => x.CanProvideAttribute(name));
            if (provider == null) throw new ArgumentOutOfRangeException(nameof(name), $"Unknown attribute name {name}");
            provider.ProvideAttribute(name, attributeAdapter);
        }
    }
}