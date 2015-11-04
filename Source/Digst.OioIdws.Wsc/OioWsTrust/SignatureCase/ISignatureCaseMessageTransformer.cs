using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;

namespace Digst.OioIdws.Wsc.OioWsTrust.SignatureCase
{
    /// <summary>
    /// Transforms the messages to be compliant with the Signature case scenario of the specification [NEMLOGIN-STSRULES]
    /// </summary>
    public interface ISignatureCaseMessageTransformer
    {
        /// <summary>
        /// Modifies the message to be compliant with the STS needs
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="clientCertificate">The client certificate</param>
        void ModifyMessageAccordingToStsNeeds(ref Message request, X509Certificate2 clientCertificate);

        /// <summary>
        /// Modifies the message to be compliant with the WS Trust needs
        /// </summary>
        /// <param name="response">The response</param>
        /// <param name="stsCertificate">The certificate of the STS</param>
        void ModifyMessageAccordingToWsTrust(ref Message response, X509Certificate2 stsCertificate);
    }
}
