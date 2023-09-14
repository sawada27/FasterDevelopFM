namespace FasterDevelopFM.Middleware.RateLimit
{

    public static class RateLimitHelper
    {
        /// <summary>
        /// 配置默认限流
        /// 默认使用按每分钟ip访问次数进行限制
        /// </summary>
        public static void AddAIpRateLimiting(this IServiceCollection services, IConfiguration config)
        {
            services.AddDefaultRateLimiting(config);
            services.AddTransient<IRateLimitProvider, MinuteIpPathRateLimitProvider>();
        }

        /// <summary>
        /// 配置默认次数，存储方式
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        private static void AddDefaultRateLimiting(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<RateLimitingOption>(config.GetSection("RateLimiting"));
            services.AddSingleton<IRateLimitHandler, CacheRateLimitHandler>();
        }

    }

}