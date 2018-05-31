using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Dragon.Framework.Infrastructure
{
    /// <summary>
    /// 属性集合比较器
    /// </summary>
    public sealed class PropertiesComparer
    {
        public PropertiesComparer()
        {
            RefrencePropertyHandling = RefrencePropertyHandling.PropertyCompare;
            CollectionPropertyHandling = CollectionPropertyHandling.ElementPropertyCompare;
        }

        /// <summary>
        /// 引用类型属性比较策略。
        /// </summary>
        public RefrencePropertyHandling RefrencePropertyHandling { get; set; }

        /// <summary>
        /// 集合类型属性比较策略。
        /// </summary>
        public CollectionPropertyHandling CollectionPropertyHandling { get; set; }

        /// <summary>
        /// 搜索属性时的标识。
        /// </summary>
        public BindingFlags PropertyBindingFlags { get; set; }

        private bool CollectionEquals(IEnumerable enum1, IEnumerable enum2, CollectionPropertyHandling handling)
        {
            if (handling == CollectionPropertyHandling.Ingore)
            {
                return true;
            }

            var refrenceEquals = NullReferenceEquals(enum1, enum2, out var containsNull);
            if (containsNull || refrenceEquals)
            {
                return refrenceEquals;
            }

            var array1 = enum1.Cast<object>().ToArray();
            var array2 = enum2.Cast<object>().ToArray();

            if (array1.Length != array2.Length)
            {
                return false;
            }

            switch (handling)
            {
                case CollectionPropertyHandling.ElementEqualCompare:
                    return CheckElementEqualCompare(array1, array2);
                case CollectionPropertyHandling.ElementPropertyCompare:
                    return CheckElementPropertyCompare(array1, array2);
                case CollectionPropertyHandling.EqualCompare:
                    return enum1.Equals(enum2);
                default:
                    return true;
            }
        }

        private bool CheckElementPropertyCompare(object[] array1, object[] array2)
        {
            for (var i = 0; i < array1.Length; i++)
            {
                var element1 = array1[i];
                var element2 = array2[i];
                NullReferenceEquals(element1, element2, out var containsNullElement);
                if (containsNullElement)
                {
                    return false;
                }

                var type1 = element1.GetType();
                var type2 = element2.GetType();
                type1 = type1.GetTypeInfo().IsAssignableFrom(type2) ? type1 : type2;
                if (!PropertyEquals(type1, element1, element2))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CheckElementEqualCompare(object[] array1, object[] array2)
        {
            for (var i = 0; i < array1.Length; i++)
            {
                NullReferenceEquals(array1[i], array2[i], out var containsNullElement);

                if (containsNullElement)
                {
                    return false;
                }

                if (!array1[i].Equals(array2[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ObjectEquals(Type propertyType, object value1, object value2, RefrencePropertyHandling handling)
        {
            switch (handling)
            {
                case RefrencePropertyHandling.EqualCompare:
                    var refrenceEquals = NullReferenceEquals(value1, value2, out var containsNull);
                    return containsNull || refrenceEquals ? refrenceEquals : value1.Equals(value2);
                case RefrencePropertyHandling.PropertyCompare:
                    return PropertyEquals(propertyType, value1, value2);
                default:
                    return true;
            }
        }

        private static bool NullReferenceEquals(object object1, object object2, out bool containsNullValue)
        {
            containsNullValue = true;
            if (object1 == null && object2 == null)
            {
                return true;
            }
            else if (object1 == null)
            {
                return false;
            }
            else if (object2 == null)
            {
                return false;
            }
            containsNullValue = false;
            return ReferenceEquals(object1, object2);
        }

        /// <summary>
        /// 对 object1 和 object2 进行属性值比较。
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="object1">要比较的第一个对象。</param>
        /// <param name="object2">要比较的第二个对象。</param>
        /// <param name="ingoreProperties">忽略比较的属性设置。</param>
        /// <returns></returns>
        public bool PropertyEquals<T>(Type objectType, T object1, T object2, Action<ExcludeProperties<T>> ingoreProperties = null)
            where T : class
        {
            var properties = new ExcludeProperties<T>();
            ingoreProperties?.Invoke(properties);
            return PropertyEquals(typeof(T), object1, object2, properties.PropertyNames.ToArray());
        }

        /// <summary>
        /// 对 object1 和 object2 进行属性值比较。
        /// </summary>
        /// <param name="objectType">要进行属性比较的类型。</param>
        /// <param name="object1">要比较的第一个对象。</param>
        /// <param name="object2">要比较的第二个对象。</param>
        /// <param name="ingorePropertNames">忽略比较的属性名称。</param>
        /// <returns></returns>
        public bool PropertyEquals(Type objectType, object object1, object object2, params string[] ingorePropertNames)
        {
            Guard.ArgumentNotNull(objectType, "objectType");
            Guard.TypeIsAssignableFromType(object1.GetType(), objectType, "object1");
            Guard.TypeIsAssignableFromType(object2.GetType(), objectType, "object2");

            ingorePropertNames = ingorePropertNames ?? new String[0];
            if (objectType.GetTypeInfo().IsValueType)
            {
                return object1.Equals(object2);
            }

            var refrenceEquals = NullReferenceEquals(object1, object2, out var containsNull);
            if (containsNull)
            {
                return refrenceEquals;
            }

            if (refrenceEquals)
            {
                return true;
            }

            var infos = objectType.GetTypeInfo().GetProperties(PropertyBindingFlags);

            return PropertyEquals(object1, object2, ingorePropertNames, infos);
        }

        private bool PropertyEquals(object object1, object object2, string[] ingorePropertNames, PropertyInfo[] infos)
        {
            foreach (var property in infos)
            {
                if (ingorePropertNames.Contains(property.Name))
                {
                    continue;
                }

                var index = property.GetIndexParameters();
                if (index.Length > 0)
                {
                    continue;
                }

                var value1 = property.GetValue(object1, null);
                var value2 = property.GetValue(object2, null);

                if (value2 == null && value1 == null)
                {
                    continue;
                }

                if (value2 == null || value1 == null)
                {
                    return false;
                }

                return PropertyEquals(property, value1, value2);
            }

            return true;
        }

        private bool PropertyEquals(PropertyInfo property, object value1, object value2)
        {
            if (!property.PropertyType.GetTypeInfo().IsValueType)
            {
                if (typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(property.PropertyType))
                {
                    if (!CollectionEquals(value1 as IEnumerable, value2 as IEnumerable, CollectionPropertyHandling))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!ObjectEquals(property.PropertyType, value1, value2, RefrencePropertyHandling))
                    {
                        return false;
                    }
                }
            }

            if (!value1.Equals(value2))
            {
                return false;
            }

            return true;
        }

        public sealed class ExcludeProperties<T>
        {
            internal ExcludeProperties()
            {
                PropertyNames = new Collection<string>();
            }
            internal Collection<String> PropertyNames { get; set; }

            public ExcludeProperties<T> AddProperty<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
            {
                if (propertyExpression != null)
                {
                    var name = propertyExpression.GetMemberName();
                    if (!PropertyNames.Contains(name))
                    {
                        PropertyNames.Add(name);
                    }
                }
                return this;
            }
        }
    }

    /// <summary>
    /// 属性比较时对于引用类型的比较策略。
    /// </summary>
    public enum RefrencePropertyHandling
    {
        /// <summary>
        /// 调用 Equal 方法进行比较。
        /// </summary>
        EqualCompare,
        /// <summary>
        /// 比较引用类型的属性（递归）。
        /// </summary>
        PropertyCompare,
        /// <summary>
        /// 忽略引用类型属性的比较。
        /// </summary>
        Ingore,
    }

    /// <summary>
    /// 属性比较时对于可枚举类型的比较策略。
    /// </summary>
    public enum CollectionPropertyHandling
    {
        /// <summary>
        /// 调用 Equal 方法进行比较。
        /// </summary>
        EqualCompare,
        /// <summary>
        /// 对集合中每一个元素 调用 Equal 方法进行比较。
        /// </summary>
        ElementEqualCompare,
        /// <summary>
        /// 比较引用类型的属性（递归）。
        /// </summary>
        ElementPropertyCompare,
        /// <summary>
        /// 忽略集合类型属性的比较。
        /// </summary>
        Ingore,
    }
}
