using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Digst.OioIdws.OioWsTrust.SignatureCase
{
    public class SignatureCaseChannel : ChannelBase, IRequestChannel
    {
        private readonly SignatureCaseChannelFactory _channelManager;
        private readonly IRequestChannel _innerChannel;

        public SignatureCaseChannel(SignatureCaseChannelFactory channelManager, IRequestChannel innerChannel)
            : base(channelManager)
        {
            if (channelManager == null) throw new ArgumentNullException("channelManager");
            if (innerChannel == null) throw new ArgumentNullException("innerChannel");
            _channelManager = channelManager;
            _innerChannel = innerChannel;
        }

        public Message Request(Message message)
        {
            return Request(message, DefaultSendTimeout);
        }

        public Message Request(Message message, TimeSpan timeout)
        {
            var signatureCaseMessageTransformer = new SignatureCaseMessageTransformer();
            signatureCaseMessageTransformer.ModifyMessageAccordingToStsNeeds(ref message, _channelManager.ClientCertificate);
            var respsonse = _innerChannel.Request(message, timeout);
            
            signatureCaseMessageTransformer.ModifyMessageAccordingToWsTrust(ref respsonse, _channelManager.StsCertificate);
            return respsonse;
        }

        #region Members which simply delegate to the inner channel
        public IAsyncResult BeginRequest(Message message, AsyncCallback callback, object state)
        {
            return _innerChannel.BeginRequest(message, callback, state);
        }

        public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerChannel.BeginRequest(message, timeout, callback, state);
        }

        public Message EndRequest(IAsyncResult result)
        {
            return _innerChannel.EndRequest(result);
        }
        protected override void OnAbort()
        {
            _innerChannel.Abort();
        }

        protected override void OnClose(TimeSpan timeout)
        {
            _innerChannel.Close(timeout);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            _innerChannel.EndClose(result);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerChannel.BeginClose(timeout, callback, state);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            _innerChannel.Open(timeout);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerChannel.BeginOpen(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            _innerChannel.EndOpen(result);
        }

        public EndpointAddress RemoteAddress
        {
            get { return _innerChannel.RemoteAddress; } 
        }

        public Uri Via
        {
            get { return _innerChannel.Via; }
        }
        #endregion
    }
}
