namespace FasterDevelopFM.Middleware.RateLimit
{



    public abstract class BaseRateLimit : IRateLimit
    {
        /// <summary>
        /// 当前请求次数
        /// </summary>
        private int _curTimes;
        /// <summary>
        /// 当前值(第一次进入的)
        /// </summary>
        private int _curValue;
        /// <summary>
        /// 上次检测次数
        /// </summary>
        private int _lastTimes;

        /// <summary>
        /// 上一次请求时间
        /// </summary>
        private DateTime _lasttime = DateTime.Now;

        /// <summary>
        /// 访问量上限
        /// </summary>
        private int _limit;

        /// <summary>
        /// 当前key
        /// </summary>
        private string _key;

        /// <summary>
        /// 当前key
        /// </summary>
        public string Key => _key;

        public BaseRateLimit(string key, int limit = 500)
        {
            _limit = limit;
            _key = key;
        }

        /// <summary>
        /// 检查是否要限流
        /// </summary>
        /// <returns></returns>
        public bool Check()
        {
            if (_lastTimes <= _limit)
            {
                return _curTimes <= _limit;
            }

            return false;
        }

        /// <summary>
        /// 调用处理
        /// </summary>
        public void Call()
        {
            int currentValue = GetCurrentValue();
            if (_curValue != currentValue)
            {
                _lastTimes = _curTimes;
                _curValue = currentValue;
                _curTimes = 1;
            }
            else
            {
                Interlocked.Increment(ref _curTimes);
            }
        }
        /// <summary>
        /// 获取当前时间范围值
        /// </summary>
        public abstract int GetCurrentValue();
    }

    /// <summary>
    /// 简单分钟限制
    /// </summary>
    public class MinuteRateLimiting : BaseRateLimit
    {
        public MinuteRateLimiting(string key, int limit = 100)
            : base(key, limit)
        {
        }
        /// <summary>
        /// 当前分钟
        /// </summary>
        public override int GetCurrentValue()
        {
            return DateTime.Now.Minute;
        }
    }



}
