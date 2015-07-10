using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace GlobalsFramework.Utils.TypeConversion
{
    internal sealed class TypeConverter
    {
        private static readonly TypeConverter ConverterInstance = new TypeConverter();

        private readonly ConcurrentDictionary<int, MethodInfo> _cachedMethods = new ConcurrentDictionary<int, MethodInfo>(); 

        private TypeConverter() { }

        internal static TypeConverter Instance
        {
            get { return ConverterInstance; }
        }

        internal object ConvertChecked(object value, Type targetType)
        {
            if (value == null)
                return null;

            var type = Nullable.GetUnderlyingType(targetType) ?? targetType;

            try
            {
                return InvokeMethod(value, GetOrAddCachedMethod(type, "ConvertChecked"));
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException is OverflowException)
                    throw e.InnerException;
                throw;
            }
        }

        internal object ConvertUnchecked(object value, Type targetType)
        {
            if (value == null)
                return null;

            var type = Nullable.GetUnderlyingType(targetType) ?? targetType;

            return InvokeMethod(value, GetOrAddCachedMethod(type, "ConvertUnchecked"));
        }

        internal object TypeAsOperation(object value, Type targetType)
        {
            return value == null ? null : InvokeMethod(value, GetOrAddCachedMethod(targetType, "TypeAsOperation"));
        }

        //ReSharper disable once UnusedMember.Local
        //method is called via reflection
        private static T ConvertUnchecked<T>(dynamic value)
        {
            return unchecked ((T) value);
        }
        //ReSharper disable once UnusedMember.Local
        //method is called via reflection
        private static T ConvertChecked<T>(dynamic value)
        {
            return checked((T)value);
        }

        //ReSharper disable once UnusedMember.Local
        //method is called via reflection
        private static T TypeAsOperation<T>(object obj)
        {
            return obj is T ? (T)obj : default(T);
        }

        private object InvokeMethod(object value, MethodInfo method)
        {
            return method.Invoke(this, new[] {value});
        }

        private MethodInfo GetOrAddCachedMethod(Type targetType, string methodName)
        {
            var hash = GetMethodHashCode(targetType, methodName);
            MethodInfo convertor;

            if (_cachedMethods.TryGetValue(hash, out convertor)) 
                return convertor;

            convertor = GetMethodConverter(targetType, methodName);
            _cachedMethods.TryAdd(hash, convertor);

            return convertor;
        }

        private MethodInfo GetMethodConverter(Type targeType, string methodName)
        {
            return GetType().GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(targeType);
        }

        private int GetMethodHashCode(Type targetType, string methodName)
        {
            return string.Format("_m{0}<{1}>", methodName, targetType).GetHashCode();
        }
    }
}
