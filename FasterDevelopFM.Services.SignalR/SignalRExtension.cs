using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using FasterDevelopFM.Services.WebSocket;

namespace FasterDevelopFM.Services.SignalR
{
    /// <summary>
    /// SignalR支持 (WebSocket)
    /// </summary>
    public static  class SignalRExtension
    {
        public static IServiceCollection AddSSignalR(this IServiceCollection services)
        {
            //services.AddSignalR()
            //    .AddMessagePackProtocol()
            //    .AddStackExchangeRedis(redis, options =>
            //    {
            //        options.Configuration.ChannelPrefix = "SignalR_";
            //    });
            services.AddScoped<IUserIdProvider, IpUserIdProvider>();
            return services;
        }

    }
}