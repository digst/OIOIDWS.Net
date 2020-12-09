using System.IdentityModel.Tokens;

namespace Digst.OioIdws.OioWsTrust
{
    /// <summary>
    /// Used for retrieving a token from NemLog-in STS. The token can then be used to call WSP's (Web Service Providers).
    /// </summary>
    public interface IStsTokenService
    {
        /// <summary>
        /// This method is used in the signature case scenario where a WSC wants to fetch a token representing the WSC itself.
        /// The STS endpoint, client certificate and WSP endpointID are configured in the configuration file.
        /// This method is thread safe.
        /// </summary>
        /// <returns>Returns a token.</returns>
        SecurityToken GetToken();

        SecurityToken GetToken(StsAuthenticationCase stsAuthenticationCase, SecurityToken authenticationToken);

        SecurityToken GetTokenWithBootstrapToken(SecurityToken bootstrapToken);

        SecurityToken GetTokenWithLocalToken(SecurityToken bootstrapToken);
    }
}