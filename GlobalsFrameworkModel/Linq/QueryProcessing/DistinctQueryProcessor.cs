using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;
using GlobalsFramework.Utils.RuntimeMethodInvocation;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal sealed class DistinctQueryProcessor : IQueryProcessor
    {
        public bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "Distinct";
        }

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            var elementType = QueryProcessingHelper.GetSourceParameterType(query);

            var items = parentResult.GetLoadedItems(elementType);
            var comparer = query.Arguments.Count > 1 ? ((ConstantExpression)query.Arguments[1]).Value : null;

            var result = RuntimeMethodInvoker.InvokeFuncCached<IEnumerable<RuntimeType1>, IEqualityComparer<RuntimeType1>, List<RuntimeType1>>(
                    Distinct, new RuntimeTypeBinding {new RuntimeType1(elementType)}, items, comparer);

            return new ProcessingResult(true, result);
        }

        private static List<T> Distinct<T>(IEnumerable<T> items, IEqualityComparer<T> comparer = null)
        {
            return items.Distinct(comparer).ToList();
        }
    }
}
