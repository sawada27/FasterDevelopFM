using System.Collections.Concurrent;

namespace FasterDevelopFM.Middleware.RateLimit
{
    /// <summary>
    /// 内存方案  //todo:分布式则用redis方案 可用increase
    /// </summary>
    public class CacheRateLimitHandler : IRateLimitHandler
    {
        private ConcurrentDictionary<string, IRateLimit> _cache = new ConcurrentDictionary<string, IRateLimit>();

        public bool CheckRateLimit(IRateLimit info)
        {
            if (_cache.TryGetValue(info.Key, out var temp))
            {
                temp.Call();
                _cache.AddOrUpdate(info.Key, temp, (key, old) => temp);
                return temp.Check();
            }

            _cache.AddOrUpdate(info.Key, info, (key, old) => info);
            return true;
        }
    }
}

