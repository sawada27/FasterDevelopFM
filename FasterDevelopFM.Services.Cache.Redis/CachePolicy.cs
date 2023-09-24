using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FasterDevelopFM.Services.Cache.Redis
{
    /// <summary>
    /// 缓存策略
    /// </summary>
    public sealed class CachePolicy
    {

        private CachePolicy(DateTime expired)
        {

            if (expired.Kind != DateTimeKind.Utc)
                throw new ArgumentException("Expired time must be an utc datetime.", nameof(expired));

            ExpiredTime = expired;
        }

        /// <summary>
        /// 缓存过期时间
        /// </summary>
        public DateTime ExpiredTime { get; }

        /// <summary>
        /// 缓存是否有效
        /// </summary>
        public bool IsValid => ExpiredTime > DateTime.UtcNow;

        /// <summary>
        /// 创建一个在指定时间过期的缓存过期策略
        /// </summary>
        /// <param name="dateTime">过期时间</param>
        public static CachePolicy Expired(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
                throw new ArgumentNullException(nameof(dateTime), "The date time must be UTC or Local.");

            return new CachePolicy(dateTime.ToUniversalTime());

        }

        /// <summary>
        /// 创建一个在指定时间过期的缓存过期策略
        /// </summary>
        /// <param name="timeSpan">过期时间</param>
        public static CachePolicy Expired(TimeSpan timeSpan)
        {
            return Expired(DateTime.UtcNow.Add(timeSpan));
        }

    }
}
