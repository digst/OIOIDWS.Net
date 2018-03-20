using System;
using System.ServiceModel.Configuration;

namespace Digst.OioIdws.Soap.Behaviors
{
    /// <summary>
    /// Used for making the SoapClientBehavior available through configuraiton
    /// </summary>
    public class SoapClientBehaviorExtensionElement : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new SoapClientBehavior();
        }

        public override Type BehaviorType { get { return typeof(SoapClientBehavior); } }
    }
}
