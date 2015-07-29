using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GlobalsFramework.Linq.DeferredOrdering;
using GlobalsFramework.Linq.ExpressionCaching;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal sealed class OrderByQueryProcessor : IQueryProcessor
    {
        public bool CanProcess(MethodCallExpression query)
        {
            switch (query.Method.Name)
            {
                case "OrderBy":
                case "OrderByDescending":
                case "ThenBy":
                case "ThenByDescending":
                    return true;
                default:
                    return false;
            }
        }

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            switch (query.Method.Name)
            {
                case "OrderBy":
                    return ProcessOrderBy(query, parentResult, false);
                case "OrderByDescending":
                    return ProcessOrderBy(query, parentResult, true);
                case "ThenBy":
                    return ProcessThenBy(query, parentResult, false);
                case "ThenByDescending":
                    return ProcessThenBy(query, parentResult, true);
                default:
                    return ProcessingResult.Unsuccessful;
            }
        }

        private ProcessingResult ProcessOrderBy(MethodCallExpression query, ProcessingResult parentResult, bool descending)
        {
            var keySelector = GetKeySelector(query);
            var comparer = GetComparer(query);

            var keysResult = GetKeys(keySelector, parentResult);
            if (!keysResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            var keyType = keySelector.ReturnType;
            var sourceType = parentResult.IsDeferred()
                ? typeof (NodeReference)
                : QueryProcessingHelper.GetSourceParameterType(query);

            var creationMethod = MakeGenericMethod("CreateOrderedEnumerable", sourceType, keyType);
            var orderedEnumerable = creationMethod.Invoke(null, new[] {parentResult.Result, keysResult.Result, comparer, descending});

            return new ProcessingResult(true, orderedEnumerable);
        }

        private ProcessingResult ProcessThenBy(MethodCallExpression query, ProcessingResult parentResult, bool descending)
        {
            var isDeferred = parentResult.IsDeferred();

            if ((!isDeferred) && (parentResult.Result is IOrderedQueryable))
                return ProcessingResult.Unsuccessful;

            var keySelector = GetKeySelector(query);
            var comparer = GetComparer(query);

            var keysResult = GetKeys(keySelector, parentResult);
            if (!keysResult.IsSuccess)
            {
                if (!isDeferred)
                    return QueryProcessingHelper.ProcessQueryByDefault(query, parentResult);

                var loadMethod = MakeGenericMethod("GetLoadedOrderedEnumerable", QueryProcessingHelper.GetSourceParameterType(query));
                var loadedEnumerable = loadMethod.Invoke(null, new[] {parentResult.Result});
                return QueryProcessingHelper.ProcessQueryByDefault(query, new ProcessingResult(true, loadedEnumerable));
            }

            var keyType = keySelector.ReturnType;
            var sourceType = parentResult.IsDeferred()
                ? typeof(NodeReference)
                : QueryProcessingHelper.GetSourceParameterType(query);

            var appendMethod = MakeGenericMethod("AppendOrderedEnumerable", sourceType, keyType);
            var orderedEnumerable = appendMethod.Invoke(null, new[] { parentResult.Result, keysResult.Result, comparer, descending });

            return new ProcessingResult(true, orderedEnumerable);
        }

        private static LambdaExpression GetKeySelector(MethodCallExpression query)
        {
            return ((UnaryExpression) query.Arguments[1]).Operand as LambdaExpression;
        }

        private static object GetComparer(MethodCallExpression query)
        {
            var hasComparer = query.Arguments.Count > 2;
            return !hasComparer ? null : ((ConstantExpression)query.Arguments[2]).Value;
        }

        private static ProcessingResult GetKeys(LambdaExpression keySelector, ProcessingResult parentResult)
        {
            if (!parentResult.IsDeferred())
            {
                var keys = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(keySelector.ReturnType));
                var compiledSelector = CompiledExpressionStorage.GetOrAddCompiledLambda(keySelector);
                
                foreach (var item in parentResult.GetItems())
                {
                    keys.Add(compiledSelector.DynamicInvoke(item));
                }

                return new ProcessingResult(true, keys);
            }

            var nodeReferences = parentResult.GetDeferredList();
            var keysResult = ExpressionProcessingHelper.ProcessExpression(keySelector.Body, nodeReferences);
            if (!keysResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            if (keysResult.IsSingleItem)
            {
                keysResult = ExpressionProcessingHelper.CopyInstances(keysResult, nodeReferences.Count,
                    () => ExpressionProcessingHelper.ProcessExpression(keySelector.Body, nodeReferences).Result);
            }

            keysResult = new ProcessingResult(true, keysResult.GetLoadedItems(keySelector.ReturnType));
            return QueryProcessingHelper.NormalizeMultipleResult(keysResult, keySelector.ReturnType);
        }

        private MethodInfo MakeGenericMethod(string methodName, params Type[] typeArguments)
        {
            return GetType()
                .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(typeArguments);
        }

        //ReSharper disable once UnusedMember.Local
        //Method is called via reflection
        private static OrderedEnumerable<TSource, TKey> CreateOrderedEnumerable<TSource, TKey>(IEnumerable<TSource> source,
            List<TKey> keys, IComparer<TKey> comparer, bool descending)
        {
            return new OrderedEnumerable<TSource, TKey>(source, keys, comparer, descending);
        }

        //ReSharper disable once UnusedMember.Local
        //Method is called via reflection
        private static OrderedEnumerable<TSource> AppendOrderedEnumerable<TSource, TKey>(OrderedEnumerable<TSource> parent, 
            List<TKey> keys, IComparer<TKey> comparer, bool descending)
        {
            return parent.CreateOrderedEnumerable(keys, comparer, descending);
        }

        //ReSharper disable once UnusedMember.Local
        //Method is called via reflection
        private static IOrderedEnumerable<TResult> GetLoadedOrderedEnumerable<TResult>(IDeferredOrderedEnumerable parent)
        {
            return parent.GetLoadedOrderedEnumerable<TResult>();
        }

    }
}
