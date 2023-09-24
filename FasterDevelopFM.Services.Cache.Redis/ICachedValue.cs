using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FasterDevelopFM.Services.Cache.Redis
{
    /// <summary>
    /// 定义缓存值
    /// </summary>
    public interface ICachedValue
    {

        /// <summary>
        /// 是否存在值
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// 获取原始值
        /// </summary>
        object RawValue { get; }

        /// <summary>
        /// 将缓存值转换为指定类型对象
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <returns>转换后的结果</returns>
        T CastTo<T>();
    }
}
