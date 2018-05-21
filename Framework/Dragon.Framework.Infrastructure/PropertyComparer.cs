using System;
using System.Collections.Generic;
using Dragon.Framework.Infrastructure.Helpers;

namespace Dragon.Framework.Infrastructure
{
    /// <summary>
    /// 属性比较器
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public class PropertyComparer<TSource> : IEqualityComparer<TSource>
    {
        private readonly Delegate _mPropertyGetter;

        public PropertyComparer(string propertyPath)
        {
            Guard.ArgumentNullOrWhiteSpaceString(propertyPath, "propertyName");
            _mPropertyGetter = ExpressionHelper.MakePropertyLambda(typeof(TSource), propertyPath).Compile();
        }

        public bool Equals(TSource x, TSource y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            var xProperty = _mPropertyGetter.DynamicInvoke(x);
            var yProperty = _mPropertyGetter.DynamicInvoke(y);

            if (xProperty == null && yProperty == null)
            {
                return true;
            }

            if (xProperty == null || yProperty == null)
            {
                return false;
            }

            return xProperty.Equals(yProperty);
        }


        public int GetHashCode(TSource obj)
        {
            Guard.ArgumentNotNull(obj, "obj");
            var xProperty = _mPropertyGetter.DynamicInvoke(obj);
            return xProperty == null ? int.MinValue : xProperty.GetHashCode();
        }
    }
}
