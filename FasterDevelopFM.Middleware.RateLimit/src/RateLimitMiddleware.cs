using Microsoft.Extensions.Options;

namespace FasterDevelopFM.Middleware.RateLimit
{

    public class RateLimitingOption
    {
        /// <summary>
        /// 访问次数
        /// </summary>
        public int Times { get; set; } = 500;
    }

    /// <summary>
    /// 限流中间件
    /// </summary>
    public class RateLimitMiddleware
    {
        /// <summary>
        /// 请求管道
        /// </summary>
        private readonly RequestDelegate _next;
        /// <summary>
        /// 日志记录
        /// </summary>
        private ILogger<RateLimitMiddleware> _logger;
        /// <summary>
        /// 创建key
        /// </summary>
        private IRateLimitProvider _factory;
        /// <summary>
        /// 限流接口
        /// </summary>
        private IRateLimit _ratelimit;
        /// <summary>
        /// 限流配置
        /// </summary>
        private RateLimitingOption _option;
        /// <summary>
        /// 日志记录中间件，用于记录访问日志
        /// </summary>
        public RateLimitMiddleware(ILogger<RateLimitMiddleware> logger, IOptions<RateLimitingOption> options, IRateLimitProvider factory, IRateLimit ratelimit, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
            _factory = factory;
            _ratelimit = ratelimit;
            _option = options.Value;
        }
        /// <summary>
        /// 记录访问日志
        /// 先执行方法，后对执行的结果以及请求信息通过IVisitLogger进行日志记录
        /// </summary>
        public async Task Invoke(HttpContext context)
        {
            IRateLimit rateLimitingInfo = _factory.CreateRateLimit(context, _option.Times);
            if (rateLimitingInfo.Check() == false)
            {
                _logger.LogInformation("触发限流:" + rateLimitingInfo.Key);
                context.Response.StatusCode = 429;
            }
            else
            {
                await _next(context);
            }
        }
    }

}
