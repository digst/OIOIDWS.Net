namespace Digst.OioIdws.OioWsTrust
{

    /// <summary>
    /// The scenarios supported by NemLog-in STS
    /// </summary>
    public enum StsAuthenticationCase
    {
        /// <summary>
        /// The signature case: The request for security token (RST) must be signed
        /// using the employee FOCES certificate. The RST contains no "act as" token.
        /// </summary>
        SignatureCase = 0,

        /// <summary>
        /// The bootstrap token case: The request for security token (RST) must
        /// contain a bootstrap token obtained through the NemLog-in identity provider.
        /// </summary>
        BootstrapTokenCase = 1,

        /// <summary>
        /// The local token case: The request for security token (RST) must
        /// contain a local token from a local STS. The local STS must have
        /// been registered in advance with NemLog-in using the local STS
        /// entity ID and signing certificate.
        /// </summary>
        LocalTokenCase = 2,
    }
}