using System;
using System.Linq.Expressions;
using System.Text;

namespace Dragon.Framework.Infrastructure
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    static partial class ExtensionMethods
    {
        public static string GetMemberName<TItem, TProperty>(this TItem item, Expression<Func<TItem, TProperty>> memberAccessExpression)
        {
            return memberAccessExpression.GetMemberName();
        }

        public static string GetMemberName<T>(this Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            if (!(propertyExpression.Body is MemberExpression memberExpression))
            {
                throw new ArgumentException("the expression not a MemberAccessExpression", nameof(propertyExpression));
            }
            var propertyInfo = memberExpression.Member;
            if (propertyInfo == null)
            {
                throw new ArgumentException("the expression not a MemberAccessExpression", nameof(propertyExpression));
            }

            return memberExpression.Member.Name;
        }

        public static string GetMemberName<TItem, TMember>(this Expression<Func<TItem, TMember>> memberAccessExpression)
        {
            var memberExpression = memberAccessExpression.Body as MemberExpression;
            if (memberExpression == null && memberAccessExpression.Body is UnaryExpression unaryExpression)
            {
                memberExpression = unaryExpression.Operand as MemberExpression;
            }

            if (memberExpression == null)
                throw new NotSupportedException("Only MemberExpression can use. Example m=>m.PropertyA");
            var builder = new StringBuilder();
            QueryMemeberExpression(memberExpression, builder);
            if (builder.Length > 0)
            {
                builder = builder.Remove(0, 1);
            }
            return builder.ToString();
        }

        private static void QueryMemeberExpression(MemberExpression expression, StringBuilder builder)
        {
            builder.Insert(0, $".{expression.Member.Name}");
            if (expression.Expression.NodeType == ExpressionType.MemberAccess)
            {
                QueryMemeberExpression((MemberExpression)expression.Expression, builder);
            }
        }
    }
}
