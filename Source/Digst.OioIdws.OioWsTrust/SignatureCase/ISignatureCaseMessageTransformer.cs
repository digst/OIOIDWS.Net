using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;

namespace Digst.OioIdws.OioWsTrust.SignatureCase
{
    /// <summary>
    /// Transforms the messages to be compliant with the Signature case scenario of the specification [NEMLOGIN-STSRULES]
    /// </summary>
    public interface ISignatureCaseMessageTransformer
    {
        /// <summary>
        /// Transforms a default WCF/WIF RST request into a proprietary request that NemLog-in STS understands.
        /// </summary>
        /// <param name="request">The default RST request from WCF/WIF</param>
        /// <param name="clientCertificate">The certificate of the WSC</param>
        void ModifyMessageAccordingToStsNeeds(ref Message request, X509Certificate2 clientCertificate);

        /// <summary>
        /// Transforms a proprietary NemLog-in RSTR response into a standard WCF/WIF RSTR response.
        /// Validating of the response must also done. Thus, signature validation, replay attack validation and expiry time validation.
        /// </summary>
        /// <param name="response">The response from NemLog-in STS</param>
        /// <param name="stsCertificate">The STS certificate</param>
        void ModifyMessageAccordingToWsTrust(ref Message response, X509Certificate2 stsCertificate);
    }
}
