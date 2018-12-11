using System.IdentityModel.Tokens;
using System.IO;
using System.Text;
using System.Xml;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.SecurityTokens
{
    /// <summary>
    /// Extension methods for working with security tokens
    /// </summary>
    public static class SecurityTokenExtensions
    {

        /// <summary>
        /// Converts a Saml2SecurityToken to a GenericXmlSecurityToken
        /// </summary>
        public static GenericXmlSecurityToken ToGenericXmlSecurityToken(this Saml2SecurityToken token)
        {
            return token.Assertion.ToGenericXmlSecurityToken();
        }

        /// <summary>
        /// Converts a Saml2Assertion to a GenericXmlSecurityToken
        /// </summary>
        public static GenericXmlSecurityToken ToGenericXmlSecurityToken(this Saml2Assertion assertion)
        {
            var handler = new ExtendedSaml2SecurityTokenHandler();
            using (var mem = new MemoryStream())
            {
                var xw = XmlWriter.Create(mem, new XmlWriterSettings() { Encoding = Encoding.UTF8 });
                handler.WriteToken(xw, new Saml2SecurityToken(assertion));
                xw.Flush();
                mem.Seek(0, SeekOrigin.Begin);
                var doc = new XmlDocument();
                doc.Load(mem);
                var xmlToken = new GenericXmlSecurityToken(doc.DocumentElement, null, assertion.Conditions.NotBefore.Value, assertion.Conditions.NotOnOrAfter.Value, new LocalIdKeyIdentifierClause(assertion.Id.Value), new LocalIdKeyIdentifierClause(assertion.Id.Value), null);
                return xmlToken;
            }
        }
    }
}