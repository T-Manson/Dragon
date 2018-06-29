using Dapper;
using Dragon.Framework.Core.Data;
using Dragon.Framework.Data.Dapper.Abstractions;
using Dragon.Framework.Data.Dapper.Common;
using Dragon.Framework.Data.Dapper.Metadata;
using Dragon.Framework.Data.Dapper.Models;
using Dragon.Framework.Data.Dapper.Query;
using Dragon.Framework.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Dragon.Framework.Data.Dapper
{
    /// <summary>
    /// Dapper仓储实现
    /// </summary>
    /// <typeparam name="T">DB实体类型</typeparam>
    public class DapperRepository<T> : IRepository<T> where T : class
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly Lazy<ILogger> _loggerLazy;

        /// <summary>
        /// 获取当前仓储的 Dapper 上下文。
        /// </summary>
        protected DapperContext Context { get; }

        /// <summary>
        /// 元数据
        /// </summary>
        private readonly DapperMetadata _metadata;

        /// <summary>
        /// 获取当前持久化实体的类型。
        /// </summary>
        protected Type EntityType => _metadata.EntityType;

        private DapperDataSource _dataSource;
        /// <summary>
        /// 数据源
        /// </summary>
        protected DapperDataSource DataSource => _dataSource ?? (_dataSource = Context.Runtime.GetDataSource(EntityType));

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dapperContext">dapper上下文</param>
        /// <param name="loggerFactory">日志组件</param>
        public DapperRepository(DapperContext dapperContext, ILoggerFactory loggerFactory = null)
        {
            Guard.ArgumentNotNull(dapperContext, nameof(dapperContext));

            Context = dapperContext;
            _metadata = dapperContext.Runtime.GetMetadata(typeof(T));
            _loggerLazy = new Lazy<ILogger>(() => loggerFactory?.CreateLogger<DapperRepository<T>>() ?? NullLogger<DapperRepository<T>>.Instance);
        }

        /// <summary>
        /// 派生类中实现时表示获得读库的连接字符串。
        /// </summary>
        /// <returns></returns>
        protected virtual IDbConnection GetReadingConnection()
        {
            return Context.GetConnection(DataSource.ReadingConnectionName);
        }

        /// <summary>
        /// 派生类中实现时表示获得写库的连接字符串。
        /// </summary>
        /// <returns></returns>
        protected virtual IDbConnection GetWritingConnection()
        {
            return Context.GetConnection(DataSource.WritingConnectionName);
        }

        #region C

        /// <summary>
        /// <see cref="IRepository.Insert"/>
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public int Insert(T entity)
        {
            Guard.ArgumentNotNull(entity, nameof(entity));

            var segments = Context.Runtime.GetCrudSegments(EntityType);

            var sql = segments.InsertSql;
            var field = GetAutoGenerationField();

            var connection = GetWritingConnection();
            if (field != null)
            {
                using (var scope = new DbTransactionScope())
                {
                    var mergedSql = string.Concat(sql, ";", Environment.NewLine, DataSource.DatabaseProvider.GetLastedInsertId());
                    var id = connection.ExecuteScalar(mergedSql, entity);

                    scope.Complete();

                    PropertySetter.SetProperty(field.Field, entity, id);
                    return 1;
                }
            }
            var count = connection.Execute(sql, entity);
            return count;
        }

        /// <summary>
        /// <see cref="IRepository.InsertAsync"/>
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task<int> InsertAsync(T entity)
        {
            Guard.ArgumentNotNull(entity, nameof(entity));
            var segments = Context.Runtime.GetCrudSegments(EntityType);
            var sql = segments.InsertSql;
            var field = GetAutoGenerationField();

            var connection = GetWritingConnection();

            if (field != null)
            {
                using (var scope = new DbTransactionScope())
                {
                    var mergedSql = string.Concat(sql, ";", Environment.NewLine, DataSource.DatabaseProvider.GetLastedInsertId());
                    var id = await connection.ExecuteScalarAsync(mergedSql, entity);

                    scope.Complete();

                    PropertySetter.SetProperty(field.Field, entity, id);
                    return 1;
                }
            }

            var count = await connection.ExecuteAsync(sql, entity);
            return count;
        }

        /// <summary>
        /// <see cref="IRepository.Insert"/>
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        public int Insert(IEnumerable<T> entities)
        {
            if (entities.IsNullOrEmpty()) return 0;

            var connection = GetWritingConnection();
            if (!DataSource.DatabaseProvider.BuildBatchInsertSqlSupported)
            {
                var segments = Context.Runtime.GetCrudSegments(EntityType);
                var sql = segments.InsertSql;
                return connection.Execute(sql, entities);
            }
            else
            {
                DynamicParameters parameters = new DynamicParameters();
                BuildBatchInsertSql(entities, parameters, out var sql);
                return connection.Execute(sql, parameters);
            }
        }

        /// <summary>
        /// <see cref="IRepository.InsertAsync"/>
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        public Task<int> InsertAsync(IEnumerable<T> entities)
        {
            if (entities.IsNullOrEmpty()) return Task.FromResult(0);

            var connection = GetWritingConnection();
            if (!DataSource.DatabaseProvider.BuildBatchInsertSqlSupported)
            {
                var segments = Context.Runtime.GetCrudSegments(EntityType);
                var sql = segments.InsertSql;
                return connection.ExecuteAsync(sql, entities);
            }
            else
            {
                DynamicParameters parameters = new DynamicParameters();
                BuildBatchInsertSql(entities, parameters, out var sql);
                return connection.ExecuteAsync(sql, parameters);
            }
        }

        #endregion

        #region U

        /// <summary>
        /// <see cref="IRepository.Update"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update(T entity)
        {
            Guard.ArgumentNotNull(entity, nameof(entity));

            var segments = Context.Runtime.GetCrudSegments(EntityType);
            string sql = segments.UpdateSql;
            var connection = GetWritingConnection();
            return connection.Execute(sql, entity);
        }

        /// <summary>
        /// <see cref="IRepository.UpdateAsync"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(T entity)
        {
            Guard.ArgumentNotNull(entity, nameof(entity));

            var segments = Context.Runtime.GetCrudSegments(EntityType);
            string sql = segments.UpdateSql;
            var connection = GetWritingConnection();
            return connection.ExecuteAsync(sql, entity);
        }

        /// <summary>
        /// <see cref="IRepository.Update"/>
        /// </summary>
        /// <param name="fieldsToUpdate"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update(T entity, IEnumerable<KeyValuePair<string, object>> fieldsToUpdate)
        {
            Guard.ArgumentNotNull(entity, nameof(entity));
            Guard.ArgumentNotNull(fieldsToUpdate, nameof(fieldsToUpdate));

            var sql = GenerateUpdateSql(entity, fieldsToUpdate);
            var connection = GetWritingConnection();
            return connection.Execute(sql.Sql, sql.Parameters);
        }

        /// <summary>
        /// <see cref="IRepository.UpdateAsync"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="fieldsToUpdate"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(T entity, IEnumerable<KeyValuePair<String, object>> fieldsToUpdate)
        {
            Guard.ArgumentNotNull(entity, nameof(entity));
            Guard.ArgumentNotNull(fieldsToUpdate, nameof(fieldsToUpdate));

            var sql = GenerateUpdateSql(entity, fieldsToUpdate);
            var connection = GetWritingConnection();
            return connection.ExecuteAsync(sql.Sql, sql.Parameters);
        }

        /// <summary>
        /// <see cref="IRepository.Update"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="fieldsToUpdate"></param>
        /// <returns></returns>
        public int Update(QueryFilter filter, IEnumerable<KeyValuePair<String, object>> fieldsToUpdate)
        {
            Guard.ArgumentNotNull(filter, nameof(filter));
            Guard.ArgumentNotNull(fieldsToUpdate, nameof(fieldsToUpdate));

            var sql = GenerateUpdateSql(filter, fieldsToUpdate);
            var connection = GetWritingConnection();
            return connection.Execute(sql.Sql, sql.Parameters);
        }

        /// <summary>
        /// <see cref="IRepository.UpdateAsync"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="fieldsToUpdate"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(QueryFilter filter, IEnumerable<KeyValuePair<String, object>> fieldsToUpdate)
        {
            Guard.ArgumentNotNull(filter, nameof(filter));
            Guard.ArgumentNotNull(fieldsToUpdate, nameof(fieldsToUpdate));

            var sql = GenerateUpdateSql(filter, fieldsToUpdate);
            var connection = GetWritingConnection();
            return connection.ExecuteAsync(sql.Sql, sql.Parameters);
        }

        #endregion

        #region R

        /// <summary>
        /// <see cref="IRepository.Count"/>
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            string tableIdentifier = Context.Runtime.DelimitIdentifier(EntityType, _metadata.TableName);
            string sql = $"SELECT COUNT(*) FROM {tableIdentifier}";
            var connection = GetReadingConnection();
            return connection.ExecuteScalar<int>(sql);
        }

        /// <summary>
        /// <see cref="IRepository.CountAsync"/>
        /// </summary>
        /// <returns></returns>
        public Task<int> CountAsync()
        {
            string tableIdentifier = Context.Runtime.DelimitIdentifier(EntityType, _metadata.TableName);
            string sql = $"SELECT COUNT(*) FROM {tableIdentifier}";
            var connection = GetReadingConnection();
            return connection.ExecuteScalarAsync<int>(sql);
        }

        /// <summary>
        /// <see cref="IRepository.QueryFirstOrDefault"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public T QueryFirstOrDefault(QueryFilter filter)
        {
            Guard.ArgumentNotNull(filter, nameof(filter));

            var sql = GenerateQuerySql(filter);

            var connection = GetReadingConnection();
            return connection.QueryFirstOrDefault<T>(sql.Sql, sql.Parameters);
        }

        /// <summary>
        /// <see cref="IRepository.QueryFirstOrDefaultAsync"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Task<T> QueryFirstOrDefaultAsync(QueryFilter filter)
        {
            Guard.ArgumentNotNull(filter, nameof(filter));

            var sql = GenerateQuerySql(filter);

            var connection = GetReadingConnection();
            return connection.QueryFirstOrDefaultAsync<T>(sql.Sql, sql.Parameters);
        }

        /// <summary>
        /// <see cref="IRepository.Count"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public int Count(QueryFilter filter)
        {
            Guard.ArgumentNotNull(filter, nameof(filter));

            var sql = GenerateCountSql(filter);
            var connection = GetReadingConnection();
            return connection.ExecuteScalar<int>(sql.Sql, sql.Parameters);
        }

        /// <summary>
        /// <see cref="IRepository.CountAsync"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Task<int> CountAsync(QueryFilter filter)
        {
            Guard.ArgumentNotNull(filter, nameof(filter));

            var sql = GenerateCountSql(filter);
            var connection = GetReadingConnection();
            return connection.ExecuteScalarAsync<int>(sql.Sql, sql.Parameters);
        }

        /// <summary>
        /// <see cref="IRepository.QueryAll"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> QueryAll()
        {
            var segments = Context.Runtime.GetCrudSegments(EntityType);
            string sql = segments.SelectSql;

            var connection = GetReadingConnection();
            return connection.Query<T>(sql);
        }

        /// <summary>
        /// <see cref="IRepository.QueryAllAsync"/>
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<T>> QueryAllAsync()
        {
            var segments = Context.Runtime.GetCrudSegments(EntityType);
            string sql = segments.SelectSql;

            var connection = GetReadingConnection();
            return connection.QueryAsync<T>(sql);
        }

        /// <summary>
        /// <see cref="IRepository.QueryPage"/>
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <param name="sortOptions"></param>
        /// <returns></returns>
        public IEnumerable<T> QueryPage(int pageIndex, int pageSize, QueryFilter filter = null, SortOptions sortOptions = null)
        {
            var sql = GeneratePaginationSql(pageIndex, pageSize, filter, sortOptions);

            var connection = GetReadingConnection();
            return connection.Query<T>(sql.Sql, sql.Parameters);
        }

        /// <summary>
        /// <see cref="IRepository.QueryPageAsync"/>
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <param name="sortOptions"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> QueryPageAsync(int pageIndex, int pageSize, QueryFilter filter = null, SortOptions sortOptions = null)
        {
            var sql = GeneratePaginationSql(pageIndex, pageSize, filter, sortOptions);

            var connection = GetReadingConnection();
            return connection.QueryAsync<T>(sql.Sql, sql.Parameters);
        }

        /// <summary>
        /// <see cref="IRepository.QueryIn"/>
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="fieldName"></param>
        /// <param name="fieldValues"></param>
        /// <returns></returns>
        public IEnumerable<T> QueryIn<TField>(string fieldName, IEnumerable<TField> fieldValues)
        {
            Guard.ArgumentNullOrWhiteSpaceString(fieldName, nameof(fieldName));
            if (fieldValues.IsNullOrEmpty())
            {
                return Enumerable.Empty<T>();
            }
            if (fieldValues.Count() == 1)
            {
                SingleQueryFilter filter = new SingleQueryFilter();
                filter.AddEqual(fieldName, fieldValues.First());
                return Query(filter);
            }
            else
            {
                var sql = GenerateQueryInSql(fieldName, fieldValues.Cast<object>());

                var connection = GetReadingConnection();
                return connection.Query<T>(sql.Sql, sql.Parameters);
            }
        }

        /// <summary>
        /// <see cref="IRepository.QueryInAsync"/>
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="fieldName"></param>
        /// <param name="fieldValues"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> QueryInAsync<TField>(string fieldName, IEnumerable<TField> fieldValues)
        {
            Guard.ArgumentNullOrWhiteSpaceString(fieldName, nameof(fieldName));
            if (fieldValues.IsNullOrEmpty())
            {
                return Task.FromResult(Enumerable.Empty<T>());
            }
            if (fieldValues.Count() == 1)
            {
                SingleQueryFilter filter = new SingleQueryFilter();
                filter.AddEqual(fieldName, fieldValues.First());
                return QueryAsync(filter);
            }
            else
            {
                var sql = GenerateQueryInSql(fieldName, fieldValues.Cast<object>());

                var connection = GetReadingConnection();
                return connection.QueryAsync<T>(sql.Sql, sql.Parameters);
            }
        }

        /// <summary>
        /// <see cref="IRepository.Query"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IEnumerable<T> Query(QueryFilter filter)
        {
            Guard.ArgumentNotNull(filter, nameof(filter));

            var sql = GenerateQuerySql(filter);

            var connection = GetReadingConnection();
            return connection.Query<T>(sql.Sql, sql.Parameters);
        }

        /// <summary>
        /// <see cref="IRepository.QueryAsync"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> QueryAsync(QueryFilter filter)
        {
            Guard.ArgumentNotNull(filter, nameof(filter));

            var sql = GenerateQuerySql(filter);

            var connection = GetReadingConnection();
            return connection.QueryAsync<T>(sql.Sql, sql.Parameters);
        }

        #endregion

        #region D

        /// <summary>
        /// <see cref="IRepository.Delete"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Delete(T entity)
        {
            Guard.ArgumentNotNull(entity, nameof(entity));

            var segments = Context.Runtime.GetCrudSegments(EntityType);
            var sql = segments.DeleteSql;
            var connection = GetWritingConnection();
            return connection.Execute(sql, entity);
        }

        /// <summary>
        /// <see cref="IRepository.DeleteAsync"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> DeleteAsync(T entity)
        {
            Guard.ArgumentNotNull(entity, nameof(entity));

            var segments = Context.Runtime.GetCrudSegments(EntityType);
            string sql = segments.DeleteSql;
            var connection = GetWritingConnection();
            return connection.ExecuteAsync(sql, entity);
        }

        /// <summary>
        /// <see cref="IRepository.Delete"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public int Delete(QueryFilter filter)
        {
            Guard.ArgumentNotNull(filter, nameof(filter));

            DynamicParameters parameters = new DynamicParameters();
            string where = Context.Runtime.SqlGenerator.GenerateFilter<T>(filter, parameters);
            string sql = $"DELETE FROM {Context.Runtime.DelimitIdentifier(EntityType, _metadata.TableName)} WHERE {where}";
            return GetWritingConnection().Execute(sql, parameters);
        }

        /// <summary>
        /// <see cref="IRepository.DeleteAsync"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Task<int> DeleteAsync(QueryFilter filter)
        {
            Guard.ArgumentNotNull(filter, nameof(filter));

            DynamicParameters parameters = new DynamicParameters();
            string where = Context.Runtime.SqlGenerator.GenerateFilter<T>(filter, parameters);
            string sql = $"DELETE FROM {Context.Runtime.DelimitIdentifier(EntityType, _metadata.TableName)} WHERE {where}";
            return GetWritingConnection().ExecuteAsync(sql, parameters);
        }

        #endregion

        #region Sql Generation

        /// <summary>
        /// 生产查询条数的SQL
        /// </summary>
        /// <param name="filter">筛选条件</param>
        /// <returns></returns>
        private DapperExecuteSql GenerateCountSql(QueryFilter filter)
        {
            var executeSql = new DapperExecuteSql();
            var whereSeg = Context.Runtime.SqlGenerator.GenerateFilter<T>(filter, executeSql.Parameters);
            executeSql.Sql = string.IsNullOrWhiteSpace(whereSeg)
                ? $"SELECT COUNT(*) FROM {Context.Runtime.DelimitIdentifier(EntityType, _metadata.TableName)}"
                : $"SELECT COUNT(*) FROM {Context.Runtime.DelimitIdentifier(EntityType, _metadata.TableName)} WHERE {whereSeg}";
            return executeSql;
        }

        /// <summary>
        /// 生成查询的SQL
        /// </summary>
        /// <param name="filter">筛选条件</param>
        /// <returns></returns>
        private DapperExecuteSql GenerateQuerySql(QueryFilter filter)
        {
            var executeSql = new DapperExecuteSql();
            var segments = Context.Runtime.GetCrudSegments(EntityType);
            executeSql.Sql = segments.SelectSql;
            var whereSeg = Context.Runtime.SqlGenerator.GenerateFilter<T>(filter, executeSql.Parameters);
            if (string.IsNullOrWhiteSpace(whereSeg)) return executeSql;
            executeSql.Sql = $"{executeSql.Sql} WHERE {whereSeg}";
            return executeSql;
        }

        /// <summary>
        /// 生成In查询的SQL
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="values">需要包含的值</param>
        /// <returns></returns>
        private DapperExecuteSql GenerateQueryInSql(string field, IEnumerable<object> values)
        {
            var executeSql = new DapperExecuteSql();
            var segments = Context.Runtime.GetCrudSegments(EntityType);
            executeSql.Sql = segments.SelectSql;
            var inSeg = Context.Runtime.SqlGenerator.GenerateInClause<T>(field, values, executeSql.Parameters);
            if (string.IsNullOrWhiteSpace(inSeg)) return executeSql;
            executeSql.Sql = $"{executeSql.Sql} WHERE {inSeg}";
            return executeSql;
        }

        /// <summary>
        /// 生成根据条件更新的SQL
        /// </summary>
        /// <param name="filter">筛选条件</param>
        /// <param name="fieldsToUpdate">需要更新的列</param>
        /// <returns></returns>
        private DapperExecuteSql GenerateUpdateSql(QueryFilter filter, IEnumerable<KeyValuePair<String, object>> fieldsToUpdate)
        {
            var executeSql = new DapperExecuteSql();

            if (!fieldsToUpdate.Any())
                throw new ArgumentException($"{typeof(IRepository<>).FullName}.{nameof(Update)} 方法的 {nameof(fieldsToUpdate)} 参数不可为 null 或空字典，如更新整个实体，考虑使用不包含 {nameof(fieldsToUpdate)} 参数的 {nameof(Update)} 重载方法。");

            string tableName = Context.Runtime.GetMetadata(EntityType).TableName;
            string setSeg = Context.Runtime.SqlGenerator.GenerateSetSegments<T>(fieldsToUpdate, executeSql.Parameters);
            string whereSeg = Context.Runtime.SqlGenerator.GenerateFilter<T>(filter, executeSql.Parameters);

            if (string.IsNullOrWhiteSpace(whereSeg)) throw new NotSupportedException("不支持没有where条件的UPDATE语句");

            executeSql.Sql = $"UPDATE {Context.Runtime.DelimitIdentifier(EntityType, tableName)} SET {setSeg} WHERE {whereSeg}";
            return executeSql;
        }

        /// <summary>
        /// 生成根据主键更新的SQL
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="fieldsToUpdate">需要更新的列</param>
        /// <returns></returns>
        private DapperExecuteSql GenerateUpdateSql(T entity, IEnumerable<KeyValuePair<String, object>> fieldsToUpdate)
        {
            var executeSql = new DapperExecuteSql();

            if (!fieldsToUpdate.Any())
                throw new ArgumentException($"{typeof(IRepository<>).FullName}.{nameof(Update)} 方法的 {nameof(fieldsToUpdate)} 参数不可为 null 或空字典，如更新整个实体，考虑使用不包含 {nameof(fieldsToUpdate)} 参数的 {nameof(Update)} 重载方法。");

            string tableName = Context.Runtime.GetMetadata(EntityType).TableName;
            string setSeg = Context.Runtime.SqlGenerator.GenerateSetSegments<T>(fieldsToUpdate, executeSql.Parameters);
            string whereSeg = Context.Runtime.SqlGenerator.GeneratePrimaryKeysWhereClause(entity, executeSql.Parameters);

            if (string.IsNullOrWhiteSpace(whereSeg)) throw new NotSupportedException("不支持没有where条件的UPDATE语句");

            executeSql.Sql = $"UPDATE {Context.Runtime.DelimitIdentifier(EntityType, tableName)} SET {setSeg} WHERE {whereSeg}";
            return executeSql;
        }

        /// <summary>
        /// 生成分页的SQL
        /// </summary>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页展示量</param>
        /// <param name="filter">筛选条件</param>
        /// <param name="sortOptions"></param>
        /// <returns></returns>
        private DapperExecuteSql GeneratePaginationSql(int pageIndex, int pageSize, QueryFilter filter, SortOptions sortOptions)
        {
            DapperExecuteSql executeSql = new DapperExecuteSql();
            sortOptions = sortOptions ?? new SortOptions(_metadata.Fields.First(f => f.IsKey).Name);
            var segments = Context.Runtime.GetCrudSegments(EntityType);
            string selectSeg = segments.SelectSql;
            string whereSeg = filter != null ? Context.Runtime.SqlGenerator.GenerateFilter<T>(filter, executeSql.Parameters) : null;
            string orderSeg = Context.Runtime.SqlGenerator.GenerateOrderBy<T>(sortOptions);
            executeSql.Sql = DataSource.DatabaseProvider.BuildPaginationTSql(pageIndex, pageSize, selectSeg, orderSeg, whereSeg);
            return executeSql;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 获取自增字段
        /// </summary>
        /// <returns></returns>
        private DapperFieldMetadata GetAutoGenerationField()
        {
            var supportTypes = new[] { typeof(int), typeof(uint), typeof(long), typeof(ulong) };
            var matedata = Context.Runtime.GetMetadata(EntityType);
            var fields = matedata.Fields
                .Where(f => f.IsKey && f.AutoGeneration && supportTypes.Contains(f.Field.PropertyType)).ToArray();
            if (DataSource.DatabaseProvider.GetLastedInsertIdSupported && fields?.Length == 1)
                return fields.First();
            return null;
        }

        /// <summary>
        /// 生产批量插入的Sql语句
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <param name="parameters">参数</param>
        /// <param name="sql">sql 语句</param>
        private void BuildBatchInsertSql(IEnumerable<T> entities, DynamicParameters parameters, out string sql)
        {
            var fields = _metadata.Fields.Where(f => !f.Ignore && !f.AutoGeneration).ToArray<DapperFieldMetadata>();
            var fieldCount = fields.Length;

            var entitiesTemp = entities.ToArray();
            List<object[]> values = new List<object[]>(entitiesTemp.Length);
            foreach (var entity in entitiesTemp)
            {
                object[] dbValues = new object[fieldCount];
                for (int i = 0; i < fieldCount; i++)
                {
                    dbValues[i] = fields[i].Field.GetValue(entity);
                }
                values.Add(dbValues);
            }

            sql = DataSource.DatabaseProvider.BuildBatchInsertSql(_metadata.TableName,
                fields.Select(f => f.Name).ToArray(), values,
                (name, value) =>
                {
                    parameters.Add(name, value);
                });
        }

        #endregion
    }
}
