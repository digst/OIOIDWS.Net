using System;

namespace Digst.OioIdws.OioWsTrust.TokenCache
{
    public class TokenCacheHitMissLogger : ITokenCacheHitMissLogger
    {
        public void CacheHit(string cacheKey)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{cacheKey} was found in cache");
            Console.ResetColor();

        }

        public void CacheMiss(string cacheKey)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{cacheKey} was not found in cache");
            Console.ResetColor();
        }
    }
}