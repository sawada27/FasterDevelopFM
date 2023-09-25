using Microsoft.AspNetCore.Http;

namespace FasterDevelopFM.Middleware.RateLimit
{

    public class MinuteIpPathRateLimitProvider : IRateLimitProvider
    {
        //创建对象
        public IRateLimit CreateRateLimit(HttpContext context, int max)
        {
            if (max <= 0)
            {
                throw new ArgumentOutOfRangeException("limit count parameter must be greater than zero",nameof(max));
                //max = int.MaxValue;
            }

            return new MinuteRateLimiting($"{context.Connection.RemoteIpAddress}:{context.Request.Path}", max);
        }
    }

}
