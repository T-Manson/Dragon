using Dragon.Framework.Data.Dapper.Common.Enums;
using Dragon.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Dragon.Framework.Data.Dapper.Query
{
    /// <summary>
    /// Lambda查询转换器
    /// </summary>
    public class LambdaQueryParser
    {
        /// <summary>
        /// 将Lambda表达式转换为条件对象
        /// </summary>
        /// <typeparam name="T">映射类型</typeparam>
        /// <param name="expression">Lambda表达式</param>
        /// <returns></returns>
        public static QueryFilter Where<T>(Expression<Func<T, bool>> expression)
        {
            Guard.ArgumentNotNull(expression, nameof(expression));
            var visitor = new SimpleVisitor();
            visitor.Visit(expression);
            return visitor.Filter;
        }
    }

    /// <summary>
    /// 简单访问者
    /// </summary>
    internal class SimpleVisitor : ExpressionVisitor
    {
        /// <summary>
        /// 查询筛选器
        /// </summary>
        private QueryFilter _filter = new SingleQueryFilter();

        /// <summary>
        /// 处理后的查询筛选器
        /// </summary>
        public QueryFilter Filter => _filter;

        /// <summary>
        /// 字段名
        /// </summary>
        private string _fieldName;

        /// <summary>
        /// 值
        /// </summary>
        private object _value;

        /// <summary>
        /// 表达式运算符匹配映射
        /// </summary>
        private static readonly IReadOnlyDictionary<ExpressionType, BinaryOperation> OperationMapping
            = new ReadOnlyDictionary<ExpressionType, BinaryOperation>(new Dictionary<ExpressionType, BinaryOperation>
            {
                { ExpressionType.Equal, BinaryOperation.Equal },
                { ExpressionType.GreaterThan, BinaryOperation.Greater },
                { ExpressionType.GreaterThanOrEqual, BinaryOperation.GreaterOrEqual },
                { ExpressionType.NotEqual, BinaryOperation.NotEqual },
                { ExpressionType.LessThan, BinaryOperation.Less },
                { ExpressionType.LessThanOrEqual, BinaryOperation.LessOrEqual }
            });

        /// <summary>
        /// 表达式从句匹配映射
        /// </summary>
        private static readonly IReadOnlyDictionary<ExpressionType, BooleanClause> ClauseMapping
            = new ReadOnlyDictionary<ExpressionType, BooleanClause>(new Dictionary<ExpressionType, BooleanClause>
            {
                { ExpressionType.And, BooleanClause.And },
                { ExpressionType.AndAlso, BooleanClause.And },
                { ExpressionType.Or, BooleanClause.Or },
                { ExpressionType.OrElse, BooleanClause.Or },
            });

        /// <summary>
        /// 访问Lambda表达式节点
        /// </summary>
        /// <typeparam name="T">Lambda表达式类型</typeparam>
        /// <param name="node">节点</param>
        /// <returns></returns>
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var expression = node.Body as BinaryExpression;
            if (expression == null) throw new ArgumentException("不是合法的表达式，请确保表达式是一个符合规范的二元表达式。");
            base.Visit(node.Body);
            return node;
        }

        /// <summary>
        /// 访问Lambda表达式所有运算符（从句、二元运算符）
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            // 遇到从句将两边条件合并
            BooleanClause clause;
            if (ClauseMapping.TryGetValue(node.NodeType, out clause))
            {
                var leftVisitor = new SimpleVisitor();
                leftVisitor.Visit(node.Left);
                var rightVisitor = new SimpleVisitor();
                rightVisitor.Visit(node.Right);
                _filter = new CombinedQueryFilter(leftVisitor._filter, rightVisitor._filter, clause);
                return node;
            }
            // 遇到二元运算符则将运算逻辑拆分为独立条件
            BinaryOperation symbol;
            if (OperationMapping.TryGetValue(node.NodeType, out symbol))
            {
                var leftVisitor = new SimpleVisitor();
                leftVisitor.Visit(node.Left);
                var rightVisitor = new SimpleVisitor();
                rightVisitor.Visit(node.Right);
                var x = new SingleQueryFilter();
                x.AddPredicate(leftVisitor._fieldName, symbol, rightVisitor._value);
                _filter = x;
                return node;
            }
            throw new NotSupportedException($"不支将 Lambda 表达式解析为 {nameof(QueryFilter)}，请确保右侧的表达式是一个符合规范的二元表达式。");
        }

        /// <summary>
        /// 访问常量
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (string.IsNullOrWhiteSpace(_fieldName))
            {
                _value = node.Value;
            }
            else
            {
                var instance = node.Value;
                var info = instance.GetType().GetMember(_fieldName).FirstOrDefault();
                var minfo = info as MethodInfo;
                if (minfo != null) _value = GetValue(minfo, instance);
                var finfo = info as FieldInfo;
                if (finfo != null) _value = GetValue(finfo, instance);
            }
            return node;
        }

        /// <summary>
        /// 访问成员
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            var expression = node.Expression;
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                var value = GetMemberValue(node);
                _value = CheckValue(value);
            }
            else
            {
                var member = node.Member;
                _fieldName = member.Name;
                Visit(expression);
            }
            return node;
        }

        /// <summary>
        /// 访问方法
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Arguments.Count == 0)
            {
                if (node.Object != null)
                {
                    if (node.Object is ConstantExpression constantExpression)
                    {
                        var instanceExpression = constantExpression;
                        _value = GetValue(node.Method, instanceExpression.Value);

                    }
                    if (node.Object is MemberExpression memberExpression && node.Object.NodeType == ExpressionType.MemberAccess)
                    {
                        var instance = GetMemberValue(memberExpression);
                        _value = GetValue(node.Method, instance);
                    }
                }
                else
                {
                    _value = node.Method.Invoke(null, new object[0]);
                }
                return node;
            }
            throw new NotSupportedException($"不支持解析给定的成员访问表达式，请考虑减少内联调用。");
        }

        #region 公共方法

        /// <summary>
        /// 判断传入的类型是否是支持的常量类型（基元类型（除object）、枚举、Guid、DateTime、TimeSpan、Nullable）
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        [DebuggerHidden]
        private static bool IsSupportedConstantType(Type type)
        {
            if (type == typeof(string)) return true;
            var info = type.GetTypeInfo();
            var p1 = new Predicate<TypeInfo>(i => i.BaseType != typeof(object) && (i.IsPrimitive || i.IsEnum));
            var p2 = new Predicate<Type>(i => new Type[] { typeof(Guid), typeof(DateTime), typeof(TimeSpan) }.Any(m => m == i));
            if (p1(info) || p2(type)) return true;
            if (!info.IsGenericType) return false;
            if (info.UnderlyingSystemType != typeof(Nullable<>)) return false;
            var first = info.GenericTypeArguments.Length == 1 ? info.GenericTypeArguments[0] : null;
            if (first == null) return false;
            return p1(first.GetTypeInfo()) || p2(first);
        }

        /// <summary>
        /// 校验值
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [DebuggerHidden]
        private static object CheckValue(object value)
        {
            if (value == null) return null;
            var type = value.GetType();
            var supported = IsSupportedConstantType(type);
            if (supported) return value;
            throw new NotSupportedException("不支持复合类型的常量值");
        }

        /// <summary>
        /// 获取成员值
        /// </summary>
        /// <param name="memberAccessExp">成员表达式</param>
        /// <returns></returns>
        [DebuggerHidden]
        private static object GetMemberValue(MemberExpression memberAccessExp)
        {
            if (memberAccessExp == null) return null;

            object instance = null;
            // 从字段、属性取值
            if (memberAccessExp.NodeType == ExpressionType.MemberAccess)
            {
                var expression = memberAccessExp.Expression;

                if (expression is MemberExpression memberExpression)
                {
                    var memeberExpression = (MemberExpression)memberAccessExp.Expression;
                    instance = GetMemberValue(memeberExpression);
                }
                else if (expression is ConstantExpression constantExpression)
                {
                    var valueExpression = (ConstantExpression)memberAccessExp.Expression;
                    instance = valueExpression.Value;
                }
            }
            if (memberAccessExp.Member is FieldInfo fieldMember) return GetValue(fieldMember, instance);
            if (memberAccessExp.Member is PropertyInfo propertyMember) return GetValue((propertyMember).GetGetMethod(), instance);
            throw new NotSupportedException($"不支持解析给定的成员访问表达式，请考虑减少内联调用。");
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="info">方法信息</param>
        /// <param name="instance">实例</param>
        /// <returns></returns>
        private static object GetValue(MethodInfo info, object instance) => info.Invoke(instance, new object[] { });

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="info">字段信息</param>
        /// <param name="instance">实例</param>
        /// <returns></returns>
        private static object GetValue(FieldInfo info, object instance) => info.GetValue(instance);

        #endregion
    }
}
