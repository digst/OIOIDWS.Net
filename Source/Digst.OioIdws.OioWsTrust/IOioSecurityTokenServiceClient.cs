using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;

namespace Digst.OioIdws.OioWsTrust
{
    /// <summary>
    /// A client for a security token service 
    /// </summary>
    /// <remarks>
    /// IdP: Identity Provider
    /// STS: Security Token Service.
    /// AUT: AUthentication Token.
    ///     Authentication tokens (AUT) are tokens issued by the employee MOCES certificate (signed using the MOCES certificate).
    ///     An AUT allows authentication to take place decoupled from the STS client, e.g. in a thick client of a
    ///     client-server application. The MOCES certificate then only needs to be available on the client. The server will
    ///     handle AUTs issued by the client.
    ///     AUTs are typically intended to be used by a security token service (STS) which can exchange it for a bootstrap token (BST)
    /// BST: Bootstrap Token.
    ///     BSTs are issued to a requester by an IdP (when a user authenticates) or by an STS (exchanged for an AUT).
    ///     The intended audience of a BST is the STS
    /// IDT: Identity token. IDTs are issued by an STS, requested by a WSC and issued to a specific WSP (the audience).
    /// </remarks>
    public interface IOioSecurityTokenServiceClient
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
        SecurityToken GetIdentityTokenFromBootstrapToken(SecurityToken bootstrapToken, string wspIdentifier, KeyType keyType, RequestClaimCollection claims=null);

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
        SecurityToken GetServiceToken(string wspIdentifier, RequestClaimCollection claims=null);

        /// <summary>
        /// Gets the bootstrap token from authentication token.
        /// </summary>
        /// <param name="authenticationToken">An authentication token (AUT). An AUT is a token which is signed by the employee certificate</param>
        /// <returns>A bootstrap token (BST).</returns>
        SecurityToken GetBootstrapTokenFromAuthenticationToken(SecurityToken authenticationToken);
    }
}