using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;

namespace Digst.OioIdws.Rest.ProviderAuthentication
{
    public class PrincipalBuilder : IPrincipalBuilder
    {
        public virtual Task<IPrincipal> BuildPrincipalAsync(OioIdwsToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return Task.FromResult((IPrincipal)
                    new ClaimsPrincipal(
                        new ClaimsIdentity(
                            token.Claims.Select(c => new Claim(c.Type, c.Value, c.ValueType, c.Issuer)).ToList(),
                            "OioIdws")));
        }
    }
}