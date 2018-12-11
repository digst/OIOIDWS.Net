using Digst.OioIdws.SamlAttributes;

namespace Digst.OioIdws.TestDoubles
{
    public interface IAttributeProvider
    {
        bool CanProvideAttribute(string name);

        void ProvideAttribute(string name, AttributeAdapter attributeAdapter);
    }
}