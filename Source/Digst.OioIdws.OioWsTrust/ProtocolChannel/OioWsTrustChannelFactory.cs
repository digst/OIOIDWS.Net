using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Digst.OioIdws.OioWsTrust.ProtocolChannel
{
    public class OioWsTrustChannelFactory : ChannelFactoryBase<IRequestChannel>
    {
        private readonly IChannelFactory<IRequestChannel> _innerFactory;
        private readonly X509Certificate2 _clientCertificate;
        private readonly X509Certificate2 _stsCertificate;
        private readonly IOioWsTrustMessageTransformer _msgTransformer;

        public OioWsTrustChannelFactory(IChannelFactory<IRequestChannel> innerFactory, X509Certificate2 clientCertificate, X509Certificate2 stsCertificate, IOioWsTrustMessageTransformer msgTransformer)
        {
            if (innerFactory == null) throw new ArgumentNullException("innerFactory");
            if (clientCertificate == null) throw new ArgumentNullException("clientCertificate");
            if (stsCertificate == null) throw new ArgumentNullException("stsCertificate");
            _innerFactory = innerFactory;
            _clientCertificate = clientCertificate;
            _stsCertificate = stsCertificate;
            _msgTransformer = msgTransformer;
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
            var channel = new OioWsTrustChannel(this, _innerFactory.CreateChannel(address, via), _msgTransformer);
            return channel;
        }
    }
}
