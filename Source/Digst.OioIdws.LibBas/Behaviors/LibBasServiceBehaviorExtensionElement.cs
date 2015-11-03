using System;
using System.ServiceModel.Configuration;

namespace Digst.OioIdws.LibBas.Behaviors
{
    /// <summary>
    /// Used for making the LibBasServiceBehavior available through configuraiton
    /// </summary>
    public class LibBasServiceBehaviorExtensionElement : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new LibBasServiceBehavior();
        }

        public override Type BehaviorType { get { return typeof(LibBasServiceBehavior); } }
    }
}
