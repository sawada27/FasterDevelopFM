using Microsoft.AspNetCore.Http;

namespace FasterDevelopFM.Middleware.RateLimit
{
    public interface IRateLimitProvider
    {

        /// <summary>
        /// 创建限流对象
        /// </summary>
        /// <returns></returns>
        IRateLimit CreateRateLimit(HttpContext httpContext, int max);
    }


}
