namespace FasterDevelopFM.Services.Cache.Redis
{
    /// <summary>
    /// 定义缓存服务的接口
    /// </summary>
    public interface ICacheService
    {

        /// <summary>
        /// 获取一个字符，用作容器和键的分隔符
        /// </summary>
        string Delimiter { get; }

        /// <summary>
        /// 获取缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存数据</returns>
        Task<ICachedValue> TryGet(string key);

        /// <summary>
        /// 查询指定缓存键是否存在
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>是否存在</returns>
        Task<bool> Exists(string key);


        /// <summary>
        /// 尝试写入缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="policy">缓存策略</param>
        /// <param name="data">缓存数据</param>
        /// <param name="overwrite">是否覆写存在的缓存值</param>
        /// <returns>是否成功写入</returns>
        Task<bool> TrySet(string key, CachePolicy policy, object data, bool overwrite);


        /// <summary>
        /// 移除一个缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>是否确实移除了缓存</returns>
        Task<bool> TryRemove(string key);

    }
}