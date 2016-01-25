using System;
using System.Collections.Generic;
using Digst.OioIdws.Rest.Common;

namespace Digst.OioIdws.Rest.Server.AuthorizationServer.TokenStorage
{
    public class OioIdwsToken
    {
        public DateTimeOffset ExpiresUtc { get; set; }
        public AccessTokenType Type { get; set; }
        public string CertificateThumbprint { get; set; }
        public ICollection<OioIdwsClaim> Claims { get; set; }
    }
}
