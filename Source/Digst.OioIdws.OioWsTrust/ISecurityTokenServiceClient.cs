using System;
using System.IdentityModel.Metadata;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace Digst.OioIdws.OioWsTrust
{
    /// <summary>
    /// Used for retrieving a token from NemLog-in STS. The token can then be used to call WSP's (Web Service Providers).
    /// </summary>
    public interface ISecurityTokenServiceClient
    {
        /// <summary>
        /// Exchanges a bootstrap token (BST) for an identity token (IDT)
        /// BSTs are issued by the security token service (STS) itself, or by an identity provider (IdP) trusted by the STS.
        /// BSTs represents the actual user, i.e. identification of the actual user is built into the BST.
        /// In the case of NemLog-in the BST can be provided by the the NemLog-in IdP.
        /// BSTs can not be used to invoke actual services offered by web service providers (WSPs). Instead, BST must be exchanged
        /// for an identity token which can then be used to invoke a service.
        /// This operation allows you to exchange a BST for an IDT.
        /// </summary>
        /// <param name="bootstrapToken">The bootstrap token (BST) obtained through an identity provider or by exchanging an authentication token for a bootstrap token.</param>
        /// <param name="wspIdentifier">The web service provider (WSP) identifier. The entity ID of the service the you want the token issued to.</param>
        /// <param name="keyType">Type of the key. Indicated bearer token oe </param>
        /// <param name="claims">The collection of claims that you request the security token builds
        /// into the issued service token. Pass null if you do not request any claims.
        /// Note that the STS may ignore or supplement claims.</param>
        /// <returns>An identity token (IDT) issued to the WSP for the same subject as the bootstrap token (BST)</returns>
        SecurityToken GetIdentityTokenFromBootstrapToken(SecurityToken bootstrapToken, string wspIdentifier, KeyType keyType, RequestClaimCollection claims = null);

        /// <summary>
        /// Gets a service token.
        /// A service token is used to invoke a non-identity based federated service (a non-IDWS service),
        /// i.e. a service which does not require a token specifying the subject.
        /// </summary>
        /// <param name="wspIdentifier">The web service provider (WSP) identifier. The entity ID of the service the you want the token issued to.</param>
        /// <param name="claims">The collection of claims that you request the security token builds
        /// into the issued service token. Pass null if you do not request any claims.
        /// Note that the STS may ignore or supplement claims.</param>
        /// <returns>A service token issued to the WSP.</returns>
        SecurityToken GetServiceToken(string wspIdentifier, KeyType keyType, RequestClaimCollection claims = null);

        /// <summary>
        /// Gets the bootstrap token from authentication token.
        /// </summary>
        /// <param name="authenticationToken">An authentication token (AUT). An AUT is a token which is signed by the employee certificate</param>
        /// <returns>A bootstrap token (BST).</returns>
        SecurityToken GetBootstrapTokenFromAuthenticationToken(SecurityToken authenticationToken);
    }
}