using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;
using GlobalsFramework.Utils.RuntimeMethodInvocation;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal sealed class ContainsQueryProcessor : IQueryProcessor
    {
        public bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "Contains";
        }

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult, DataContext context)
        {
            var elementType = QueryProcessingHelper.GetSourceParameterType(query);

            var items = parentResult.GetLoadedItems(elementType);
            var value = ((ConstantExpression) query.Arguments[1]).Value;
            var comparer = query.Arguments.Count > 2 ? ((ConstantExpression)query.Arguments[2]).Value : null;

            var result = RuntimeMethodInvoker.InvokeFuncCached<IEnumerable<RuntimeType1>, RuntimeType1, IEqualityComparer<RuntimeType1>, bool>(
                    Contains, new RuntimeTypeBinding {new RuntimeType1(elementType)}, items, value, comparer);

            return new ProcessingResult(true, result, true);
        }

        private static bool Contains<T>(IEnumerable<T> items, T value, IEqualityComparer<T> comparer = null)
        {
            var targetComparer = comparer ?? EqualityComparer<T>.Default;
            return items.Any(item => targetComparer.Equals(item, value));
        }
    }
}
