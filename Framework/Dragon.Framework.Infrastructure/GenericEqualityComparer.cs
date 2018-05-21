using System;
using System.Collections.Generic;

namespace Dragon.Framework.Infrastructure
{
    /// <summary>
    /// 比较器
    /// </summary>
    public class GenericEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _mComparerAction;

        public GenericEqualityComparer(Func<T, T, bool> comparerAction)
        {
            Guard.ArgumentNotNull(comparerAction, "comparerAction");
            _mComparerAction = comparerAction;
        }

        public bool Equals(T x, T y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return _mComparerAction.Invoke(x, y);
        }

        public int GetHashCode(T obj)
        {
            return _mComparerAction.GetHashCode();
        }
    }
}
