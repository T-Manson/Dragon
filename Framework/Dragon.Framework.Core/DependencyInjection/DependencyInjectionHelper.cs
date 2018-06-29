using Dragon.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dragon.Framework.Core.DependencyInjection
{
    /// <summary>
    /// 依赖注入帮助类
    /// </summary>
    public static class DependencyInjectionHelper
    {
        /// <summary>
        /// 是否需要依赖注入接口类型
        /// </summary>
        public static IEnumerable<Type> GetDependencyInterfaceTypeList(IEnumerable<Type> interfaceTypes)
        {
            var interfaceTypeList = interfaceTypes.ToList();
            if (interfaceTypeList.IsNullOrEmpty()) return null;

            return interfaceTypeList.Where(IsDependencyInterface);
        }

        /// <summary>
        /// 是否为依赖注入的接口
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <returns></returns>
        private static bool IsDependencyInterface(Type interfaceType)
        {
            var singletonBaseType = typeof(ISingletonDependency);
            var transientBaseType = typeof(ITransientDependency);
            var scopedBaseType = typeof(IDependency);

            return (scopedBaseType.IsAssignableFrom(interfaceType)
                   || singletonBaseType.IsAssignableFrom(interfaceType)
                   || transientBaseType.IsAssignableFrom(interfaceType))
                   && interfaceType != scopedBaseType
                   && interfaceType != singletonBaseType
                   && interfaceType != transientBaseType;
        }
    }
}
