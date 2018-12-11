using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Digst.OioIdws.Healthcare.Common
{



    [XmlRoot("UserAuthorizationList", Namespace = "urn:dk:healthcare:saml:user_authorization_profile:1.0", IsNullable = false)]
    [XmlType("UserAuthorizationListType", Namespace = "urn:dk:healthcare:saml:user_authorization_profile:1.0")]
    public class UserAuthorizationList
    {
        [Required]
        [XmlElement("UserAuthorization", Namespace = "urn:dk:healthcare:saml:user_authorization_profile:1.0", IsNullable = false)]
        public Collection<UserAuthorization> UserAuthorizations { get; set; }
    }
}