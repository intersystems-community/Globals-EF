using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace GlobalsFramework.Utils.TypeConversion
{
    internal sealed class TypeConverter
    {
        private static readonly TypeConverter ConverterInstance = new TypeConverter();

        private readonly ConcurrentDictionary<int, MethodInfo> _cachedConverters = new ConcurrentDictionary<int, MethodInfo>(); 

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
                return InvokeConverter(value, GetOrAddCachedConverter(type, "ConvertChecked"));
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

            return InvokeConverter(value, GetOrAddCachedConverter(type, "ConvertUnchecked"));
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

        private object InvokeConverter(object value, MethodInfo converter)
        {
            return converter.Invoke(this, new[] {value});
        }

        private MethodInfo GetOrAddCachedConverter(Type targetType, string converterName)
        {
            var hash = GetConverterHashCode(targetType, converterName);
            MethodInfo convertor;

            if (_cachedConverters.TryGetValue(hash, out convertor)) 
                return convertor;

            convertor = GetConverter(targetType, converterName);
            _cachedConverters.TryAdd(hash, convertor);

            return convertor;
        }

        private MethodInfo GetConverter(Type targeType, string converterName)
        {
            return GetType().GetMethod(converterName,
                BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(targeType);
        }

        private int GetConverterHashCode(Type targetType, string converterName)
        {
            return string.Format("_m{0}<{1}>", converterName, targetType).GetHashCode();
        }
    }
}
