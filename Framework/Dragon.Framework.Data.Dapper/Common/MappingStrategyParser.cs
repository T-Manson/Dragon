using Dragon.Framework.Data.Dapper.Common.Enums;
using Microsoft.Extensions.Options;
using System.Text;

namespace Dragon.Framework.Data.Dapper
{
    /// <summary>
    /// 将名称根据db映射策略转换为符合db规则的名称
    /// </summary>
    public static class MappingStrategyParser
    {
        /// <summary>
        /// Dapper配置
        /// </summary>
        private static DapperOptions _dapperOptions;

        /// <summary>
        /// 注入配置
        /// </summary>
        /// <param name="options"></param>
        public static void Configuration(IOptions<DapperOptions> options)
        {
            _dapperOptions = options.Value;
        }

        /// <summary>
        /// 按照映射策略获取类型名所映射的表名或属性名所映射的列名
        /// </summary>
        /// <param name="name">类型名或者属性名</param> 
        /// <returns></returns>
        public static string Parse(string name)
        {
            var strategy = _dapperOptions?.ColumnNameMappingStrategy ?? DbIdentifierMappingStrategy.Underline;
            var rule = _dapperOptions?.CapitalizationRule ?? CapitalizationRule.LowerCase;
            return Parse(name, strategy, rule);
        }

        #region 公共方法

        /// <summary>
        /// 根据大小写规则转换名称
        /// </summary>
        /// <param name="name">类型名或者属性名</param>
        /// <param name="strategy">列名映射策略</param>
        /// <param name="rule">大小写规则</param>
        /// <returns></returns>
        private static string Parse(string name, DbIdentifierMappingStrategy strategy, CapitalizationRule rule)
        {
            var result = Parse(name, strategy);
            switch (rule)
            {
                case CapitalizationRule.LowerCase:
                    return result.ToLower();
                case CapitalizationRule.UpperCase:
                    return result.ToUpper();
                case CapitalizationRule.Original:
                    return result;
            }
            return result.ToLower();
        }

        /// <summary>
        /// 根据列名映射策略转换名称
        /// </summary>
        /// <param name="name">类型名或者属性名</param>
        /// <param name="strategy">列名映射策略</param>
        /// <returns></returns>
        private static string Parse(string name, DbIdentifierMappingStrategy strategy)
        {
            if (strategy == DbIdentifierMappingStrategy.PascalCase)
            {
                return name;
            }
            var array = name.ToCharArray();
            var length = array.Length;
            var builder = new StringBuilder().Append(array[0]);
            var position = -1;
            for (var i = 1; i < length; i++)
            {
                var current = array[i];
                var prev = array[i - 1];
                if (char.IsUpper(current))
                {
                    if (char.IsLower(prev))
                    {
                        builder.Append("_").Append(current);
                        position = -1;
                    }
                    else
                    {
                        builder.Append(current);
                        position = i;
                    }
                }
                else
                {
                    builder.Append(current);
                }
            }
            if (position > 0 && char.IsLower(array[position]))
            {
                builder.Insert(position - 1, "_");
            }
            return builder.ToString();
        }

        #endregion
    }
}
