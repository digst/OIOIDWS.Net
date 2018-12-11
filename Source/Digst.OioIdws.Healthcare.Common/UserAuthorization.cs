using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Digst.OioIdws.Healthcare.Common
{
    [XmlType("UserAuthorizationType", Namespace = "urn:dk:healthcare:saml:user_authorization_profile:1.0")]
    public class UserAuthorization
    {
        [Required]
        [XmlElement("AuthorizationCode", Namespace = "urn:dk:healthcare:saml:user_authorization_profile:1.0", Order = 1, IsNullable = false)]
        public string AuthorizationCode { get; set; }

        [Required]
        [XmlElement("EducationCode", Namespace = "urn:dk:healthcare:saml:user_authorization_profile:1.0", Order = 2, IsNullable = false)]
        public string EducationCode { get; set; }

        [Required]
        [XmlElement("EducationType", Namespace = "urn:dk:healthcare:saml:user_authorization_profile:1.0", Order = 3, IsNullable = false)]
        public string EducationType { get; set; }

    }
}