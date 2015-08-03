using System;
using System.Reflection;
using GlobalsFramework.Utils.RuntimeMethodInvocation;

namespace GlobalsFramework.Utils.TypeConversion
{
    internal static class TypeConverter
    {
        internal static object ConvertChecked(object value, Type targetType)
        {
            if (value == null)
                return null;

            var type = Nullable.GetUnderlyingType(targetType) ?? targetType;

            try
            {
                return RuntimeMethodInvoker.InvokeFuncCached<RuntimeType1, RuntimeType1>(
                    ConvertChecked<RuntimeType1>, new RuntimeTypeBinding {new RuntimeType1(type)}, value);
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException is OverflowException)
                    throw e.InnerException;
                throw;
            }
        }

        internal static object ConvertUnchecked(object value, Type targetType)
        {
            if (value == null)
                return null;

            var type = Nullable.GetUnderlyingType(targetType) ?? targetType;

            return RuntimeMethodInvoker.InvokeFuncCached<RuntimeType1, RuntimeType1>(
                    ConvertUnchecked<RuntimeType1>, new RuntimeTypeBinding { new RuntimeType1(type) }, value);
        }

        internal static object TypeAsOperation(object value, Type targetType)
        {
            if (value == null)
                return null;

            return RuntimeMethodInvoker.InvokeFuncCached<RuntimeType1, RuntimeType1>(TypeAsOperation<RuntimeType1>,
                new RuntimeTypeBinding {new RuntimeType1(targetType)}, value);
        }

        private static T ConvertUnchecked<T>(dynamic value)
        {
            return unchecked ((T) value);
        }
     
        private static T ConvertChecked<T>(dynamic value)
        {
            return checked((T)value);
        }

        private static T TypeAsOperation<T>(object obj)
        {
            return obj is T ? (T)obj : default(T);
        }
    }
}
