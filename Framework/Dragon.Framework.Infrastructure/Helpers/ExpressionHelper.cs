using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Dragon.Framework.Infrastructure.Helpers
{
    /// <summary>
    /// 表达式帮助器类。
    /// </summary>
    public static class ExpressionHelper
    {
        /// <summary>
        /// 获取成员初始化表达式的属性名称和属性值的键值对集合。
        /// （如 ()=>new A { Id = 3, Name = "test" } 表达式调用此方法 ， 结果为 { { "Id", 3 }, { "Name", "test" } }）
        /// </summary>
        /// <param name="memberInitLambda">成员初始化表达式（如 ()=>new A() { Id = 4, Name = "test" }，表达式类型不符合将抛出异常，
        /// 只支持简单属性赋值常量的初始化表达式，其他类型表达式将忽略）</param>
        /// <returns></returns>
        public static IDictionary<string, object> GetMembersAndValues<T>(Expression<Func<T>> memberInitLambda)
        {
            Guard.ArgumentNotNull(memberInitLambda, nameof(memberInitLambda));
            if (!(memberInitLambda.Body is MemberInitExpression initExpression))
            {
                throw new ArgumentException("memberInitLambda 不是有效的成员初始化表达式。", nameof(memberInitLambda));
            }
            IDictionary<string, object> dic = new Dictionary<string, object>();
            foreach (var binder in initExpression.Bindings)
            {
                if (binder is MemberAssignment assigment && assigment.Expression is ConstantExpression valueExpression)
                {
                    dic.Set(assigment.Member.Name, valueExpression.Value);
                }
            }
            return dic;
        }

        /// <summary>
        /// 创建一个类型实例属性的条件表达式。
        /// </summary>
        /// <typeparam name="T">要对属性进行比较的类型。</typeparam>
        /// <typeparam name="TProperty">属性类型。</typeparam>
        /// <param name="propertyExpression">属性表达式。</param>
        /// <param name="condition">条件运算样式。</param>
        /// <param name="value">要进行比较的常值。</param>
        public static Expression<Func<T, bool>> MakePropertyConditionLambda<T, TProperty>(Expression<Func<T, TProperty>> propertyExpression, ConditionStyle condition, object value)
        {
            return (Expression<Func<T, bool>>)MakePropertyConditionLambda(typeof(T), propertyExpression.GetMemberName(), condition, value);
        }

        /// <summary>
        /// 创建属性赋值表达式  。 形如 ：(obj, o) => obj.Property = o
        /// </summary>
        /// <typeparam name="T">实例类型。</typeparam>
        /// <typeparam name="TProperty">实例属性的类型。</typeparam>
        /// <param name="propertyExpression">属性访问表达式。</param>
        /// <returns>赋值表达式。</returns>
        public static LambdaExpression MakeMemberAssignExpression<T, TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            if (!(propertyExpression.Body is MemberExpression memberExpression))
            {
                throw new ArgumentException("the expression not a MemberAccessExpression", nameof(propertyExpression));
            }

            var parameter = Expression.Parameter(typeof(TProperty), "o");
            var assign = Expression.Assign(memberExpression, parameter);
            return Expression.Lambda(assign, propertyExpression.Parameters[0], parameter);
        }

        /// <summary>
        /// 创建一个类型实例属性的条件表达式。形如 ： o=>o.PropertyName == value
        /// </summary>
        /// <param name="objectType">实例类型。</param>
        /// <param name="propertyName">属性名称。</param>
        /// <param name="condition">条件运算样式。</param>
        /// <param name="value">要进行比较的常值。</param>
        public static LambdaExpression MakePropertyConditionLambda(Type objectType, string propertyName, ConditionStyle condition, object value)
        {
            Guard.ArgumentNotNull(objectType, "objectType");
            Guard.ArgumentNullOrWhiteSpaceString(propertyName, "propertyName");
            var propertyInfo = GetPropertyInfo(objectType, propertyName, BindingFlags.Public | BindingFlags.Instance);
            var constant = (Expression)Expression.Constant(value);
            if (value != null && !propertyInfo.PropertyType.Equals(value))
            {
                constant = Expression.Convert(constant, propertyInfo.PropertyType);
            }
            var parameter = Expression.Parameter(objectType, "o");
            var property = Expression.Property(parameter, propertyInfo);

            Expression body;

            switch (condition)
            {
                case ConditionStyle.GreaterThan:
                    body = Expression.GreaterThan(property, constant);
                    break;
                case ConditionStyle.GreaterThanOrEqual:
                    body = Expression.GreaterThanOrEqual(property, constant);
                    break;
                case ConditionStyle.LessThan:
                    body = Expression.LessThan(property, constant);
                    break;
                case ConditionStyle.LessThanOrEqual:
                    body = Expression.LessThanOrEqual(property, constant);
                    break;
                case ConditionStyle.NotEquals:
                    body = Expression.NotEqual(property, constant);
                    break;
                default:
                    body = Expression.Equal(property, constant);
                    break;
            }

            return Expression.Lambda(body, parameter);
        }

        /// <summary>
        /// 创建一个将引用类型进行强制转换表达式。形如 o=>(TargetType)o
        /// </summary>
        /// <typeparam name="TArgetType">转换到的类型。</typeparam>
        /// <returns></returns>
        public static Expression<Func<object, TArgetType>> MakeConvertLambda<TArgetType>()
        {
            return (Expression<Func<object, TArgetType>>)MakeConvertLambda(typeof(TArgetType));
        }

        /// <summary>
        /// 创建一个将引用类型进行强制转换表达式。形如 o=>(TargetType)o
        /// </summary>
        /// <param name="typeConvertTo">转换到的类型。</param>
        public static LambdaExpression MakeConvertLambda(Type typeConvertTo)
        {
            Guard.ArgumentNotNull(typeConvertTo, "typeConvertTo");
            var parameter = Expression.Parameter(typeof(object), "obj");
            var converter = Expression.Convert(parameter, typeConvertTo);
            return Expression.Lambda(converter, parameter);
        }

        /// <summary>
        /// 合并两个属性访问表达式为一个属性访问表达式。如 o=>o.A 和 a=>a.B 合并为 o=>o.A.B 形式。
        /// </summary>
        /// <typeparam name="T">实例的类型。</typeparam>
        /// <typeparam name="TProperty">实例属性类型。</typeparam>
        /// <typeparam name="TChildProperty">子属性类型。</typeparam>
        /// <param name="propertyExpression">实例属性访问表达式。</param>
        /// <param name="childPropertyExpression">子属性访问表达式。</param>
        /// <returns>合并后的属性访问表达式。</returns>
        public static Expression<Func<T, TChildProperty>> MergePropertyExpression<T, TProperty, TChildProperty>(Expression<Func<T, TProperty>> propertyExpression, Expression<Func<TProperty, TChildProperty>> childPropertyExpression)
        {
            Guard.ArgumentNotNull(propertyExpression, "propertyExpression");
            Guard.ArgumentNotNull(childPropertyExpression, "childPropertyExpression");
            if (propertyExpression.Body.NodeType != ExpressionType.MemberAccess || childPropertyExpression.NodeType != ExpressionType.MemberAccess)
            {
                throw new NotSupportedException("The propertyExpression and childPropertyExpression must be the MemberAccessExpression.");
            }
            var m1 = (MemberExpression)propertyExpression.Body;
            var m2 = (MemberExpression)childPropertyExpression.Body;

            var body = Expression.MakeMemberAccess(m1, m2.Member);
            return (Expression<Func<T, TChildProperty>>)Expression.Lambda(body, propertyExpression.Parameters);
        }

        /// <summary>
        /// 构建实例属性的表达式 。形如 o=>o.A.B
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="propertyPath">实例属性路径（以 . 分格），如 PropertyA.Name。</param>
        /// <returns></returns>
        public static LambdaExpression MakePropertyLambda(Type objectType, string propertyPath)
        {
            Guard.ArgumentNotNull(objectType, "objectType");
            Guard.ArgumentNullOrWhiteSpaceString(propertyPath, "propertyPath");

            var spliteNames = propertyPath.Trim().Split('.');

            var parameter = Expression.Parameter(objectType, "obj");

            Expression expression = parameter;
            var instanceType = objectType;
            foreach (var name in spliteNames)
            {
                var property = GetPropertyInfo(instanceType, name, BindingFlags.Instance | BindingFlags.Public);
                if (property == null)
                {
                    throw new ArgumentException($"The property named {name} on the type {instanceType.AssemblyQualifiedName} was not found.");
                }
                instanceType = property.PropertyType;
                expression = Expression.MakeMemberAccess(expression, property);
            }

            return Expression.Lambda(expression, parameter);
        }

        #region 公共方法

        private static PropertyInfo GetPropertyInfo(Type objectType, string propertyName, BindingFlags propertyBindingAttr)
        {
            var property = objectType.GetTypeInfo().GetProperty(propertyName, propertyBindingAttr);
            if (property == null)
            {
                throw new ArgumentException(
                    $@"The property named ""{objectType.Name}"" in type ""{propertyName}"" was not found.");
            }
            ThrowPropertyCantRead(property);
            return property;
        }

        private static void ThrowPropertyCantRead(PropertyInfo property)
        {
            if (property.CanRead) return;
            if (property.DeclaringType != null)
                throw new ArgumentException($@"The property named ""{property.DeclaringType.Name}"" in type ""{property.Name}"" cant read.");
        }

        #endregion
    }

    /// <summary>
    /// 条件运算样式
    /// </summary>
    public enum ConditionStyle
    {
        /// <summary>
        /// 等于。
        /// </summary>
        Equals,
        /// <summary>
        /// 不等于。
        /// </summary>
        NotEquals,
        /// <summary>
        /// 大于。
        /// </summary>
        GreaterThan,
        /// <summary>
        /// 大于或等于。
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// 小于。
        /// </summary>
        LessThan,
        /// <summary>
        /// 小于或等于。
        /// </summary>
        LessThanOrEqual,
    }
}
