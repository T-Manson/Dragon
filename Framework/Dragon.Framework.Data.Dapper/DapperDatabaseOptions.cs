using Dragon.Framework.Core.Data;
using Dragon.Framework.Data.Dapper.Abstractions;
using Dragon.Framework.Data.Dapper.Conventions;
using Dragon.Framework.Data.Dapper.DatabaseProviders;
using Dragon.Framework.Infrastructure;
using System;

namespace Dragon.Framework.Data.Dapper
{
    /// <summary>
    /// Dapper数据库配置
    /// </summary>
    public class DapperDatabaseOptions : DatabaseOptions
    {
        private DapperOptions _dapperOptions;
        /// <summary>
        /// Dapper配置
        /// </summary>
        public virtual DapperOptions Dapper
        {
            get => _dapperOptions ?? (_dapperOptions = new DapperOptions());
            set => _dapperOptions = value;
        }

        /// <summary>
        /// 数据提供者
        /// </summary>
        private Type _databaseProvider;

        /// <summary>
        /// 数据模型约定
        /// </summary>
        private readonly Lazy<ModelConvention> _conventionLazy;

        /// <summary>
        /// 默认数据提供者
        /// </summary>
        public Type DefaultDatabaseProvider
        {
            get => _databaseProvider ?? typeof(MySqlDatabaseProvider);
            set
            {
                if (value != null)
                    Guard.TypeIsAssignableFromType(value, typeof(IDatabaseProvider), "DefaultDatabaseProvider");

                _databaseProvider = value;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DapperDatabaseOptions()
        {
            _conventionLazy = new Lazy<ModelConvention>(() => new ModelConvention(), false);
        }

        /// <summary>
        /// 获取数据模型约定
        /// </summary>
        /// <returns></returns>
        internal ModelConvention GetModelConvention()
        {
            return _conventionLazy.Value;
        }
    }
}
