using System;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.Healthcare.Common
{
    /// <summary>
    /// A base class for values in coded systems
    /// according to IHE Revision 14.0 – July 21, 2017 (https://www.ihe.net/Technical_Framework_Archives/)
    /// </summary>
    public abstract class Hl7CodedElement : IEquatable<Hl7CodedElement>, IXmlSerializable
    {
        protected const string ns = "urn:hl7-org:v3";

        /// <summary>
        /// Constructor for support of XML serialization
        /// </summary>
        protected Hl7CodedElement()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hl7CodedElement"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="codeSystem">The code system.</param>
        /// <param name="codeSystemName">Name of the code system.</param>
        /// <param name="displayName">The display name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// code
        /// or
        /// codeSystem
        /// </exception>
        protected Hl7CodedElement(string code, string codeSystem, string codeSystemName, string displayName)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            CodeSystem = codeSystem ?? throw new ArgumentNullException(nameof(codeSystem));
            CodeSystemName = codeSystemName;
            DisplayName = displayName;
        }

        /// <summary>
        /// The code 
        /// </summary>
        [Required]
        public string Code { get; private set; }

        /// <summary>
        /// The code system *should* be set to urn:oasis:names:tc:xspa:1.0, which is also the default for a newly created object
        /// </summary>
        [Required]
        public string CodeSystem { get; private set; }

        /// <summary>
        /// Gets or sets the name of the code system.
        /// </summary>
        public string CodeSystemName { get; private set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; private set; }

        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema() => null;

        /// <inheritdoc />
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Code = reader.GetAttribute("code");
            CodeSystem = reader.GetAttribute("codeSystem");
            CodeSystemName = reader.GetAttribute("codeSystemName");
            DisplayName = reader.GetAttribute("displayName");
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {


            writer.WriteAttributeString("xsi", "type", Saml2Constants.Namespaces.XmlSchemaInstanceString, "CE");
            writer.WriteAttributeString("code", Code);
            writer.WriteAttributeString("codeSystem", CodeSystem);
            if (CodeSystemName != null) writer.WriteAttributeString("codeSystemName", CodeSystemName);
            if (DisplayName != null) writer.WriteAttributeString("displayName", DisplayName);
        }

        public bool Equals(Hl7CodedElement other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Code, other.Code) && string.Equals(CodeSystem, other.CodeSystem);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Hl7CodedElement) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Code.GetHashCode() * 397) ^ CodeSystem.GetHashCode();
            }
        }
    }
}