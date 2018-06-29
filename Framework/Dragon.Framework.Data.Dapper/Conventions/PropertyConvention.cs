using Dragon.Framework.Data.Dapper.Common.Enums;
using Dragon.Framework.Infrastructure;
using System;
using System.Reflection;

namespace Dragon.Framework.Data.Dapper.Conventions
{
    public class PropertyConvention
    {
        /// <summary>
        /// 
        /// </summary>
        internal Func<PropertyInfo, bool> Filter { get; }

        /// <summary>
        /// DB字段规则
        /// </summary>
        internal DbColumnRule DbColumnRule { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filter"></param>
        internal PropertyConvention(Func<PropertyInfo, bool> filter)
        {
            Guard.ArgumentNotNull(filter, nameof(filter));

            Filter = filter;
            DbColumnRule = DbColumnRule.None;
        }

        /// <summary>
        /// 指示该属性是一个数据库的主键。
        /// </summary>
        /// <returns></returns>
        public void IsKey()
        {
            DbColumnRule = DbColumnRule.Key;
        }

        /// <summary>
        /// 自增
        /// </summary>
        public void AutoGeneration()
        {
            DbColumnRule = DbColumnRule.AutoGeneration;
        }

        /// <summary>
        /// 自增主键
        /// </summary>
        public void AutoGenerateKey()
        {
            DbColumnRule = DbColumnRule.AutoGeneration | DbColumnRule.Key;
        }

        /// <summary>
        /// 指示该属性不映射到数据库。
        /// </summary>
        /// <returns></returns>
        public void Ignore()
        {
            DbColumnRule = DbColumnRule.Ignore;
        }
    }
}
