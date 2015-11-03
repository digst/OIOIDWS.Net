using System.IdentityModel.Tokens;
using System.Xml;

namespace Digst.OioIdws.Wsp.DecryptedAssertionSupport
{
    /// <summary>
    ///     A custom saml2 security token handler which will support references made to assertions that has been encrypted.
    /// </summary>
    public class DecryptedSaml2SecurityTokenHandler : Saml2SecurityTokenHandler
    {
        /// <summary>
        ///     Reads a token with support for references to assertions that has been encrypted. 
        /// </summary>
        /// <param name="reader">An xml reader that reads the token from a stream</param>
        /// <returns>An instance of type <see cref="DecryptedSaml2SecurityToken"/></returns>
        public override SecurityToken ReadToken(XmlReader reader)
        {
            var saml2SecurityToken = base.ReadToken(reader) as Saml2SecurityToken;

            return new DecryptedSaml2SecurityToken(saml2SecurityToken);
        }
    }
}