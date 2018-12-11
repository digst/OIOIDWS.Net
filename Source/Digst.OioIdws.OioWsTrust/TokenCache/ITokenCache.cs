using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens;

namespace Digst.OioIdws.OioWsTrust.TokenCache
{
    /// <summary>
    /// The interface defines actions against the pluggable token cache. 
    /// </summary>
    public interface ITokenCache
    {
        /// <summary>
        /// Gets an item from implemented cache. Returns null if no token is available
        /// </summary>
        /// <param name="cacheKey">The cache key used to save token</param>
        /// <returns></returns>
        SecurityToken GetToken(string cacheKey);

        /// <summary>
        /// Adds an item to cache
        /// </summary>
        /// <param name="cacheKey">The cache key to save token as</param>
        /// <param name="securityToken"><see cref="SecurityToken"/></param>
        void PutToken(string cacheKey, SecurityToken securityToken);

        /// <summary>
        /// Checks if item with cacheKey is in cache
        /// </summary>
        /// <param name="cacheKey">The cache key used to save item</param>
        /// <returns>True/false depending on cache hit or miss</returns>
        bool IsInCache(string cacheKey);

    }
}
