using System.IdentityModel.Tokens;
using Digst.OioIdws.Common.Constants;

namespace Digst.OioIdws.Wsp.DecryptedAssertionSupport
{
    /// <summary>
    /// This class makes it possible to support encrypted SAML2 assertions where the reference to the proof key is the ID of the encrypted assertion.
    /// By auto accepting all proof key references with the name "encryptedassertion" WIF uses the only included token (the client certificate) from the decrypted assertion as proof key.
    /// </summary>
    public class DecryptedSaml2SecurityToken : Saml2SecurityToken
    {
        public DecryptedSaml2SecurityToken(Saml2SecurityToken saml2SecurityToken)
            : base(saml2SecurityToken.Assertion, saml2SecurityToken.SecurityKeys, saml2SecurityToken.IssuerToken)
        {
        }

        /// <summary>
        /// <see cref="Saml2SecurityToken.MatchesKeyIdentifierClause"/>
        /// </summary>
        /// <param name="keyIdentifierClause"></param>
        /// <returns>Returns true if there is a normal match or if the id is <see cref="EncryptedAssertionId"/></returns>
        public override bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            return base.MatchesKeyIdentifierClause(keyIdentifierClause) || OioWsTrust.EncryptedAssertionId == keyIdentifierClause.Id;
        }
    }
}