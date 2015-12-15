using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Digst.OioIdws.OioWsTrust.SignatureCase
{
    public class SignatureCaseBindingElement : BindingElement
    {
        private readonly X509Certificate2 _stsCertificate;

        public override BindingElement Clone()
        {
            return new SignatureCaseBindingElement(_stsCertificate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stsCertificate">The certificate used for validating the signature in the response from STS</param>
        public SignatureCaseBindingElement(X509Certificate2 stsCertificate)
        {
            if (stsCertificate == null) throw new ArgumentNullException("stsCertificate");
            _stsCertificate = stsCertificate;
        }

        public override T GetProperty<T>(BindingContext context)
        {
            // GetProperty<T>(BindingContext context) of SignatureCaseBindingElement must match GetProperty<T>() of SignatureCaseChannelFactory. 
            // In runtime, Wcf verifies that the security capabilities the factory claims it can support are also supported by the actual channel.
            // SignatureCaseChannelFactory uses the default implementation of GetProperty<T>() which returns null and this method returns an instance of type ISecurityCapabilities (created by HttpsTransportBindingElement). This makes the WCF throw an exception.
            // Either side could be changed to match the other but setting null here has been chosen because SecurityCapabilities is internal to the .Net Framework. The alternative would have been to copy/paste SecurityCapabilities and create my own implementation of ISecurityCapabilities and override GetProperty<T>() in SignatureCaseChannelFactory.
            // Downside of this is that above protocol channels are not able to read the ISecurityCapabilities.
            if (typeof(T) == typeof(ISecurityCapabilities))
                return null; 

            return context.GetInnerProperty<T>();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            // Return true if it is a request channel and the rest of the call stack supports this type of channel.
            return typeof (TChannel) == typeof (IRequestChannel) && context.CanBuildInnerChannelFactory<TChannel>();
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            var clientCredentials = context.BindingParameters.OfType<ClientCredentials>().SingleOrDefault();
            if(clientCredentials == null || clientCredentials.ClientCertificate == null || clientCredentials.ClientCertificate.Certificate == null)
                throw new InvalidOperationException("No Client certificate was configured.");

            var innerFactory = context.BuildInnerChannelFactory<IRequestChannel>();
            var factory = new SignatureCaseChannelFactory(innerFactory, clientCredentials.ClientCertificate.Certificate, _stsCertificate);
            return (IChannelFactory<TChannel>) factory;
        }
    }
}
