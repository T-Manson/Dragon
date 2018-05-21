using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Dragon.Framework.Infrastructure.Helpers
{
    /// <summary>
    /// 程序集帮助类
    /// </summary>
    public static class AssemblyHelper
    {
        /// <summary>
        /// 缓存符合搜索条件的程序集信息
        /// </summary>
        private static readonly ConcurrentDictionary<string, Assembly[]> AssembliesDict = new ConcurrentDictionary<string, Assembly[]>();

        /// <summary>
        /// 根据匹配条件获取程序集集合
        /// </summary>
        /// <param name="searchPattern">搜索条件</param>
        /// <returns></returns>
        public static Assembly[] GetAssembliesBySearchPattern(string searchPattern)
        {
            if (!AssembliesDict.TryGetValue(searchPattern, out Assembly[] assemblies))
            {
                assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, searchPattern, SearchOption.AllDirectories)
                   .Select(Assembly.LoadFrom).ToArray();
                AssembliesDict.TryAdd(searchPattern, assemblies);
            }
            return assemblies;
        }
    }
}
