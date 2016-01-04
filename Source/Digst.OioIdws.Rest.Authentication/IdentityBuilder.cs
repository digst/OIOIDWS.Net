using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;

namespace Digst.OioIdws.Rest.Authentication
{
    /// <summary>
    /// Default implementation that simply transforms the token information into a <see cref="ClaimsIdentity"/>
    /// </summary>
    public class IdentityBuilder : IIdentityBuilder
    {
        public virtual Task<ClaimsIdentity> BuildIdentityAsync(OioIdwsToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return Task.FromResult(
                new ClaimsIdentity(
                    token.Claims.Select(c => new Claim(c.Type, c.Value, c.ValueType, c.Issuer)).ToList(),
                    "OioIdws"));
        }
    }
}