using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

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
    }
}
