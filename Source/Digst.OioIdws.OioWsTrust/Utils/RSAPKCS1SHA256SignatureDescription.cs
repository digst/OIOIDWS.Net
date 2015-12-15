using System.Security.Cryptography;

namespace Digst.OioIdws.OioWsTrust.Utils
{
    /// <summary>
    /// Adds SHA256 support to the .NET SignedXml class
    /// SignatureDescription impl for http://www.w3.org/2001/04/xmldsig-more#rsa-sha256
    /// </summary>
    public class RsaPkcs1Sha256SignatureDescription : SignatureDescription
    {
        // SHA256 URL Identifiers
        public const string XmlDsigMoreRsaSha256Url = @"http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"; // Signature algoritm
        public const string XmlEncSha256Url = @"http://www.w3.org/2001/04/xmlenc#sha256"; // Digest algorithm

        /// <summary>
        /// Registers the http://www.w3.org/2001/04/xmldsig-more#rsa-sha256 algorithm
        /// with the .NET CrytoConfig registry. This needs to be called once per
        /// appdomain before attempting to validate SHA256 signatures.
        /// </summary>
        public static void Register()
        {
            CryptoConfig.AddAlgorithm(
                typeof(RsaPkcs1Sha256SignatureDescription), XmlDsigMoreRsaSha256Url);
        }

        /// <summary>
        /// .NET calls this parameterless ctor
        /// </summary>
        public RsaPkcs1Sha256SignatureDescription()
        {
            KeyAlgorithm = "System.Security.Cryptography.RSACryptoServiceProvider";
            DigestAlgorithm = "System.Security.Cryptography.SHA256Managed";
            FormatterAlgorithm = "System.Security.Cryptography.RSAPKCS1SignatureFormatter";
            DeformatterAlgorithm = "System.Security.Cryptography.RSAPKCS1SignatureDeformatter";
        }

        public override AsymmetricSignatureDeformatter CreateDeformatter(AsymmetricAlgorithm key)
        {
            var asymmetricSignatureDeformatter =
                (AsymmetricSignatureDeformatter)CryptoConfig.CreateFromName(DeformatterAlgorithm);
            asymmetricSignatureDeformatter.SetKey(key);
            asymmetricSignatureDeformatter.SetHashAlgorithm("SHA256");
            return asymmetricSignatureDeformatter;
        }

        public override AsymmetricSignatureFormatter CreateFormatter(AsymmetricAlgorithm key)
        {
            var asymmetricSignatureFormatter =
                (AsymmetricSignatureFormatter)CryptoConfig.CreateFromName(FormatterAlgorithm);
            asymmetricSignatureFormatter.SetKey(key);
            asymmetricSignatureFormatter.SetHashAlgorithm("SHA256");
            return asymmetricSignatureFormatter;
        }
    }
}