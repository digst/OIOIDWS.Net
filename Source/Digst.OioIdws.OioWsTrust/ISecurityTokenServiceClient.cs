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
        /// This method is used in the signature case scenario where a WSC wants to fetch a token representing the WSC itself, i.e. *not* an identity token.
        /// The STS endpoint, client certificate are configured in the configuration file.
        /// This method is thread safe.
        /// </summary>
        /// <param name="serviceIdentifier">The URI identifying the service (audience)</param>
        /// <param name="keyType"></param>
        /// <returns>Returns a token.</returns>
        SecurityToken GetServiceToken(string serviceIdentifier, KeyType keyType);

        /// <summary>
        /// Uses the STS to retrieve a identity token (IDT)/service token from a bootstrap (BST) token.
        /// The BST may have been retrieved from an identity provider (IdP) or through a previous call to the STS using the <see cref="GetBootstrapTokenFromAuthenticationToken"/> method.
        /// </summary>
        /// <remarks>
        /// This method is used in the bootstrap case sceanrio where a WSC in context of a user wants to fetch a token representing the WSC and a user.
        /// The STS endpoint, client certificate are configured in the configuration file.
        /// This method is thread safe.</remarks>
        /// <param name="bootstrapToken">The token representing a user. It is retrieved through the attribute with name "urn:liberty:disco:2006-08:DiscoveryEPR" from the SAML assertion from NemLog-in IdP. A null value results in the same as calling <see cref="GetServiceToken"/></param>
        /// <param name="serviceIdentifier">The URI identifying the service. Also called "audience".</param>
        /// <param name="keyType"></param>
        /// <returns>Returns a token.</returns>
        SecurityToken GetIdentityTokenFromBootstrapToken(SecurityToken bootstrapToken, string serviceIdentifier, KeyType keyType);


        /// <summary>
        /// Retrieve a bootstrap token from the STS given an authentification (AUT) token.
        /// An AUT token is a token signed using by an employee certificate (MOCES) that is trusted by the STS.
        /// </summary>
        /// <remarks>
        /// This method is used in the client-server (frontend/backend) scenario - also referred to as "legacy" mode.
        /// In this scenario, the frontend(client) issues an AUT token locally on the employee workstation (in the frontend application - typically a Windows Forms or WPF application)
        /// and transports it to the backend (server) by some proprietary means. The backend uses the STS to exchange the AUT token for a bootstrap (BST) token.
        /// When the backend needs to invoke a given web service at a web service provider (WSP), it will use the STS to exchange the BST to an identity (IDT) token for the specific WSP.
        /// </remarks>
        /// <param name="authenticationToken">Token which represents an employee authentication. Typically a token signed by a MOCES certificate.</param>
        /// <returns>An Identity(IDT)/service token which can be used to invoke a service at a specific WSP</returns>
        SecurityToken GetBootstrapTokenFromAuthenticationToken(SecurityToken authenticationToken);


    }
}