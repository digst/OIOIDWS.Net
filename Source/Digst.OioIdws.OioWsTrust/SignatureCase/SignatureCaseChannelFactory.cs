using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Digst.OioIdws.OioWsTrust.SignatureCase
{
    public class SignatureCaseChannelFactory : ChannelFactoryBase<IRequestChannel>
    {
        private readonly IChannelFactory<IRequestChannel> _innerFactory;
        private readonly X509Certificate2 _clientCertificate;
        private readonly X509Certificate2 _stsCertificate;

        public SignatureCaseChannelFactory(IChannelFactory<IRequestChannel> innerFactory, X509Certificate2 clientCertificate, X509Certificate2 stsCertificate)
        {
            if (innerFactory == null) throw new ArgumentNullException("innerFactory");
            if (clientCertificate == null) throw new ArgumentNullException("clientCertificate");
            if (stsCertificate == null) throw new ArgumentNullException("stsCertificate");
            _innerFactory = innerFactory;
            _clientCertificate = clientCertificate;
            _stsCertificate = stsCertificate;
        }

        #region Members which simply delegate to the inner factory
        protected override void OnOpen(TimeSpan timeout)
        {
            _innerFactory.Open(timeout);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerFactory.BeginOpen(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            _innerFactory.EndOpen(result);
        }
        #endregion

        public X509Certificate2 ClientCertificate
        {
            get { return _clientCertificate; }
        }

        public X509Certificate2 StsCertificate
        {
            get { return _stsCertificate; }
        }

        protected override IRequestChannel OnCreateChannel(EndpointAddress address, Uri via)
        {   
            return new SignatureCaseChannel(this, _innerFactory.CreateChannel(address, via));
        }
    }
}
