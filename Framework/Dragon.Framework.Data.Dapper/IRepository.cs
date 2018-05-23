using Dragon.Framework.Data.Dapper.Common;
using Dragon.Framework.Data.Dapper.Query;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dragon.Framework.Data.Dapper
{
    /// <summary>
    /// 仓储接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : class
    {
        #region C

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="entity">对象</param>
        /// <returns></returns>
        int Insert(T entity);

        /// <summary>
        /// 异步插入
        /// </summary>
        /// <param name="entity">对象</param>
        /// <returns></returns>
        Task<int> InsertAsync(T entity);

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="entities">对象集合</param>
        /// <returns></returns>
        int Insert(IEnumerable<T> entities);

        /// <summary>
        /// 异步批量插入
        /// </summary>
        /// <param name="entities">对象集合</param>
        /// <returns></returns>
        Task<int> InsertAsync(IEnumerable<T> entities);

        #endregion

        #region U

        /// <summary>
        /// 根据主键修改
        /// </summary>
        /// <param name="entity">对象</param>
        /// <returns></returns>
        int Update(T entity);

        /// <summary>
        /// 异步根据主键修改
        /// </summary>
        /// <param name="entity">对象</param>
        /// <returns></returns>
        Task<int> UpdateAsync(T entity);

        /// <summary>
        /// 根据条件修改
        /// </summary>
        /// <param name="entity">对象</param>
        /// <param name="fieldsToUpdate">需要修改的字段-值</param>
        /// <returns></returns>
        int Update(T entity, IEnumerable<KeyValuePair<String, Object>> fieldsToUpdate);

        /// <summary>
        /// 异步根据条件修改
        /// </summary>
        /// <param name="entity">对象</param>
        /// <param name="fieldsToUpdate">需要修改的字段-值</param>
        /// <returns></returns>
        Task<int> UpdateAsync(T entity, IEnumerable<KeyValuePair<String, Object>> fieldsToUpdate);

        /// <summary>
        /// 根据条件修改
        /// </summary>
        /// <param name="filter">条件</param>
        /// <param name="fieldsToUpdate">需要修改的字段-值</param>
        /// <returns></returns>
        int Update(QueryFilter filter, IEnumerable<KeyValuePair<String, Object>> fieldsToUpdate);

        /// <summary>
        /// 异步根据条件修改
        /// </summary>
        /// <param name="filter">条件</param>
        /// <param name="fieldsToUpdate">需要修改的字段-值</param>
        /// <returns></returns>
        Task<int> UpdateAsync(QueryFilter filter, IEnumerable<KeyValuePair<String, Object>> fieldsToUpdate);

        #endregion

        #region R

        /// <summary>
        /// 总条数
        /// </summary>
        /// <returns></returns>
        int Count();

        /// <summary>
        /// 异步总条数
        /// </summary>
        /// <returns></returns>
        Task<int> CountAsync();

        /// <summary>
        /// 根据条件查询条数
        /// </summary>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        int Count(QueryFilter filter);

        /// <summary>
        /// 异步根据条件查询条数
        /// </summary>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        Task<int> CountAsync(QueryFilter filter);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="filter">条件</param>
        /// <param name="sortOptions">排序配置</param>
        /// <returns></returns>
        IEnumerable<T> QueryPage(int pageIndex, int pageSize, QueryFilter filter = null, SortOptions sortOptions = null);

        /// <summary>
        /// 异步分页查询
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="filter">条件</param>
        /// <param name="sortOptions">排序配置</param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryPageAsync(int pageIndex, int pageSize, QueryFilter filter = null, SortOptions sortOptions = null);

        /// <summary>
        /// 根据集合in查询
        /// </summary>
        /// <typeparam name="TField">字段类型</typeparam>
        /// <param name="fieldName">字段名</param>
        /// <param name="fieldValues">数据集合</param>
        /// <returns></returns>
        IEnumerable<T> QueryIn<TField>(string fieldName, IEnumerable<TField> fieldValues);

        /// <summary>
        /// 异步根据集合in查询
        /// </summary>
        /// <typeparam name="TField">字段类型</typeparam>
        /// <param name="fieldName">字段名</param>
        /// <param name="fieldValues">数据集合</param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryInAsync<TField>(string fieldName, IEnumerable<TField> fieldValues);

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        IEnumerable<T> Query(QueryFilter filter);

        /// <summary>
        /// 异步根据条件查询
        /// </summary>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryAsync(QueryFilter filter);

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> QueryAll();

        /// <summary>
        /// 异步查询所有
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryAllAsync();

        /// <summary>
        /// 根据条件查询一条
        /// </summary>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        T QueryFirstOrDefault(QueryFilter filter);

        /// <summary>
        /// 异步根据条件查询一条
        /// </summary>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        Task<T> QueryFirstOrDefaultAsync(QueryFilter filter);

        #endregion

        #region D

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="entity">对象</param>
        /// <returns></returns>
        int Delete(T entity);

        /// <summary>
        /// 异步根据主键删除
        /// </summary>
        /// <param name="entity">对象</param>
        /// <returns></returns>
        Task<int> DeleteAsync(T entity);

        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        int Delete(QueryFilter filter);

        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        Task<int> DeleteAsync(QueryFilter filter);

        #endregion
    }
}