using System.Collections.Generic;
using System.Linq.Expressions;
using GlobalsFramework.Linq.DeferredOrdering;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.QueryProcessing.OrderingQueries
{
    internal sealed class OrderByQueryProcessor : OrderingQueryProcessor
    {
        public override bool CanProcess(MethodCallExpression query)
        {
            var methodName = query.Method.Name;
            return methodName == "OrderBy" || methodName == "OrderByDescending";
        }

        public override ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            return ProcessOrderBy(query, parentResult, query.Method.Name == "OrderByDescending");
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
                ? typeof(NodeReference)
                : QueryProcessingHelper.GetSourceParameterType(query);

            var creationMethod = MakeGenericMethod("CreateOrderedEnumerable", sourceType, keyType);
            var orderedEnumerable = creationMethod.Invoke(null, new[] { parentResult.Result, keysResult.Result, comparer, descending });

            return new ProcessingResult(true, orderedEnumerable);
        }

        //ReSharper disable once UnusedMember.Local
        //Method is called via reflection
        private static OrderedEnumerable<TSource, TKey> CreateOrderedEnumerable<TSource, TKey>(IEnumerable<TSource> source,
            List<TKey> keys, IComparer<TKey> comparer, bool descending)
        {
            return new OrderedEnumerable<TSource, TKey>(source, keys, comparer, descending);
        }
    }
}
