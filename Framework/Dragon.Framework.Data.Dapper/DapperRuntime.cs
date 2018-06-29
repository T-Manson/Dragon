using Dragon.Framework.Core.Data;
using Dragon.Framework.Core.Exceptions;
using Dragon.Framework.Data.Dapper.Abstractions;
using Dragon.Framework.Data.Dapper.Common;
using Dragon.Framework.Data.Dapper.Conventions;
using Dragon.Framework.Data.Dapper.DatabaseProviders;
using Dragon.Framework.Data.Dapper.Extensions;
using Dragon.Framework.Data.Dapper.Metadata;
using Dragon.Framework.Infrastructure;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Dragon.Framework.Data.Dapper
{
    /// <summary>
    /// 表示 Dapper 运行时环境（包含实体持久化配置和数据库连接等信息）。
    /// </summary>
    public sealed class DapperRuntime
    {
        /// <summary>
        /// 元数据集合
        /// </summary>
        private readonly ConcurrentDictionary<Type, DapperMetadata> _metadataSet;

        /// <summary>
        /// Dapper配置
        /// </summary>
        private readonly IOptions<DapperDatabaseOptions> _options;

        /// <summary>
        /// 数据提供者
        /// </summary>

        private readonly ConcurrentDictionary<String, IDatabaseProvider> _databaseProviders;

        /// <summary>
        /// 默认数据提供者
        /// </summary>

        private readonly Lazy<IDatabaseProvider> _defaultDatabaseProviderLazy;

        /// <summary>
        /// Dapper数据源集合
        /// </summary>

        private readonly ConcurrentDictionary<Type, DapperDataSource> _mappedDataSources;

        /// <summary>
        /// 基础CURD语句提供者
        /// </summary>

        private readonly ConcurrentDictionary<Type, CrudSqlSegments> _crudSegments;

        private DapperSqlGenerator _sqlGenerator;
        /// <summary>
        /// 获取当前  Dapper 运行环境下的 SQL 生成器。
        /// </summary>
        public DapperSqlGenerator SqlGenerator => _sqlGenerator ?? (_sqlGenerator = new DapperSqlGenerator(this));

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">Dapper数据库配置</param>
        /// <param name="metadataProviders">元数据提供者集合</param>
        public DapperRuntime(IOptions<DapperDatabaseOptions> options,
            IEnumerable<IDapperMetadataProvider> metadataProviders)
        {
            Guard.ArgumentNotNull(options, nameof(options));

            metadataProviders = metadataProviders ?? Enumerable.Empty<IDapperMetadataProvider>();
            _defaultDatabaseProviderLazy = new Lazy<IDatabaseProvider>(() =>
                (IDatabaseProvider)Activator.CreateInstance(options.Value.DefaultDatabaseProvider));
            _mappedDataSources = new ConcurrentDictionary<Type, DapperDataSource>();
            _databaseProviders = new ConcurrentDictionary<String, IDatabaseProvider>();
            _options = options;

            var metadatas = metadataProviders.Select(m => m.GetMetadata()).ToList();
            try
            {
                _metadataSet = new ConcurrentDictionary<Type, DapperMetadata>(metadatas.ToDictionary(m => m.EntityType));
            }
            catch (ArgumentException exception)
            {
                var duplicate = metadatas.FirstOrDefault(metadata => metadatas.Count(item => item.EntityType == metadata.EntityType) > 1);
                throw new ConfigException($"{duplicate?.EntityType} 存在重复的 IDapperMetadataProvider ,请检查", exception);
            }
            _crudSegments = new ConcurrentDictionary<Type, CrudSqlSegments>();
        }

        /// <summary>
        /// 根据数据库连接字符串名称获取数据库提供程序。
        /// </summary>
        /// <param name="connectionName">数据库连接配置名</param>
        /// <returns></returns>
        public IDatabaseProvider GetDatabaseProvider(string connectionName = null)
        {
            if (connectionName.IsNullOrWhiteSpace())
                return _defaultDatabaseProviderLazy.Value;

            var databaseConnectionOption = GetDatabaseConnectionOption(connectionName);
            return _databaseProviders.GetOrAdd(databaseConnectionOption.ConnectionString, connName =>
            {
                switch (databaseConnectionOption.DatabaseProviderType)
                {
                    case DatabaseProviderType.MySql:
                        return new MySqlDatabaseProvider();
                    case DatabaseProviderType.SqlServer:
                        return new SqlServerDatabaseProvider();
                    default:
                        return _defaultDatabaseProviderLazy.Value;
                }
            });
        }

        /// <summary>
        /// 根据数据库连接字符串名称获取数据库提供程序。
        /// </summary>
        /// <param name="databaseConnectionOption">数据库连接配置</param>
        /// <returns></returns>
        public IDatabaseProvider GetDatabaseProvider(DatabaseConnectionOptions databaseConnectionOption = null)
        {
            if (databaseConnectionOption == null || databaseConnectionOption.ConnectionString.IsNullOrWhiteSpace())
                return _defaultDatabaseProviderLazy.Value;

            return _databaseProviders.GetOrAdd(databaseConnectionOption.ConnectionString, connName =>
            {
                switch (databaseConnectionOption.DatabaseProviderType)
                {
                    case DatabaseProviderType.MySql:
                        return new MySqlDatabaseProvider();
                    case DatabaseProviderType.SqlServer:
                        return new SqlServerDatabaseProvider();
                    default:
                        return _defaultDatabaseProviderLazy.Value;
                }
            });
        }

        /// <summary>
        /// 获取指定类型对应的数据源。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="throwIfNotFound">未发现元数据时是否抛异常。</param>
        public DapperDataSource GetDataSource(Type entityType, bool throwIfNotFound = true)
        {
            return _mappedDataSources.GetOrAdd(entityType, et =>
            {
                DapperMetadata metadata = GetMetadata(et, throwIfNotFound);
                return GetDataSource(metadata?.ReadingConnectionName, metadata?.WritingConnectionName);
            });
        }

        /// <summary>
        /// 根据实体类型获取 Dapper 配置的元数据。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="throwIfNotFound">未发现元数据时是否抛异常</param>
        /// <returns></returns>
        public DapperMetadata GetMetadata(Type entityType, bool throwIfNotFound = true)
        {
            if (_metadataSet.TryGetValue(entityType, out DapperMetadata value))
                return value;

            var convention = _options.Value.GetModelConvention();
            var item = convention.TypeConventions.FirstOrDefault(x => x.Filter(entityType));

            if (item == null && throwIfNotFound)
                throw new InvalidOperationException(
                    $"找不到类型 {entityType.Name} 的元数据提供程序， 请确保已经通过 {typeof(DapperMetadataProvider<>).Name} 或者{typeof(TypeConvention).Name}实现了元数据提供程序。");

            value = new DapperMetadata(entityType, item, convention);
            _metadataSet.TryAdd(entityType, value);
            return value;
        }

        /// <summary>
        /// 根据实体类型获取基本的T-SQL语句。
        /// </summary>
        /// <param name="entityType">要用于CRUD的实体类型。</param>
        /// <returns></returns>
        public CrudSqlSegments GetCrudSegments(Type entityType)
        {
            return _crudSegments.GetOrAdd(entityType, t => new CrudSqlSegments(t, this));
        }

        /// <summary>
        /// 获取数据库连接字符串。
        /// </summary>
        /// <param name="connectionName">连接配置名</param>
        /// <returns></returns>
        public DatabaseConnectionOptions GetDatabaseConnectionOption(string connectionName = null)
        {
            return _options.Value.GetDatabaseConnectionOption(connectionName);
        }

        /// <summary>
        /// 使用界定符包围T-SQL中的主体字符串。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="identifier">要进行包围的主体字符。</param>
        /// <returns></returns>
        public string DelimitIdentifier(Type entityType, string identifier)
        {
            return GetDataSource(entityType).DatabaseProvider.DelimitIdentifier(identifier);
        }

        /// <summary>
        /// 使用界定符包围T-SQL中的参数字符串。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <param name="parameter">要进行包围的参数字符。</param>
        /// <returns></returns>
        public string DelimitParameter(Type entityType, string parameter)
        {
            return GetDataSource(entityType).DatabaseProvider.DelimitParameter(parameter);
        }

        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <param name="readingConnectionName">读连接串配置名</param>
        /// <param name="writingConnectionName">写连接串配置名</param>
        /// <returns></returns>
        private DapperDataSource GetDataSource(string readingConnectionName, string writingConnectionName)
        {
            var wprovider = GetDatabaseProvider(readingConnectionName);
            var rprovider = GetDatabaseProvider(writingConnectionName);
            if (wprovider.GetType() != rprovider.GetType())
                throw new ConfigException($"数据库连接 {writingConnectionName} 和 {readingConnectionName} 配置了不同的 {nameof(IDatabaseProvider)}，读写库必须使用一致的提供程序。");

            return new DapperDataSource(wprovider, readingConnectionName, writingConnectionName);
        }
    }
}
