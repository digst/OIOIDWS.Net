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

        /// <summary>
        /// This method is used in the bootstrap case sceanrio where a WSC in context of a user wants to fetch a token representing the WSC and a user.
        /// The STS endpoint, client certificate and WSP endpointID are configured in the configuration file.
        /// This method is thread safe.
        /// </summary>
        /// <param name="bootstrapToken">The token representing a user. It is retrieved through the attribute with name "urn:liberty:disco:2006-08:DiscoveryEPR" from the SAML assertion from NemLog-in IdP. A null value results in the same as calling <see cref="GetToken()"/></param>
        /// <returns>Returns a token.</returns>
        SecurityToken GetTokenWithBootstrapToken(SecurityToken bootstrapToken);
    }
}