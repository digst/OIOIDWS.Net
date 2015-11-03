using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;

namespace Digst.OioIdws.Wsc.OioWsTrust.SignatureCase
{
    public interface ISignatureCaseMessageTransformer
    {
        void ModifyMessageAccordingToStsNeeds(ref Message request, X509Certificate2 clientCertificate);
        void ModifyMessageAccordingToWsTrust(ref Message response, X509Certificate2 stsCertificate);
    }
}
