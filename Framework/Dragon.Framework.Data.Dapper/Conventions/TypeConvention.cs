using Dragon.Framework.Infrastructure;
using System;

namespace Dragon.Framework.Data.Dapper.Conventions
{
    /// <summary>
    /// 类型约定
    /// </summary>
    public class TypeConvention
    {
        /// <summary>
        /// 筛选方法
        /// </summary>
        internal Func<Type, bool> Filter { get; }

        /// <summary>
        /// 读连接串配置名称
        /// </summary>
        internal string DbReadingConnectionName { get; set; }

        /// <summary>
        /// 写连接串配置名称
        /// </summary>
        internal string DbWritingConnectionName { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filter">筛选方法</param>
        internal TypeConvention(Func<Type, bool> filter)
        {
            Guard.ArgumentNotNull(filter, nameof(filter));
            Filter = filter;
        }
    }
}
