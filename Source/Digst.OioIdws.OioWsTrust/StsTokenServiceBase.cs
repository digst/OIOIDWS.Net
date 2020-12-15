using System.IdentityModel.Tokens;

namespace Digst.OioIdws.OioWsTrust
{

    /// <summary>
    /// Base implementation of a security token service
    /// </summary>
    /// <seealso cref="Digst.OioIdws.OioWsTrust.IStsTokenService" />
    public abstract class StsTokenServiceBase : IStsTokenService
    {

        /// <summary>
        /// This method is used in the signature case scenario where a WSC wants to fetch a token representing the WSC itself.
        /// The STS endpoint, client certificate and WSP endpointID are configured in the configuration file.
        /// This method is thread safe.
        /// </summary>
        public SecurityToken GetToken()
        {
            return GetToken(StsAuthenticationCase.SignatureCase, null);
        }

        /// <summary>
        /// Gets a token from the service
        /// </summary>
        /// <param name="stsAuthenticationCase">The STS authentication case.</param>
        /// <param name="authenticationToken">The authentication token (bootstrap or local -token).</param>
        public abstract SecurityToken GetToken(StsAuthenticationCase stsAuthenticationCase, SecurityToken authenticationToken);

        /// <summary>
        /// Gets the token with bootstrap token.
        /// </summary>
        /// <param name="bootstrapToken">The bootstrap token.</param>
        public SecurityToken GetTokenWithBootstrapToken(SecurityToken bootstrapToken)
        {
            return GetToken(StsAuthenticationCase.BootstrapTokenCase, bootstrapToken);
        }

        /// <summary>
        /// Gets the token with local token.
        /// </summary>
        /// <param name="localToken">The local token.</param>
        public SecurityToken GetTokenWithLocalToken(SecurityToken localToken)
        {
            return GetToken(StsAuthenticationCase.LocalTokenCase, localToken);
        }
    }
}