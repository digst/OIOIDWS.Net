using System.ServiceModel.Channels;
using System.Xml;
using Digst.OioIdws.Common;
using Digst.OioIdws.Common.Constants;

namespace Digst.OioIdws.LibBas.Headers
{
    /// <summary>
    /// A class that represents the liberty framework message header.
    /// </summary>
    [System.Serializable]
    public class LibertyFrameworkHeader : MessageHeader
    {
        public LibertyFrameworkHeader()
        {
            Name = Common.Constants.LibBas.HeaderName;
            Namespace = Common.Constants.LibBas.HeaderNameSpace;
            MustUnderstand = true; // Is set to true in the examples in [LIB-BAS]. However, not stated as either optional or mandatory.
        }

        public override bool MustUnderstand { get; }

        public override string Name { get; }

        public override string Namespace { get; }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteAttributeString(Common.Constants.LibBas.ProfileName, Common.Constants.LibBas.ProfileNameSpace, Common.Constants.LibBas.ProfileValue);
            writer.WriteAttributeString(Common.Constants.LibBas.VersionName, Common.Constants.LibBas.VersionValue);
        }
    }
}
