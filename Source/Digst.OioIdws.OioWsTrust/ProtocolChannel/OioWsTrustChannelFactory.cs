using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Digst.OioIdws.OioWsTrust.ProtocolChannel
{
    public class OioWsTrustChannelFactory : ChannelFactoryBase<IRequestChannel>
    {
        public StsTokenServiceConfiguration StsTokenServiceConfiguration { get; }

        private readonly IChannelFactory<IRequestChannel> _innerFactory;
        private readonly X509Certificate2 _clientCertificate;
        private readonly X509Certificate2 _stsCertificate;

        public OioWsTrustChannelFactory(IChannelFactory<IRequestChannel> innerFactory, X509Certificate2 clientCertificate, StsTokenServiceConfiguration stsTokenServiceConfiguration)
        {
            StsTokenServiceConfiguration = stsTokenServiceConfiguration ?? throw new ArgumentNullException(nameof(stsTokenServiceConfiguration));
            _innerFactory = innerFactory ?? throw new ArgumentNullException(nameof(innerFactory));
            _clientCertificate = clientCertificate ?? throw new ArgumentNullException(nameof(clientCertificate));
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

        protected override IRequestChannel OnCreateChannel(EndpointAddress address, Uri via)
        {   
            return new OioWsTrustChannel(this, _innerFactory.CreateChannel(address, via));
        }
    }
}
