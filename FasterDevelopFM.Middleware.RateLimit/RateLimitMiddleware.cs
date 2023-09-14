using Microsoft.Extensions.Options;

namespace FasterDevelopFM.Middleware.RateLimit
{
    /// append to application.json
    ///    "RateLimiting": {
    ///     "Times": 5
    ///        }
    
    public class RateLimitingOption
    {
        /// <summary>
        /// 限流总共次数 default 
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
        /// 
        /// </summary>
        private readonly IRateLimitHandler _rateLimitHandler;

        /// <summary>
        /// 日志记录
        /// </summary>
        private ILogger<RateLimitMiddleware> _logger;
        /// <summary>
        /// 创建限流对象
        /// </summary>
        private IRateLimitProvider _factory;

        /// <summary>
        /// 限流配置
        /// </summary>
        private RateLimitingOption _option;
        /// <summary>
        /// 日志记录中间件，用于记录访问日志
        /// </summary>
        public RateLimitMiddleware(ILogger<RateLimitMiddleware> logger, IOptions<RateLimitingOption> options, IRateLimitProvider factory,  RequestDelegate next,IRateLimitHandler rateLimitHandler)
        {
            _logger = logger;
            _next = next;
            _rateLimitHandler = rateLimitHandler;
            _factory = factory;
            _option = options.Value;
        }
        /// <summary>
        /// 记录访问日志
        /// </summary>
        public async Task Invoke(HttpContext context)
        {
            IRateLimit rateLimitingInfo = _factory.CreateRateLimit(context, _option.Times);
            if (_rateLimitHandler.CheckRateLimit(rateLimitingInfo) == false)
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
