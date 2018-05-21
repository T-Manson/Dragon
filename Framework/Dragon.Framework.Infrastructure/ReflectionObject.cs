using System;
using System.Dynamic;
using System.Reflection;

namespace Dragon.Framework.Infrastructure
{
    /// <summary>
    /// 通过动态类型包装一个反射对象。
    /// </summary>
    public class ReflectionObject : DynamicObject
    {
        private readonly object _object;
        private readonly BindingFlags _mFlags;
        private readonly Type _mReflectionType;

        public ReflectionObject(object instance, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            : this(instance, instance?.GetType(), flags)
        {

        }

        public ReflectionObject(object instance, Type reflectionType, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
        {
            Guard.ArgumentNotNull(instance, "instance");
            Guard.ArgumentNotNull(reflectionType, "reflectionType");
            _object = instance;
            _mFlags = flags;
            _mReflectionType = reflectionType;
        }

        public override bool TryInvokeMember(
                InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;
            // Find the called method using reflection
            var methodInfo = _mReflectionType.GetTypeInfo().GetMethod(
                binder.Name,
                _mFlags);

            if (methodInfo == null)
            {
                return false;
            }

            // Call the method
            result = methodInfo.Invoke(_object, args);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            // Find the called method using reflection
            var propertyInfo = _mReflectionType.GetTypeInfo().GetProperty(
                binder.Name,
                _mFlags);

            if (propertyInfo == null || !propertyInfo.CanRead || propertyInfo.GetIndexParameters().Length > 0)
            {
                return false;
            }

            // Call the method
            result = propertyInfo.GetValue(_object, null);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // Find the called method using reflection
            var propertyInfo = _mReflectionType.GetTypeInfo().GetProperty(
                binder.Name,
                _mFlags);

            if (propertyInfo == null || !propertyInfo.CanWrite || propertyInfo.GetIndexParameters().Length > 0)
            {
                return false;
            }

            // Call the method
            propertyInfo.SetValue(_object, value, null);
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = null;
            // Find the called method using reflection
            var propertyInfo = _mReflectionType.GetTypeInfo().GetProperty(
                binder.CallInfo.ArgumentNames[0],
                _mFlags);

            if (propertyInfo == null || !propertyInfo.CanRead || propertyInfo.GetIndexParameters().Length != indexes.Length)
            {
                return false;
            }

            // Call the method
            propertyInfo.GetValue(_object, indexes);

            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            // Find the called method using reflection
            var propertyInfo = _mReflectionType.GetTypeInfo().GetProperty(
                binder.CallInfo.ArgumentNames[0],
                _mFlags);

            if (propertyInfo == null || !propertyInfo.CanWrite || propertyInfo.GetIndexParameters().Length != indexes.Length)
            {
                return false;
            }

            // Call the method
            propertyInfo.SetValue(_object, value, indexes);
            return true;
        }
    }
}
