using Digst.OioIdws.SamlAttributes;
using Digst.OioIdws.SamlAttributes.AttributeAdapters;

namespace Digst.OioIdws.TestDoubles
{
    public interface IAttributeProvider
    {
        bool CanProvideAttribute(string name);

        void ProvideAttribute(string name, AttributeAdapter attributeAdapter);
    }
}