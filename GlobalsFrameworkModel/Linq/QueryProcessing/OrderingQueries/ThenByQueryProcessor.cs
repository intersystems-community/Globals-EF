using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GlobalsFramework.Linq.DeferredOrdering;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.QueryProcessing.OrderingQueries
{
    internal sealed class ThenByQueryProcessor : OrderingQueryProcessor
    {
        public override bool CanProcess(MethodCallExpression query)
        {
            var methodName = query.Method.Name;
            return methodName == "ThenBy" || methodName == "ThenByDescending";
        }

        public override ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            return ProcessThenBy(query, parentResult, query.Method.Name == "ThenByDescending");
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
                var loadedEnumerable = loadMethod.Invoke(null, new[] { parentResult.Result });
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
