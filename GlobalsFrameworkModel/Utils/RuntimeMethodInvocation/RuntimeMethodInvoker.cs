using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GlobalsFramework.Utils.RuntimeMethodInvocation
{
    internal static class RuntimeMethodInvoker
    {
        private static readonly ConcurrentDictionary<int, MethodInfo> CachedMethods = new ConcurrentDictionary<int, MethodInfo>();

        internal static void InvokeAction(Action method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            InvokeMethodInternal(method, binding, false, methodParameters);
        }
        internal static void InvokeAction<T>(Action<T> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            InvokeMethodInternal(method, binding, false, methodParameters);
        }
        internal static void InvokeAction<T1, T2>(Action<T1, T2> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            InvokeMethodInternal(method, binding, false, methodParameters);
        }
        internal static void InvokeAction<T1, T2, T3>(Action<T1, T2, T3> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            InvokeMethodInternal(method, binding, false, methodParameters);
        }
        internal static void InvokeAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            InvokeMethodInternal(method, binding, false, methodParameters);
        }
        internal static void InvokeAction<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            InvokeMethodInternal(method, binding, false, methodParameters);
        }

        internal static void InvokeActionCached(Action method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            InvokeMethodInternal(method, binding, true, methodParameters);
        }
        internal static void InvokeActionCached<T>(Action<T> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            InvokeMethodInternal(method, binding, true, methodParameters);
        }
        internal static void InvokeActionCached<T1, T2>(Action<T1, T2> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            InvokeMethodInternal(method, binding, true, methodParameters);
        }
        internal static void InvokeActionCached<T1, T2, T3>(Action<T1, T2, T3> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            InvokeMethodInternal(method, binding, true, methodParameters);
        }
        internal static void InvokeActionCached<T1, T2, T3, T4>(Action<T1, T2, T3, T4> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            InvokeMethodInternal(method, binding, true, methodParameters);
        }
        internal static void InvokeActionCached<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            InvokeMethodInternal(method, binding, true, methodParameters);
        }

        internal static object InvokeFunc<TResult>(Func<TResult> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            return InvokeMethodInternal(method, binding, false, methodParameters);
        }
        internal static object InvokeFunc<T, TResult>(Func<T, TResult> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            return InvokeMethodInternal(method, binding, false, methodParameters);
        }
        internal static object InvokeFunc<T1, T2, TResult>(Func<T1, T2, TResult> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            return InvokeMethodInternal(method, binding, false, methodParameters);
        }
        internal static object InvokeFunc<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            return InvokeMethodInternal(method, binding, false, methodParameters);
        }
        internal static object InvokeFunc<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            return InvokeMethodInternal(method, binding, false, methodParameters);
        }
        internal static object InvokeFunc<T1, T2, T3, T4, T5, TResult>(Action<T1, T2, T3, T4, T5, TResult> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            return InvokeMethodInternal(method, binding, false, methodParameters);
        }

        internal static object InvokeFuncCached<TResult>(Func<TResult> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            return InvokeMethodInternal(method, binding, true, methodParameters);
        }
        internal static object InvokeFuncCached<T, TResult>(Func<T, TResult> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            return InvokeMethodInternal(method, binding, true, methodParameters);
        }
        internal static object InvokeFuncCached<T1, T2, TResult>(Func<T1, T2, TResult> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            return InvokeMethodInternal(method, binding, true, methodParameters);
        }
        internal static object InvokeFuncCached<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            return InvokeMethodInternal(method, binding, true, methodParameters);
        }
        internal static object InvokeFuncCached<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            return InvokeMethodInternal(method, binding, true, methodParameters);
        }
        internal static object InvokeFuncCached<T1, T2, T3, T4, T5, TResult>(Action<T1, T2, T3, T4, T5, TResult> method, RuntimeTypeBinding binding, params object[] methodParameters)
        {
            return InvokeMethodInternal(method, binding, true, methodParameters);
        }

        private static object InvokeMethodInternal(Delegate methodDelegate, RuntimeTypeBinding binding, bool useCache, params object[] methodParameters)
        {
            var genericMethod = GetGenericMethod(methodDelegate, useCache, binding);
            return genericMethod.Invoke(methodDelegate.Target, methodParameters);
        }

        private static MethodInfo GetGenericMethod(Delegate methodDelegate, bool useCache, RuntimeTypeBinding binding)
        {
            var method = methodDelegate.Method;

            if (!method.IsGenericMethod)
                return method;

            var genericArguments = method.GetGenericArguments().Select(binding.GetUnderlyingType).ToArray();

            if (!useCache)
                return MakeGenericMethod(method, genericArguments);

            var hashCode = CalcHashCode(methodDelegate, genericArguments);
            MethodInfo result;

            if (CachedMethods.TryGetValue(hashCode, out result))
                return result;

            result = MakeGenericMethod(method, genericArguments);
            CachedMethods.TryAdd(hashCode, result);

            return result;
        }

        private static MethodInfo MakeGenericMethod(MethodInfo methodInfo, Type[] genericArguments)
        {
            ValidateTypesAndThrow(genericArguments);
            return methodInfo.GetGenericMethodDefinition().MakeGenericMethod(genericArguments);
        }

        private static void ValidateTypesAndThrow(IEnumerable<Type> genericArguments)
        {
            var runtimeType = typeof (IRuntimeType);
            var runtimeArguments = genericArguments.Where(runtimeType.IsAssignableFrom).ToList();

            if (runtimeArguments.Any())
                throw new InvalidOperationException(string.Format("Binding for runtime types: {0} is not provided",
                    string.Join(",", runtimeArguments)));
        }

        private static int CalcHashCode(Delegate methodDelegate, IEnumerable<Type> genericArguments)
        {
            var hashStringBuilder = new StringBuilder();

            var target = methodDelegate.Target;
            hashStringBuilder.AppendFormat("Instance:{0}", target == null ? "static" : target.GetType().FullName);
            hashStringBuilder.AppendFormat("Method:{0}", methodDelegate.Method.Name);
            hashStringBuilder.AppendFormat("GenericArguments:{0}", string.Join(",", genericArguments.Select(arg => arg.FullName)));

            return hashStringBuilder.ToString().GetHashCode();
        }
    }
}
