using System.Collections.Concurrent;

namespace FasterDevelopFM.Middleware.RateLimit
{
    public interface IRateLimitHandler
    {

        bool CheckRateLimit(IRateLimit info);

    }



}
