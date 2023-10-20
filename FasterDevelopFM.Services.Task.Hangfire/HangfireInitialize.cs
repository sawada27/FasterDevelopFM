using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.MemoryStorage;
using Hangfire.MySql;
using Hangfire.Redis.StackExchange;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Transactions;

namespace FasterDevelopFM.Services.Task.Hangfire
{



    public static class HangfireExtension
    {
        /// <summary>
        /// 启动hangfire支持 数据库持久化模式
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="connectionString">连接字符串</param>
        public static void AddHangFireService( this IServiceCollection services,string connectionString,int retryTimes=0, StorageType storageType = StorageType.Memory)
        {

            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                               .UseSimpleAssemblyNameTypeSerializer().UseRecommendedSerializerSettings();

                switch (storageType)
                {
                    case StorageType.Memory:
                        {
                            config.UseStorage(new MemoryStorage());
                            break;
                        }
                    case StorageType.SqlServer:
                        {
                            config.UseStorage(new SqlServerStorage(connectionString));
                            break;
                        }
                    case StorageType.Redis:
                        {
                            config.UseStorage(new RedisStorage(connectionString));
                            break;
                        }
                    case StorageType.MySql:
                        {
                            config.UseStorage(new MySqlStorage(connectionString, new MySqlStorageOptions()
                            {
                                TransactionIsolationLevel = IsolationLevel.ReadCommitted, //事务隔离级别 默认读已提交
                                QueuePollInterval = TimeSpan.FromSeconds(20), //队列轮询间隔
                                JobExpirationCheckInterval = TimeSpan.FromMinutes(30), //任务过期时间检查间隔
                                CountersAggregateInterval = TimeSpan.FromMinutes(10),//聚合计数器的间隔 含义不明
                                PrepareSchemaIfNecessary = false,//表不存在就建表 取消
                                DashboardJobListLimit = 5000, //看板作业列表限制条数
                                TransactionTimeout = TimeSpan.FromMinutes(2), //事务超时时间
                                                                              //TablesPrefix = "neware_" //表前缀
                            }));
                            break;
                        }
                };

            });

            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = retryTimes});
        }

        public enum StorageType
        {
            Memory,
            SqlServer,
            Redis,
            MySql

        }

        /// <summary>
        /// 注册hangfire dashboard到管道
        /// </summary>
        /// <param name="app"></param>
        public static void UseHangfire(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/Backend/Tasks/Hangfire", new DashboardOptions()
            {
                Authorization = new[]{    new BasicAuthAuthorizationFilter(
                    new BasicAuthAuthorizationFilterOptions
                    {
                        SslRedirect = false,          // 是否将所有非SSL请求重定向到SSL URL
                        RequireSsl = false,           // 需要SSL连接才能访问HangFire Dahsboard。强烈建议在使用基本身份验证时使用SSL
                        LoginCaseSensitive = false,   //登录检查是否区分大小写
                        Users = new[]
                        {
                            //todo:先定好，后续从Iconfiguration获取
                            new BasicAuthAuthorizationUser
                            {
                                Login ="HangfireAdmin",//用户名
                                PasswordClear="zxc@fastdevelop"

                            }
                        }
                    })}
            });
        }




    }
}