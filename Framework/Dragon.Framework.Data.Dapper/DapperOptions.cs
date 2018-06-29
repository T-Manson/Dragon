using Dapper;
using Dragon.Framework.Data.Dapper.Common.Enums;

namespace Dragon.Framework.Data.Dapper
{
    /// <summary>
    /// Dapper配置类
    /// </summary>
    public class DapperOptions
    {
        /// <summary>
        /// 大小写规则
        /// </summary>
        public CapitalizationRule CapitalizationRule { get; set; } = CapitalizationRule.LowerCase;

        /// <summary>
        /// DB标识符映射策略
        /// </summary>
        private DbIdentifierMappingStrategy _dbIdentifierMappingStrategy = DbIdentifierMappingStrategy.Underline;

        /// <summary>
        /// 表名映射策略
        /// </summary>
        public virtual DbIdentifierMappingStrategy DbIdentifierMappingStrategy
        {
            get => _dbIdentifierMappingStrategy;
            set
            {
                _dbIdentifierMappingStrategy = value;
                DefaultTypeMap.MatchNamesWithUnderscores = _dbIdentifierMappingStrategy != DbIdentifierMappingStrategy.PascalCase;
            }
        }
    }
}
