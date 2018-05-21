using System;
using System.Threading;

namespace Dragon.Framework.Infrastructure.Helpers
{
    /// <summary>
    /// 指数退避算法帮助类
    /// </summary>
    public static class ExponentialBackoffHelper
    {
        /// <summary>
        /// 获取下一步延迟重试次方指数，如果参数pow已生成延迟指数时，延迟返回结果
        /// </summary>
        /// <param name="pow">次方指数</param>
        /// <param name="retryMethod">重试的方法（参数为指数退避后下一次的指数）</param>
        /// <param name="delaySeconds">当前延迟重试秒数</param>
        /// <param name="maxDelaySeconds">最大延迟重试秒数</param>
        /// <returns></returns>
        public static void GetNextRetryPow(int pow, Action<int> retryMethod, int delaySeconds = 1, int maxDelaySeconds = 3600)
        {
            var powTemp = 1;
            var delaySecondsTemp = delaySeconds <= 0 ? 1 : delaySeconds;
            var maxDelaySecondsTemp = maxDelaySeconds <= 0 ? 3600 : maxDelaySeconds;

            //上一步已生成延迟指数时，延迟等待
            if (pow > 0)
            {
                var milliSeconds = Math.Min(delaySecondsTemp * 1000 * (pow - 1) / 2, maxDelaySecondsTemp * 1000);

                // 生成下一步延迟重试次方指数
                powTemp = pow << 1;
                if (milliSeconds > 0)
                {
                    Thread.Sleep(milliSeconds);
                    retryMethod?.Invoke(powTemp);
                    return;
                }
            }

            retryMethod?.Invoke(powTemp);
        }
    }
}
