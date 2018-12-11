using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;

namespace Digst.OioIdws.OioWsTrust
{
    /// <summary>
    /// A client for a security token service 
    /// </summary>
    public interface IOioSecurityTokenServiceClient
    {
        /// <summary>
        /// Gets the bootstrap token from authentication token.
        /// </summary>
        /// <param name="authenticationToken">The authentication token.</param>
        /// <returns></returns>
        SecurityToken GetBootstrapTokenFromAuthenticationToken(SecurityToken authenticationToken);

        /// <summary>
        /// Gets the identity token from bootstrap token.
        /// </summary>
        /// <param name="bootstrapToken">The bootstrap token.</param>
        /// <param name="wspIdentifier">The WSP identifier.</param>
        /// <param name="claims">The claims.</param>
        /// <param name="keyType">Type of the key.</param>
        /// <returns></returns>
        SecurityToken GetIdentityTokenFromBootstrapToken(SecurityToken bootstrapToken, string wspIdentifier, RequestClaimCollection claims, KeyType keyType);

        /// <summary>
        /// Gets the service token.
        /// </summary>
        /// <param name="wspIdentifier">The WSP identifier.</param>
        /// <param name="claims">The claims.</param>
        /// <returns></returns>
        SecurityToken GetServiceToken(string wspIdentifier, RequestClaimCollection claims);
    }
}