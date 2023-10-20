using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FasterDevelopFM.Tools.Diagnostics
{

    /// <summary>
    /// 关于处理器和内存占用的健康检查器
    /// </summary>
    public class ProcessHealthCheck : IHealthCheck
    {

        private const long Ki = 1024;
        private const long Mi = Ki * Ki;
        Process process = Process.GetCurrentProcess();

        Task<HealthCheckResult> IHealthCheck.CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {

            var message = $"Porcess Id: {process.Id}, WorkSet Memory: {process.WorkingSet64 / Mi:F0} MiB, Private Memory : {process.PrivateMemorySize64 / Mi:F0} MiB, Managed Memory: {GC.GetTotalMemory(false) / Mi:F0} MiB, TimeUtc: {DateTime.UtcNow}, TimeElapsed: {DateTime.UtcNow - process.StartTime.ToUniversalTime():d' days 'hh\\:mm\\:ss}";
            return Task.FromResult(HealthCheckResult.Healthy(message));

        }
    }

    /// <summary>
    /// 工作线程检查
    /// </summary>
    internal class ThreadPoolHealthCheck : IHealthCheck
    {
        Task<HealthCheckResult> IHealthCheck.CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            ThreadPool.GetAvailableThreads(out var available1, out var available2);
            ThreadPool.GetMaxThreads(out var max1, out var max2);

            var message = $"Worker threads: {max1 - available1}/{max1}, completion port threads: {max2 - available2}/{max2}";



            if (available1 > max1 / 20 && available2 > max2 / 20)
                return Task.FromResult(HealthCheckResult.Healthy(message));

            else
                return Task.FromResult(HealthCheckResult.Unhealthy(message));



        }
    }

}





