using System;
using System.Collections.Generic;
using Digst.OioIdws.SamlAttributes;
using Digst.OioIdws.SamlAttributes.AttributeMarshals;

namespace Digst.OioIdws.TestDoubles.Test
{
    public class ConstantAttributeProvider : IAttributeProvider
    {
        private IDictionary<string,Action<AttributeAdapter>> _setters = new Dictionary<string, Action<AttributeAdapter>>();

        public void Add<T>(SamlAttributeMarshal<T> marshal, T value)
        {
            _setters.Add(marshal.Name, mgr => mgr.SetValue(marshal, value));
        }

        public bool CanProvideAttribute(string name)
        {
            return _setters.ContainsKey(name);
        }

        public void ProvideAttribute(string name, AttributeAdapter attributeAdapter)
        {
            _setters[name](attributeAdapter);
        }
    }
}