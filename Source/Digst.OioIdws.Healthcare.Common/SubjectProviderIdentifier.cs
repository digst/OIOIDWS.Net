using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;

namespace Digst.OioIdws.Healthcare.Common
{

    /// <summary>
    /// Implementation of the attribute for urn:ihe:iti:xua:2017:subject:provider-identifier
    /// </summary>
    [XmlRoot("id", Namespace = "urn:hl7-org:v3")]
    public class SubjectProviderIdentifier : IXmlSerializable
    {
        protected SubjectProviderIdentifier()
        {
        }

        public SubjectProviderIdentifier(string root, string extension, string assigningAuthorityName, bool? displayable=null)
        {
            Root = root ?? throw new ArgumentNullException(nameof(root));
            Extension = extension ?? throw new ArgumentNullException(nameof(extension));
            AssigningAuthorityName = assigningAuthorityName;
            Displayable = displayable;
        }

        /// <summary>
        /// An OID referencing the authority providing the identifier. Required.
        /// </summary>
        public string Root { get; set; }

        /// <summary>
        /// A unique string identifier for the provider. Required.
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// The name for the assigning authority
        /// </summary>
        public string AssigningAuthorityName { get; set; }

        /// <summary>
        /// Indicating whether the provider-id in extension is intended to be shown to human users. 
        /// </summary>
        public bool? Displayable { get; set; }


        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <inheritdoc />
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Root = reader.GetAttribute("root");

            Extension = reader.GetAttribute("extension");

            AssigningAuthorityName = reader.GetAttribute("assigningAuthorityName");

            var displayable = reader.GetAttribute("displayable");
            switch (displayable?.Trim())
            {
                case null:
                    Displayable = null;
                    break;
                case "true":
                    Displayable = true;
                    break;
                case "false":
                    Displayable = false;
                    break;
                default: throw new InvalidOperationException($@"Unable to deserialize value ""{displayable}"" ");
            }
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("xsi","type",Saml2Constants.Namespaces.XmlSchemaInstanceString, "II");

            writer.WriteAttributeString("root", Root);

            writer.WriteAttributeString("extension", Extension);

            if (AssigningAuthorityName != null)
            {
                writer.WriteAttributeString("assigningAuthorityName", AssigningAuthorityName);
            }

            if (Displayable.HasValue)
            {
                writer.WriteStartAttribute("displayable");
                writer.WriteValue(Displayable.Value);
                writer.WriteEndAttribute();
            }
        }


        protected bool Equals(SubjectProviderIdentifier other)
        {
            return string.Equals(Root, other.Root) && string.Equals(Extension, other.Extension) && string.Equals(AssigningAuthorityName, other.AssigningAuthorityName) && Displayable == other.Displayable;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SubjectProviderIdentifier) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Root != null ? Root.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Extension != null ? Extension.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AssigningAuthorityName != null ? AssigningAuthorityName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Displayable.GetHashCode();
                return hashCode;
            }
        }
    }
}