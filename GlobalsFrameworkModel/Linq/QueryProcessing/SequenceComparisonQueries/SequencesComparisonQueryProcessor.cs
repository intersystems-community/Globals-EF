using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;

namespace GlobalsFramework.Linq.QueryProcessing.SequenceComparisonQueries
{
    internal abstract class SequencesComparisonQueryProcessor : IQueryProcessor
    {
        public abstract bool CanProcess(MethodCallExpression query);

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            var source1 = parentResult.GetLoadedItems(QueryProcessingHelper.GetSourceParameterType(query));

            var constantArgument = query.Arguments[1] as ConstantExpression;
            if (constantArgument == null)
                return ProcessingResult.Unsuccessful;

            var source2 = constantArgument.Value;
            var comparer = query.Arguments.Count > 2 ? ((ConstantExpression) query.Arguments[2]).Value : null;

            var sourceType = QueryProcessingHelper.GetSourceParameterType(query);

            var resolver = GetType()
                .GetMethod("ProcessQuery", BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(sourceType);

            return (ProcessingResult) resolver.Invoke(this, new[] {query, source1, source2, comparer});
        }

        protected abstract ProcessingResult ProcessQuery<TSource>(MethodCallExpression query, IEnumerable<TSource> first,
            IEnumerable<TSource> second, IEqualityComparer<TSource> comparer);
    }
}
