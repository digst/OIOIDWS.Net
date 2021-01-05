using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;

namespace Digst.OioIdws.OioWsTrust.ProtocolChannel
{
    /// <summary>
    /// Transforms the messages to be compliant with the Signature case scenario of the specification [NEMLOGIN-STSRULES]
    /// </summary>
    public interface IOioWsTrustMessageTransformer
    {
        /// <summary>
        /// Transforms a default WCF/WIF RST request into a proprietary request that NemLog-in STS understands.
        /// </summary>
        /// <param name="request">The default RST request from WCF/WIF</param>
        void ModifyMessageAccordingToStsNeeds(ref Message request);

        /// <summary>
        /// Transforms a proprietary NemLog-in RSTR response into a standard WCF/WIF RSTR response.
        /// Validating of the response must also done. Thus, signature validation, replay attack validation and expiry time validation.
        /// </summary>
        /// <param name="response">The response from NemLog-in STS</param>
        void ModifyMessageAccordingToWsTrust(ref Message response);
    }
}
