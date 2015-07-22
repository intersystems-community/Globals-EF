using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using GlobalsFramework.Utils.TypeDescription;

namespace GlobalsFramework.Utils.InstanceCreation
{
    internal static class InstanceCreator
    {
        internal static object CreateInstance(Type instanceType)
        {
            var constructor = instanceType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault();
            if (constructor == null)
                return FormatterServices.GetUninitializedObject(instanceType);

            var arguments = constructor.GetParameters();
            var argumentsValues = arguments.Select(a => a.ParameterType.IsValueType
                ? CreateInstance(a.ParameterType)
                : null);
            return constructor.Invoke(argumentsValues.ToArray());
        }

        internal static object GetDefaultValue(Type instanceType)
        {
            return ((instanceType.IsClass) || (EntityTypeDescriptor.IsNullableType(instanceType)))
                ? null
                : CreateInstance(instanceType);
        }
    }
}
