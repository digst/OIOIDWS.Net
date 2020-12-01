using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Digst.OioIdws.OioWsTrust.ProtocolChannel
{
    public class OioWsTrustChannelFactory : ChannelFactoryBase<IRequestChannel>
    {
        /// <summary>
        /// STS configuration parameters
        /// </summary>
        public StsTokenServiceConfiguration StsTokenServiceConfiguration { get; }

        private readonly IChannelFactory<IRequestChannel> _innerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="OioWsTrustChannelFactory"/> class.
        /// </summary>
        /// <param name="innerFactory">The inner factory.</param>
        /// <param name="stsTokenServiceConfiguration">The STS token service configuration.</param>
        public OioWsTrustChannelFactory(IChannelFactory<IRequestChannel> innerFactory, StsTokenServiceConfiguration stsTokenServiceConfiguration)
        {
            _innerFactory = innerFactory ?? throw new ArgumentNullException(nameof(innerFactory));
            StsTokenServiceConfiguration = stsTokenServiceConfiguration ?? throw new ArgumentNullException(nameof(stsTokenServiceConfiguration));
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

        protected override IRequestChannel OnCreateChannel(EndpointAddress address, Uri via)
        {   
            return new OioWsTrustChannel(this, _innerFactory.CreateChannel(address, via));
        }
    }
}
