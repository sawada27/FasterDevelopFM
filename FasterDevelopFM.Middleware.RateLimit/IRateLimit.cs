namespace FasterDevelopFM.Middleware.RateLimit
{
    /// <summary>
    /// 最根本的基类
    /// </summary>
    public interface IRateLimit
    {
        /// <summary>
        /// 限流标识键
        /// </summary>
        string Key { get; }

        /// <summary>
        /// 检查是否限流  true不用 false要
        /// </summary>
        /// <returns></returns>
        bool Check();

        /// <summary>
        /// 调用后行为定义
        /// </summary>
        void Call();

    }




}
