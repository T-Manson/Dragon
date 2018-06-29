using Dragon.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dragon.Framework.Data.Dapper.Conventions
{
    /// <summary>
    /// 表示一个数据模型约定。
    /// </summary>
    public class ModelConvention
    {
        private readonly List<PropertyConvention> _properties;
        /// <summary>
        /// 属性约定集合
        /// </summary>
        internal IList<PropertyConvention> PropertyConventions => _properties;

        private readonly List<TypeConvention> _types;
        /// <summary>
        /// 类型约定集合
        /// </summary>
        internal IList<TypeConvention> TypeConventions => _types;

        /// <summary>
        /// 构造函数
        /// </summary>
        internal ModelConvention()
        {
            _properties = new List<PropertyConvention>(2);
            _types = new List<TypeConvention>(2);
        }

        /// <summary>
        /// 筛选特定的属性进行约定。
        /// </summary>
        /// <param name="propertyFilter">属性筛选器。</param>
        /// <returns></returns>
        public PropertyConvention Properties(Func<PropertyInfo, bool> propertyFilter)
        {
            Guard.ArgumentNotNull(propertyFilter, nameof(propertyFilter));

            var convention = new PropertyConvention(propertyFilter);
            _properties.Add(convention);
            return convention;
        }

        /// <summary>
        /// 筛选特定的类型进行约定。
        /// </summary>
        /// <param name="typeFilter">类型筛选器。</param>
        /// <returns></returns> 
        public TypeConvention Types(Func<Type, bool> typeFilter)
        {
            Guard.ArgumentNotNull(typeFilter, nameof(typeFilter));

            var convention = new TypeConvention(typeFilter);
            _types.Add(convention);
            return convention;
        }
    }
}
