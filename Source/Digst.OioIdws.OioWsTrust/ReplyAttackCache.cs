using System;
using System.Runtime.Caching;

namespace Digst.OioIdws.OioWsTrust
{
    /// <summary>
    ///  Class for holding information about previously recieved responses.
    ///  Could be optimized to use the configured clock screw instead of being hard coded to the default clock screw of 5 minutes.
    /// </summary>
    class ReplyAttackCache
    {
        private static ObjectCache _cache = new MemoryCache("ReplayAttackCache");

        /// <summary>
        /// Add an item to the cache.
        /// </summary>
        /// <param name="key">The key should be the signature value of the incoming response.</param>
        /// <param name="absoluteExpiryTime">Items in the cache must be set to the expiry time of the response. This prevents the cache from growing more than necessary.</param>
        internal static void Set(string key, DateTimeOffset absoluteExpiryTime)
        {
            // Add some clock screw in order for allow servers to be a little out of sync ... ideally it should always be the same as the configured WCF clock screw. Default is 5 minutes and hence 5 minutes are used.
            var dateTimeOffset = absoluteExpiryTime.AddMinutes(5); 
            _cache.Set(key, "", dateTimeOffset);
        }

        /// <summary>
        /// Used for checking if a response is replayed.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static bool DoesKeyExist(string key)
        {
            return _cache[key] != null;
        }
    }
}
