using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GlobalsFramework.Linq.DeferredOrdering;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;
using GlobalsFramework.Utils.RuntimeMethodInvocation;
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

        public override ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult, DataContext context)
        {
            return ProcessOrderBy(query, parentResult, context, query.Method.Name == "OrderByDescending");
        }

        private ProcessingResult ProcessOrderBy(MethodCallExpression query, ProcessingResult parentResult, DataContext context, bool descending)
        {
            var keySelector = GetKeySelector(query);
            var comparer = GetComparer(query);

            var keysResult = GetKeys(keySelector, parentResult, context);
            if (!keysResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            var keyType = keySelector.ReturnType;
            var sourceType = parentResult.IsDeferred()
                ? typeof(NodeReference)
                : QueryProcessingHelper.GetSourceParameterType(query);

            Func<IEnumerable<RuntimeType1>, List<RuntimeType2>, IComparer<RuntimeType2>, bool,
                OrderedEnumerable<RuntimeType1>> func = CreateOrderedEnumerable;

            var orderedEnumerable = RuntimeMethodInvoker.InvokeFuncCached(func,
                new RuntimeTypeBinding {new RuntimeType1(sourceType), new RuntimeType2(keyType)},
                parentResult.Result, keysResult.Result, comparer, descending);

            return new ProcessingResult(true, orderedEnumerable);
        }

        private static OrderedEnumerable<TSource> CreateOrderedEnumerable<TSource, TKey>(IEnumerable<TSource> source,
            List<TKey> keys, IComparer<TKey> comparer, bool descending)
        {
            return new OrderedEnumerable<TSource, TKey>(source, keys, comparer, descending);
        }
    }
}
