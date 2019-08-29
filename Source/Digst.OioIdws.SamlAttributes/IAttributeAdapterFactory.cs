using Digst.OioIdws.SamlAttributes.AttributeAdapters;

namespace Digst.OioIdws.SamlAttributes
{


    /// <summary>
    /// Enables creation of <see cref="AttributeAdapter"/> derivatives.
    /// </summary>
    public interface IAttributeAdapterFactory
    {

        /// <summary>
        /// Creates a new attribute manager
        /// </summary>
        /// <returns></returns>
        AttributeAdapter Create();

    }
}