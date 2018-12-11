using System.Xml.Serialization;

namespace Digst.OioIdws.Healthcare.Common
{
    /// <summary>
    /// This class is used to encode relevant subject roles, as defined by [IHE-XUA] volume 2b section 3.40.4.1.
    /// 
    /// SAML name: urn:oasis:names:tc:xacml:2.0:subject:role
    /// </summary>
    /// <seealso cref="System.Xml.Serialization.IXmlSerializable" />
    [XmlRoot("Role", Namespace = "urn:hl7-org:v3", IsNullable = false)]
    public class SubjectRole : Hl7CodedElement
    {
        /// <summary>
        /// Constructor which supports XML serialization
        /// </summary>
        protected SubjectRole()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubjectRole"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="codeSystem">The code system.</param>
        /// <param name="codeSystemName">Name of the code system.</param>
        /// <param name="displayName">The display name.</param>
        public SubjectRole(string code, string codeSystem, string codeSystemName, string displayName) : base(code, codeSystem, codeSystemName, displayName)
        {
        }
    }
}