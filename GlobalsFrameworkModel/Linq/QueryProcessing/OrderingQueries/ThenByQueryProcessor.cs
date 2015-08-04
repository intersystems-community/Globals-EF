using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GlobalsFramework.Linq.DeferredOrdering;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;
using GlobalsFramework.Utils.RuntimeMethodInvocation;
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

        public override ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult, DataContext context)
        {
            return ProcessThenBy(query, parentResult, context, query.Method.Name == "ThenByDescending");
        }

        private ProcessingResult ProcessThenBy(MethodCallExpression query, ProcessingResult parentResult, DataContext context, bool descending)
        {
            if ((!parentResult.IsDeferred()) && (parentResult.Result is IOrderedQueryable))
                return ProcessingResult.Unsuccessful;

            var keySelector = GetKeySelector(query);
            var comparer = GetComparer(query);

            var keysResult = GetKeys(keySelector, parentResult, context);
            if (!keysResult.IsSuccess)
                return ProcessByDefault(query, parentResult, context);
          
            var keyType = keySelector.ReturnType;
            var sourceType = parentResult.IsDeferred()
                ? typeof(NodeReference)
                : QueryProcessingHelper.GetSourceParameterType(query);

            Func<OrderedEnumerable<RuntimeType1>, List<RuntimeType2>, IComparer<RuntimeType2>, bool,
                OrderedEnumerable<RuntimeType1>> func = AppendOrderedEnumerable;

            var orderedEnumerable = RuntimeMethodInvoker.InvokeFuncCached(func,
                new RuntimeTypeBinding {new RuntimeType1(sourceType), new RuntimeType2(keyType)}, parentResult.Result,
                keysResult.Result, comparer, descending);

            return new ProcessingResult(true, orderedEnumerable);
        }

        private static ProcessingResult ProcessByDefault(MethodCallExpression query, ProcessingResult parentResult, DataContext context)
        {
            if (!parentResult.IsDeferred())
                return QueryProcessingHelper.ProcessQueryByDefault(query, parentResult, context);

            Func<IDeferredOrderedEnumerable, IOrderedEnumerable<RuntimeType1>> func = GetLoadedOrderedEnumerable<RuntimeType1>;

            var loadedEnumerable = RuntimeMethodInvoker.InvokeFuncCached(func,
                new RuntimeTypeBinding {new RuntimeType1(QueryProcessingHelper.GetSourceParameterType(query))},
                parentResult.Result);

            return QueryProcessingHelper.ProcessQueryByDefault(query, new ProcessingResult(true, loadedEnumerable), context);
        }

        private static OrderedEnumerable<TSource> AppendOrderedEnumerable<TSource, TKey>(OrderedEnumerable<TSource> parent,
            List<TKey> keys, IComparer<TKey> comparer, bool descending)
        {
            return parent.CreateOrderedEnumerable(keys, comparer, descending);
        }

        private static IOrderedEnumerable<TResult> GetLoadedOrderedEnumerable<TResult>(IDeferredOrderedEnumerable parent)
        {
            return parent.GetLoadedOrderedEnumerable<TResult>();
        }
    }
}
