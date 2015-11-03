using System;
using System.ServiceModel.Configuration;

namespace Digst.OioIdws.LibBas.Behaviors
{
    /// <summary>
    /// Used for making the LibBasClientBehavior available through configuraiton
    /// </summary>
    public class LibBasClientBehaviorExtensionElement : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new LibBasClientBehavior();
        }

        public override Type BehaviorType { get { return typeof(LibBasClientBehavior); } }
    }
}
