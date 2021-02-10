using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Digst.OioIdws.Wsp.BasicPrivilegeProfile
{
    /// <summary>
    /// Deserialization of OIO SAML Basic Privilige Profile version 1.0.1 or 1.0.2.
    /// </summary>
    public class BasicPrivilegeProfileDeserializer
    {
        /// <summary>
        /// Deserializes a base64 encoded privilege assertion according to OIOSAML Basic Privilege Profile 1.0.1 or 1.0.2.
        /// </summary>
        public static PrivilegeList DeserializeBase64EncodedPrivilegeList(string base64EncodedPrivilegeList)
        {
            if (base64EncodedPrivilegeList == null) throw new ArgumentNullException(nameof(base64EncodedPrivilegeList));

            // Decode base64 to XML string.
            var privilegeListAsXml = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedPrivilegeList));

            // Determine target type for deserialization using profile version
            var targetType = GetProfileType(privilegeListAsXml);
            
            var serializer = new XmlSerializer(targetType);
            using (var rdr = new StringReader(privilegeListAsXml))
            {
                return (PrivilegeList) serializer.Deserialize(rdr);
            }
        }

        /// <summary>
        /// Determines the OIO SAML Basic Privilege Profile type by inspecting the namespaces of the XML.
        /// Returns the target type for the deserialization based on this profile version.
        /// </summary>
        private static Type GetProfileType(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var navigator = doc.CreateNavigator();
            navigator?.MoveToFirstChild();

            var namespaces = navigator?.GetNamespacesInScope(XmlNamespaceScope.Local)?.Values;

            if (namespaces == null)
            {
                throw new InvalidOperationException("Invalid XML.");
            }

            foreach (var ns in namespaces)
            {
                switch (ns)
                {
                    case "http://itst.dk/oiosaml/basic_privilege_profile":
                        return typeof(PrivilegeList101);
                    case "http://digst.dk/oiosaml/basic_privilege_profile":
                        return typeof(PrivilegeList102);
                }
            }

            throw new InvalidOperationException("Could not determine OIO SAML Basic Privilege Profile version.");
        }
    }
}