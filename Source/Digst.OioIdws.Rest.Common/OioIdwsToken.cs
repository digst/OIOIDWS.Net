using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digst.OioIdws.Rest.Common
{
    public class OioIdwsToken
    {
        public DateTime ValidUntilUtc { get; set; }
        public AccessTokenType Type { get; set; }
        public string CertificateThumbprint { get; set; }
        public ICollection<OioIdwsClaim> Claims { get; set; }
        public string NameType { get; set; }
        public string RoleType { get; set; }
        public string AuthenticationType { get; set; }
    }
}
