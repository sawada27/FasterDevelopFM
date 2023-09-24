using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FasterDevelopFM.Services.Cache.Redis
{
    /// <summary>
    /// Redis 缓存服务实现
    /// </summary>
    public class RedisCacheService : ICacheService
    {
        private readonly ILogger<RedisCacheService> _logger;
        private readonly ConfigurationOptions _configuration;

        /// <summary>
        /// 指定容器和键之间的分隔符
        /// </summary>
        public string Delimiter => ":";

        /// <summary>
        /// 创建 <see cref="RedisCacheService"/> 对象实例
        /// </summary>
        public RedisCacheService(ILogger<RedisCacheService> logger, IConfiguration configuration, string connectString = null)
        {

            //todo:配置从配置中心获取 暂未接入

            var redisSettings = configuration.GetSection("RedisOptions");

            var result = redisSettings["RedisServers"].Split(",");
            if (result.Any())
                throw new ArgumentNullException("Redis servers can not be null.Please check the configuration file");


            _configuration = new ConfigurationOptions();
            foreach ( var item in result )
            {
                _configuration.EndPoints.Add(item);
            }

            if (int.TryParse(configuration["Database"], out var database) == false)
                database = -1;

            _configuration.DefaultDatabase = database;

            _configuration.Password = redisSettings["Password"] ?? string.Empty;

            _logger = logger;
        }



        private IDatabase database = null;
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);



        /// <summary>
        /// 尝试连接到 Redis 数据库
        /// </summary>
        /// <returns></returns>
        protected virtual async ValueTask<IDatabase> ConnectToDatabaseAsync()
        {
            if (database != null && database.Multiplexer.IsConnected)
                return database;

            //控制并发
            await semaphore.WaitAsync();
            try
            {
                if (database != null && database.Multiplexer.IsConnected)
                    return database;

                var connection = await ConnectionMultiplexer.ConnectAsync(_configuration);
                connection.ConnectionFailed += Connection_ConnectionFailed;
                connection.ErrorMessage += Connection_ErrorMessage;
                connection.InternalError += Connection_InternalError;
                connection.ConnectionRestored += Connection_ConnectionRestored;
                return database = connection.GetDatabase();

            }
            finally
            {
                semaphore.Release();
            }
        }

        private void Connection_ConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            _logger.LogInformation($"Redis Connection Restored.");
        }

        private void Connection_InternalError(object sender, InternalErrorEventArgs e)
        {
            _logger.LogError($"Redis Internal Error.\n{e.Exception}");
        }

        private void Connection_ErrorMessage(object sender, RedisErrorEventArgs e)
        {
            _logger.LogError($"Redis Error.\n{e.Message}");
        }

        private void Connection_ConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            _logger.LogError($"Redis Connection Failed.\n{e.Exception}");
        }







        /// <summary>
        /// 确认指定的缓存键是否存在
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>是否存在</returns>
        public async Task<bool> Exists(string key)
        {
            var db = await ConnectToDatabaseAsync();
            return await db.KeyExistsAsync(key);
        }


        /// <summary>
        /// 尝试获取缓存值
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存值</returns>
        public async Task<ICachedValue> TryGet(string key)
        {
            var db = await ConnectToDatabaseAsync();

            return new CachedValue(await db.StringGetAsync(key));

        }


        private class CachedValue : ICachedValue
        {
            private readonly RedisValue _value;

            public CachedValue(RedisValue value)
            {
                _value = value;
            }

            public bool HasValue => _value.HasValue;

            public object RawValue => _value.ToString();

            public T CastTo<T>()
            {
                if (HasValue == false)
                    throw new InvalidOperationException("Cache key has no value.");

                return JsonConvert.DeserializeObject<T>(_value.ToString());
            }
        }


        /// <summary>
        /// 尝试移除一个缓存值
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>是否成功移除</returns>
        public async Task<bool> TryRemove(string key)
        {
            var db = await ConnectToDatabaseAsync();

            return await db.KeyDeleteAsync(key);
        }


        /// <summary>
        /// 尝试设置缓存值
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="policy">缓存策略</param>
        /// <param name="value">缓存值</param>
        /// <param name="overwrite">是否覆写已经存在的缓存值</param>
        /// <returns>是否成功</returns>
        public async Task<bool> TrySet(string key, CachePolicy policy, object value, bool overwrite)
        {
            if (key == null)
                throw new ArgumentNullException("the key can not be null", nameof(key));
            if (policy == null)
                throw new ArgumentNullException("the policy can not be null", nameof(policy));
            if (value == null)
                throw new ArgumentNullException("the value can not be null", nameof(value));

            var db = await ConnectToDatabaseAsync();
            var data = JsonConvert.SerializeObject(value);

            return await db.StringSetAsync(key, data, policy.ExpiredTime - DateTime.UtcNow, when: overwrite ? When.Always : When.NotExists);

        }

    }
}
