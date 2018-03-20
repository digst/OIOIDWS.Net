using System.ServiceModel.Configuration;

namespace Digst.OioIdws.Soap.Bindings
{
    /// <summary>
    /// Class used for making <see cref="SoapBinding"/> available through configuration.
    /// </summary>
    public class SoapBindingCollectionElement : StandardBindingCollectionElement<SoapBinding, SoapBindingElement>
    {
    }
}
