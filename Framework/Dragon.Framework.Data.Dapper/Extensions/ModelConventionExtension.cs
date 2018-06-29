using Dragon.Framework.Data.Dapper.Conventions;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Dragon.Framework.Data.Dapper.Extensions
{
    /// <summary>
    /// 数据模型约定扩展
    /// </summary>
    public static class ModelConventionExtension
    {
        /// <summary>
        /// 指定主键
        /// </summary>
        /// <typeparam name="T">表达式对应类型</typeparam>
        /// <param name="convention">模型约定</param>
        /// <param name="expression">Lambda表达式</param>
        /// <returns></returns>
        public static ModelConvention IsKey<T>(this ModelConvention convention, Expression<Func<T, object>> expression) where T : class
        {
            var property = ReadExpression(expression);
            convention.Properties(x => x == property).IsKey();
            return convention;
        }

        /// <summary>
        /// 指定自增
        /// </summary>
        /// <typeparam name="T">表达式对应类型</typeparam>
        /// <param name="convention">模型约定</param>
        /// <param name="expression">Lambda表达式</param>
        /// <returns></returns>
        public static ModelConvention AutoGeneration<T>(this ModelConvention convention, Expression<Func<T, object>> expression) where T : class
        {
            var property = ReadExpression(expression);
            convention.Properties(x => x == property).AutoGeneration();
            return convention;
        }

        /// <summary>
        /// 指定自增主键
        /// </summary>
        /// <typeparam name="T">表达式对应类型</typeparam>
        /// <param name="convention">模型约定</param>
        /// <param name="expression">Lambda表达式</param>
        /// <returns></returns>
        public static ModelConvention AutoGenerateKey<T>(this ModelConvention convention, Expression<Func<T, object>> expression) where T : class
        {
            var property = ReadExpression(expression);
            convention.Properties(x => x == property).AutoGenerateKey();
            return convention;
        }

        /// <summary>
        /// 指定忽略字段
        /// </summary>
        /// <typeparam name="T">表达式对应类型</typeparam>
        /// <param name="convention">模型约定</param>
        /// <param name="expression">Lambda表达式</param>
        /// <returns></returns>
        public static ModelConvention Ignore<T>(this ModelConvention convention, Expression<Func<T, object>> expression) where T : class
        {
            var property = ReadExpression(expression);
            convention.Properties(x => x == property).Ignore();
            return convention;
        }

        /// <summary>
        /// 读取Lambda表达式
        /// </summary>
        /// <typeparam name="T">表达式对应类型</typeparam>
        /// <param name="expression">Lambda表达式</param>
        /// <returns></returns>
        private static PropertyInfo ReadExpression<T>(Expression<Func<T, object>> expression) where T : class
        {
            var body = expression.Body;
            if (body is UnaryExpression unary) body = unary.Operand;
            var member = body as MemberExpression;
            var property = member?.Member as PropertyInfo;
            if (property == null) throw new NotSupportedException("只接受单个属性表达式,例如 a => a.Property。");
            return property;
        }
    }
}
