using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digst.OioIdws.Rest.AuthorizationService
{
    public interface IAccessTokenGenerator
    {
        string GenerateAccesstoken();
    }

    public class AccessToken
    {
        public string Value { get; set; }
        public string Type { get; set; }
        public int ExpiresInSeconds { get; set; }
    }
}
