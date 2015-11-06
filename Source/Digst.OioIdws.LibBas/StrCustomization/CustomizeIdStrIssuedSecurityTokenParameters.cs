using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;
using Digst.OioIdws.Common.Logging;

namespace Digst.OioIdws.LibBas.StrCustomization
{
    /// <summary>
    /// Notice that this implementation is NOT necessary from a technical point of view and can easily be removed together with <see cref="StrReferenceSaml2SecurityTokenHandler"/>
    /// It has only been done in order to follow the examples in [LIB-BAS] profile.
    /// 
    /// This class inherits the IssuedSecurityTokenParameters class which is responsible for customizing the u:Id attribute of a SecurityTokenReference element
    /// Works in conjunction with <see cref="StrReferenceSaml2SecurityTokenHandler"/>
    /// 
    /// As-is, the STR element which .Net generates will look like this:

    ///  <o:SecurityTokenReference b:TokenType="http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0" u:Id="_f23ef5f3-9efb-40f0-bf38-758d3a9589db" xmlns:b="http://docs.oasis-open.org/wss/oasis-wss-wssecurity-secext-1.1.xsd">
    ///      <o:KeyIdentifier ValueType="http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLID">_f23ef5f3-9efb-40f0-bf38-758d3a9589db</o:KeyIdentifier>
    ///  </o:SecurityTokenReference>

    /// Note that it uses the assertion Id which is "_f23ef5f3-9efb-40f0-bf38-758d3a9589db" for both KeyIdentifier and SecurityTokenReference:Id.
    /// While this is perfectly fine, Liberty Basic SOAP binding spec's message sample uses a different id value for SecurityTokenReference:Id.
    /// Thus, this custom class is used to customize that Id as a proof that this sample can generate a message that is identical to Liberty's sample message
    /// 
    /// Thanks to SafeWhere/Kombit for this solution. Taken from https://github.com/Safewhere/kombit-common
    /// </summary>
    public class CustomizeIdStrIssuedSecurityTokenParameters : IssuedSecurityTokenParameters
    {
        /// <summary>
        ///     Instantiates an object of type CustomizeIdStrIssuedSecurityTokenParameters
        /// </summary>
        /// <param name="tokenType">Token type</param>
        public CustomizeIdStrIssuedSecurityTokenParameters(string tokenType) : base(tokenType)
        {
        }

        /// <summary>
        ///     Instantiates an object of type CustomizeIdStrIssuedSecurityTokenParameters from an existing
        ///     CustomizeIdStrIssuedSecurityTokenParameters object.
        /// </summary>
        /// <param name="other">An existing CustomizeIdStrIssuedSecurityTokenParameters object</param>
        public CustomizeIdStrIssuedSecurityTokenParameters(CustomizeIdStrIssuedSecurityTokenParameters other)
            : base(other)
        {
        }

        /// <summary>
        ///     Clones an object of this class
        /// </summary>
        /// <returns>The cloned object</returns>
        protected override SecurityTokenParameters CloneCore()
        {
            return new CustomizeIdStrIssuedSecurityTokenParameters(this);
        }

        /// <summary>
        ///     Creates a SecurityKeyIdentifierClause object which will be serialized to the request message as a
        ///     SecurityTokenReference node
        ///     Use of this class is optional, but when it is used, it must be used in conjunction with the
        ///     Kombit.Samples.Consumer.StrReferenceSaml2SecurityTokenHandler handler.
        /// </summary>
        /// <param name="token">The involved security token</param>
        /// <param name="referenceStyle">Reference style</param>
        /// <returns>An object of type SecurityKeyIdentifierClause</returns>
        protected override SecurityKeyIdentifierClause CreateKeyIdentifierClause(SecurityToken token,
            SecurityTokenReferenceStyle referenceStyle)
        {
            const string referenceId = "_str";
            Logger.Instance.Trace(string.Format("Modifying ID on STR  on request to WSP with prefix '{0}'.", referenceId));
            var clause = base.CreateKeyIdentifierClause(token, referenceStyle);
            if (clause.Id != null)
            {
                if (!clause.Id.StartsWith(referenceId))
                {
                    var temp = referenceId + clause.Id;
                    Logger.Instance.Trace(string.Format("ID on STR updated from '{0}' to '{1}'.", clause.Id, temp));
                    clause.Id = temp;
                }
                else
                {
                    Logger.Instance.Trace(string.Format("ID on STR not updated as it was already modified. ID was '{0}'.", clause.Id));
                }
            }
            else
            {
                Logger.Instance.Trace("Key identifier clause on response from STS was null. Modifying skipped.");
            }

            return clause;
        }
    }
}