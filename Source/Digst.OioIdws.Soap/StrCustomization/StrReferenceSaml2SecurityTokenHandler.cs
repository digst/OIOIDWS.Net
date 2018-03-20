using System;
using System.IdentityModel.Tokens;
using System.Xml;
using Digst.OioIdws.Common.Logging;

namespace Digst.OioIdws.Soap.StrCustomization
{
    /// <summary>
    /// Notice that this implementation is NOT necessary from a technical point of view and can easily be removed together with <see cref="CustomizedIssuedSecurityTokenParameters"/>
    /// It has only been done in order to follow the examples in [OIO IDWS SOAP 1.1] profile.
    ///  
    /// A custom Saml2SecurityTokenHandler which can write a SecurityTokenReference whose SecurityTokenReference:Id is different from the KeyIdentifier
    /// Works in conjunction with <see cref="CustomizedIssuedSecurityTokenParameters"/>
    /// 
    ///  // Default WCF:
    ///
    ///  <o:SecurityTokenReference b:TokenType="http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0" u:Id="_f23ef5f3-9efb-40f0-bf38-758d3a9589db" xmlns:b="http://docs.oasis-open.org/wss/oasis-wss-wssecurity-secext-1.1.xsd">
    ///      <o:KeyIdentifier ValueType="http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLID">_f23ef5f3-9efb-40f0-bf38-758d3a9589db</o:KeyIdentifier>
    ///  </o:SecurityTokenReference>
    ///
    /// This handler produces:
    ///
    ///  <o:SecurityTokenReference b:TokenType="http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0" u:Id="_str_f23ef5f3-9efb-40f0-bf38-758d3a9589db" xmlns:b="http://docs.oasis-open.org/wss/oasis-wss-wssecurity-secext-1.1.xsd">
    ///      <o:KeyIdentifier ValueType="http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLID">_f23ef5f3-9efb-40f0-bf38-758d3a9589db</o:KeyIdentifier>
    ///  </o:SecurityTokenReference>
    ///
    /// Note the difference between the two u:Id attributes.
    ///
    /// Thanks to SafeWhere/Kombit for this solution. Taken from https://github.com/Safewhere/kombit-common
    /// </summary>
    public class StrReferenceSaml2SecurityTokenHandler : Saml2SecurityTokenHandler
    {
        /// <summary>
        ///     Writes a SecurityTokenReference element. This handler is only required when the
        ///     <see cref="CustomizedIssuedSecurityTokenParameters" /> is used.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="securityKeyIdentifierClause"></param>
        public override void WriteKeyIdentifierClause(XmlWriter writer,
            SecurityKeyIdentifierClause securityKeyIdentifierClause)
        {
            Logger.Instance.Trace("Writing STR");
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (securityKeyIdentifierClause == null)
            {
                throw new ArgumentNullException("securityKeyIdentifierClause");
            }


            const string referenceId = "_str";
            if (!securityKeyIdentifierClause.Id.StartsWith(referenceId))
            {
                Logger.Instance.Trace(string.Format("Writing normal WCF STR because ID was not prefixed with '{0}'. ID was '{1}'.", referenceId, securityKeyIdentifierClause.Id));
                base.WriteKeyIdentifierClause(writer, securityKeyIdentifierClause);
                return;
            }

            Logger.Instance.Trace(string.Format("Writing custom STR because ID was prefixed with '{0}'. ID was '{1}'.", referenceId, securityKeyIdentifierClause.Id));

            var samlClause = securityKeyIdentifierClause as Saml2AssertionKeyIdentifierClause;

            // <wsse:SecurityTokenReference>
            writer.WriteStartElement("SecurityTokenReference",
                "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");

            // @wsse11:TokenType
            writer.WriteAttributeString("TokenType",
                "http://docs.oasis-open.org/wss/oasis-wss-wssecurity-secext-1.1.xsd",
                "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0");

            // <wsse:KeyIdentifier>
            writer.WriteStartElement("KeyIdentifier",
                "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");

            // @ValueType
            writer.WriteAttributeString("ValueType",
                "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLID");

            // ID is the string content
            writer.WriteString(samlClause.Id.Replace(referenceId, string.Empty));

            // </wsse:KeyIdentifier>
            writer.WriteEndElement();

            // </wsse:SecurityTokenReference>
            writer.WriteEndElement();
        }
    }
}