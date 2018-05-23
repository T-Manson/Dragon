using Dapper;
using Dragon.Framework.Data.Dapper.Common.Enums;

namespace Dragon.Framework.Data.Dapper
{
    /// <summary>
    /// Dapper配置类
    /// </summary>
    public class DapperOptions
    {
        private DbIdentifierMappingStrategy _columnNameMappingStrategy = DbIdentifierMappingStrategy.Underline;

        /// <summary>
        /// 表名映射策略
        /// </summary>
        public virtual DbIdentifierMappingStrategy ColumnNameMappingStrategy
        {
            get => _columnNameMappingStrategy;
            set
            {
                _columnNameMappingStrategy = value;
                if (_columnNameMappingStrategy == DbIdentifierMappingStrategy.PascalCase)
                {
                    DefaultTypeMap.MatchNamesWithUnderscores = false;
                }
                else
                {
                    DefaultTypeMap.MatchNamesWithUnderscores = true;
                }
            }
        }

        public CapitalizationRule CapitalizationRule { get; set; } = CapitalizationRule.LowerCase;
    }
}
