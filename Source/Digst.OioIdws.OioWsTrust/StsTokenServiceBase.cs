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
        /// <see cref="IStsTokenService.GetToken()"/>
        /// </summary>
        protected internal abstract SecurityToken GetToken(StsAuthenticationCase stsAuthenticationCase, SecurityToken authenticationToken);

        /// <summary>
        /// <see cref="IStsTokenService.GetTokenWithBootstrapToken()"/>
        /// </summary>
        public SecurityToken GetTokenWithBootstrapToken(SecurityToken bootstrapToken)
        {
            return GetToken(StsAuthenticationCase.BootstrapTokenCase, bootstrapToken);
        }

        /// <summary>
        /// <see cref="IStsTokenService.GetTokenWithLocalToken()"/>
        /// </summary>
        public SecurityToken GetTokenWithLocalToken(SecurityToken localToken)
        {
            return GetToken(StsAuthenticationCase.LocalTokenCase, localToken);
        }
    }
}