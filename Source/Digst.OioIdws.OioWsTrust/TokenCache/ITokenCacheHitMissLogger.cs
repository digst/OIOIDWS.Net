namespace Digst.OioIdws.OioWsTrust.TokenCache
{
    public interface ITokenCacheHitMissLogger
    {
        void CacheHit(string cacheKey);

        void CacheMiss(string cacheKey);
    }
}