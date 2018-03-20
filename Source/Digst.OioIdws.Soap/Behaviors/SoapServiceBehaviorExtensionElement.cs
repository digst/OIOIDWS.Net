using System;
using System.ServiceModel.Configuration;

namespace Digst.OioIdws.Soap.Behaviors
{
    /// <summary>
    /// Used for making the SoapServiceBehavior available through configuraiton
    /// </summary>
    public class SoapServiceBehaviorExtensionElement : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new SoapServiceBehavior();
        }

        public override Type BehaviorType { get { return typeof(SoapServiceBehavior); } }
    }
}
