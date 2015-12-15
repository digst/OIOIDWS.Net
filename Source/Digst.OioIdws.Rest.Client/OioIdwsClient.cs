using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Digst.OioIdws.OioWsTrust;

namespace Digst.OioIdws.Rest.Client
{
    public class OioIdwsClient
    {
        public SecurityToken GetSecurityToken()
        {
            return new TokenIssuingService().RequestToken(null);
        }
    }
}
