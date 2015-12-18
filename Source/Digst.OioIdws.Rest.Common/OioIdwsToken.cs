using System;
using System.Collections.Generic;

namespace Digst.OioIdws.Rest.Common
{
    public class OioIdwsToken
    {
        public DateTime ValidUntilUtc { get; set; }
        public AccessTokenType Type { get; set; }
        public string CertificateThumbprint { get; set; }
        public ICollection<OioIdwsClaim> Claims { get; set; }
    }
}
