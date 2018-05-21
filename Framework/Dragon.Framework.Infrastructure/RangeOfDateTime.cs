using System;

namespace Dragon.Framework.Infrastructure
{
    /// <summary>
    /// 区间时间
    /// </summary>
    public struct RangeOfDateTime
    {
        public RangeOfDateTime(DateTime min, DateTime max)
        {
            Minimum = min;
            Maximum = max;
        }

        public DateTime Minimum { get; set; }

        public DateTime Maximum { get; set; }
    }
}
