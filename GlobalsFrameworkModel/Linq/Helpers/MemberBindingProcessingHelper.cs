using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.MemberBindingEvaluation;

namespace GlobalsFramework.Linq.Helpers
{
    internal static class MemberBindingProcessingHelper
    {
        internal static ProcessingResult ProcessBindings(List<EvaluatedMemberBinding> bindings, ProcessingResult instanceResult)
        {
            var result = instanceResult;

            foreach (var binding in bindings)
            {
                switch (binding.BindingType)
                {
                    case MemberBindingType.Assignment:
                        result = ProcessAssignmentBinding(binding, result);
                        break;
                    case MemberBindingType.ListBinding:
                        result = ProcessListBinding(binding, result);
                        break;
                    case MemberBindingType.MemberBinding:
                        result = ProcessMemeberBinding(binding, result);
                        break;
                }
            }

            return result;
        }
        internal static ProcessingResult ProcessListInitializer(EvaluatedListInitializer initializer, ProcessingResult instanceResult)
        {
            var argumentEnumerators = initializer.Arguments
                .Select(a => new
                {
                    IsSingle = a.IsSingleItem,
                    Value = a.Result,
                    Enumerator = a.IsSingleItem ? null : a.GetItems().GetEnumerator()
                })
                .ToList();

            Func<bool> moveNext = () => argumentEnumerators.Where(a => !a.IsSingle).All(a => a.Enumerator.MoveNext());
            Func<object[]> getNextArguments = () => argumentEnumerators
                .Select(a => a.IsSingle ? a.Value : a.Enumerator.Current)
                .ToArray();
            Func<object, object> processInitializer = instance =>
            {
                initializer.AddMethod.Invoke(instance, getNextArguments());
                return instance;
            };

            if (instanceResult.IsSingleItem)
                return new ProcessingResult(true, processInitializer(instanceResult.Result), true);

            var resultList = new List<object>();

            foreach (var instance in instanceResult.GetItems())
            {
                if (!moveNext())
                    throw new InvalidOperationException("Unable to perform iteration");

                resultList.Add(processInitializer(instance));
            }

            return new ProcessingResult(true, resultList);
        }

        private static ProcessingResult ProcessAssignmentBinding(EvaluatedMemberBinding binding, ProcessingResult instanceResult)
        {
            if (instanceResult.IsSingleItem)
                return new ProcessingResult(true, SetValue(instanceResult.Result, binding.Member, binding.Result.Result), true);

            var resultList = new List<object>();

            if (binding.Result.IsSingleItem)
            {
                resultList.AddRange(from object instance in instanceResult.GetItems()
                    select SetValue(instance, binding.Member, binding.Result.Result));
                return new ProcessingResult(true, resultList);
            }

            var valueEnumerator = binding.Result.GetItems().GetEnumerator();
            valueEnumerator.MoveNext();

            foreach (var instance in instanceResult.GetItems())
            {
                resultList.Add(SetValue(instance, binding.Member, valueEnumerator.Current));
                valueEnumerator.MoveNext();
            }

            return new ProcessingResult(true, resultList);
        }
        private static ProcessingResult ProcessListBinding(EvaluatedMemberBinding binding, ProcessingResult instanceResult)
        {
            var initializers = binding.EvaluatedInitializers;
            var listResult = GetMemberInstances(binding.Member, instanceResult);

            listResult = initializers.Aggregate(listResult,
                (current, initializer) => ProcessListInitializer(initializer, current));

            return SetMemberInstances(binding.Member, instanceResult, listResult);
        }

        private static ProcessingResult ProcessMemeberBinding(EvaluatedMemberBinding binding, ProcessingResult instanceResult)
        {
            var childMemberResult = GetMemberInstances(binding.Member, instanceResult);
            childMemberResult = ProcessBindings(binding.EvaluatedBindings, childMemberResult);

            return SetMemberInstances(binding.Member, instanceResult, childMemberResult);
        }

        private static ProcessingResult GetMemberInstances(MemberInfo member, ProcessingResult instanceResult)
        {
            if (instanceResult.IsSingleItem)
                return new ProcessingResult(true, GetValue(instanceResult.Result, member), true);

            var instances = from object item in instanceResult.GetItems()
                select GetValue(item, member);

            return new ProcessingResult(true, instances.ToList());
        }
        private static ProcessingResult SetMemberInstances(MemberInfo member, ProcessingResult instanceResult, ProcessingResult valueResult)
        {
            if (instanceResult.IsSingleItem)
                return new ProcessingResult(true, SetValue(instanceResult.Result, member, valueResult.Result), true);

            var resultList = new List<object>();

            if (valueResult.IsSingleItem)
            {
                resultList.AddRange(from object instance in instanceResult.GetItems()
                                    select SetValue(instance, member, valueResult.Result));
                return new ProcessingResult(true, resultList);
            }

            var valueEnumerator = valueResult.GetItems().GetEnumerator();
            valueEnumerator.MoveNext();

            foreach (var instance in instanceResult.GetItems())
            {
                resultList.Add(SetValue(instance, member, valueEnumerator.Current));
                valueEnumerator.MoveNext();
            }

            return new ProcessingResult(true, resultList);
        }

        private static object GetValue(object obj, MemberInfo memberInfo)
        {
            if (obj == null)
                throw new NullReferenceException("Object reference not set to an instance of an object");

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
                return propertyInfo.GetValue(obj);

            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
                return fieldInfo.GetValue(obj);

            throw new NotSupportedException("Supported only properties and fields for initializers");
        }
        private static object SetValue(object obj, MemberInfo memberInfo, object value)
        {
            if (obj == null)
                throw new NullReferenceException("Object reference not set to an instance of an object");

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, value);
                return obj;
            }

            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(obj, value);
                return obj;
            }

            throw new NotSupportedException("Supported only properties and fields for initializers");
        }
    }
}
