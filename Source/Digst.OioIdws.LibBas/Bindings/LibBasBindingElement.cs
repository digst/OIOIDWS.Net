using System;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace Digst.OioIdws.LibBas.Bindings
{
    /// <summary>
    /// Class used for making configuration possible of the <see cref="LibBasBinding"/>
    /// </summary>
    public class LibBasBindingElement : StandardBindingElement
    {
        protected override void OnApplyConfiguration(Binding binding)
        {
            var libBasBinding = (LibBasBinding) binding;
            libBasBinding.UseHttps = UseHttps;
            libBasBinding.MaxReceivedMessageSize = MaxReceivedMessageSize;
        }

        protected override Type BindingElementType
        {
            get { return typeof (LibBasBinding); }
        }

        /// <summary>
        /// Specifies max size of message recieved in bytes. If not set, default value on <see cref="TransportBindingElement.MaxReceivedMessageSize"/> are used.
        /// </summary>
        [ConfigurationProperty("maxReceivedMessageSize", IsRequired = false, DefaultValue = null)]
        public int? MaxReceivedMessageSize
        {
            get
            {
                return (int?)this["maxReceivedMessageSize"];
            }
            set
            {
                this["maxReceivedMessageSize"] = value;
            }
        }

        /// <summary>
        /// Specifies whether transport layer security is needed in the http communication.
        /// </summary>
        [ConfigurationProperty("useHttps", IsRequired = false, DefaultValue = true)]
        public bool UseHttps
        {
            get
            {
                return (bool)this["useHttps"];
            }
            set
            {
                this["useHttps"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                var properties = base.Properties;
                properties.Add(new ConfigurationProperty("useHttps", typeof (bool), true));
                properties.Add(new ConfigurationProperty("maxReceivedMessageSize", typeof (int?), null));
                return properties;
            }
        }

        protected override void InitializeFrom(Binding binding)
        {
            base.InitializeFrom(binding);
            var libBasBinding = (LibBasBinding)binding;
            UseHttps = libBasBinding.UseHttps;
            MaxReceivedMessageSize = libBasBinding.MaxReceivedMessageSize;
        }
    }
}