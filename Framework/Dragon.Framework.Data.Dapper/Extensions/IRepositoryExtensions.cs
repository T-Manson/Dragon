using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dragon.Framework.Data.Dapper.Abstractions;
using Dragon.Framework.Data.Dapper.Common;
using Dragon.Framework.Infrastructure;

namespace Dragon.Framework.Data.Dapper.Extensions
{
    /// <summary>
    /// 仓储类扩展
    /// </summary>
    public static class IRepositoryExtensions
    {
        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <typeparam name="T">映射类型</typeparam>
        /// <param name="repository">仓储类</param>
        /// <param name="predicate">Lambda表达式</param>
        /// <returns></returns>
        public static int Delete<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate)
            where T : class
        {
            Guard.ArgumentNotNull(predicate, nameof(predicate));
            var filter = LambdaQueryParser.Where(predicate);
            return repository.Delete(filter);
        }

        /// <summary>
        /// 异步根据条件删除
        /// </summary>
        /// <typeparam name="T">映射类型</typeparam>
        /// <param name="repository">仓储类</param>
        /// <param name="predicate">Lambda表达式</param>
        /// <returns></returns>
        public static Task<int> DeleteAsync<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate)
            where T : class
        {
            Guard.ArgumentNotNull(predicate, nameof(predicate));
            var filter = LambdaQueryParser.Where(predicate);
            return repository.DeleteAsync(filter);
        }

        /// <summary>
        /// 根据条件查询数量
        /// </summary>
        /// <typeparam name="T">映射类型</typeparam>
        /// <param name="repository">仓储类</param>
        /// <param name="predicate">Lambda表达式</param>
        /// <returns></returns>
        public static int Count<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate)
            where T : class
        {
            Guard.ArgumentNotNull(predicate, nameof(predicate));
            var filter = LambdaQueryParser.Where(predicate);
            return repository.Count(filter);
        }

        /// <summary>
        /// 异步根据条件查询数量
        /// </summary>
        /// <typeparam name="T">映射类型</typeparam>
        /// <param name="repository">仓储类</param>
        /// <param name="predicate">Lambda表达式</param>
        /// <returns></returns>
        public static Task<int> CountAsync<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate)
            where T : class
        {
            Guard.ArgumentNotNull(predicate, nameof(predicate));
            var filter = LambdaQueryParser.Where(predicate);
            return repository.CountAsync(filter);
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T">映射类型</typeparam>
        /// <param name="repository">仓储类</param>
        /// <param name="predicate">Lambda表达式</param>
        /// <returns></returns>
        public static IEnumerable<T> Query<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate)
            where T : class
        {
            Guard.ArgumentNotNull(predicate, nameof(predicate));
            var filter = LambdaQueryParser.Where(predicate);
            return repository.Query(filter);
        }

        /// <summary>
        /// 异步根据条件查询
        /// </summary>
        /// <typeparam name="T">映射类型</typeparam>
        /// <param name="repository">仓储类</param>
        /// <param name="predicate">Lambda表达式</param>
        /// <returns></returns>
        public static Task<IEnumerable<T>> QueryAsync<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate)
             where T : class
        {
            Guard.ArgumentNotNull(predicate, nameof(predicate));
            var filter = LambdaQueryParser.Where(predicate);
            return repository.QueryAsync(filter);
        }

        /// <summary>
        /// 根据条件分页
        /// </summary>
        /// <typeparam name="T">映射类型</typeparam>
        /// <param name="repository">仓储类</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="predicate">Lambda表达式</param>
        /// <param name="sortOptions">排序配置</param>
        /// <returns></returns>
        public static IEnumerable<T> QueryPage<T>(this IRepository<T> repository, int pageIndex, int pageSize,
            Expression<Func<T, bool>> predicate, SortOptions sortOptions = null)
            where T : class
        {
            Guard.ArgumentNotNull(predicate, nameof(predicate));
            var filter = LambdaQueryParser.Where(predicate);
            return repository.QueryPage(pageIndex, pageSize, filter, sortOptions);
        }

        /// <summary>
        /// 异步根据条件分页
        /// </summary>
        /// <typeparam name="T">映射类型</typeparam>
        /// <param name="repository">仓储类</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="predicate">Lambda表达式</param>
        /// <param name="sortOptions">排序配置</param>
        /// <returns></returns>
        public static Task<IEnumerable<T>> QueryPageAsync<T>(this IRepository<T> repository, int pageIndex, int pageSize,
            Expression<Func<T, bool>> predicate, SortOptions sortOptions = null)
            where T : class
        {
            Guard.ArgumentNotNull(predicate, nameof(predicate));
            var filter = LambdaQueryParser.Where(predicate);
            return repository.QueryPageAsync(pageIndex, pageSize, filter, sortOptions);
        }

        /// <summary>
        /// 根据条件查询一条
        /// </summary>
        /// <typeparam name="T">映射类型</typeparam>
        /// <param name="repository">仓储类</param>
        /// <param name="predicate">Lambda表达式</param>
        /// <returns></returns>
        public static T QueryFirstOrDefault<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate)
            where T : class
        {
            Guard.ArgumentNotNull(predicate, nameof(predicate));
            var filter = LambdaQueryParser.Where(predicate);
            return repository.QueryFirstOrDefault(filter);
        }

        /// <summary>
        /// 异步根据条件查询一条
        /// </summary>
        /// <typeparam name="T">映射类型</typeparam>
        /// <param name="repository">仓储类</param>
        /// <param name="predicate">Lambda表达式</param>
        /// <returns></returns>
        public static Task<T> QueryFirstOrDefaultAsync<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate)
             where T : class
        {
            Guard.ArgumentNotNull(predicate, nameof(predicate));
            var filter = LambdaQueryParser.Where(predicate);
            return repository.QueryFirstOrDefaultAsync(filter);
        }
    }
}
