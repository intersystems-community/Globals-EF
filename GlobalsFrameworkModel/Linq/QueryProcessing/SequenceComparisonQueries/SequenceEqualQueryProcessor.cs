using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;

namespace GlobalsFramework.Linq.QueryProcessing.SequenceComparisonQueries
{
    internal sealed class SequenceEqualQueryProcessor : SequencesComparisonQueryProcessor
    {
        public override bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "SequenceEqual";
        }

        protected override ProcessingResult ProcessQuery<TSource>(MethodCallExpression query, IEnumerable<TSource> first, IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer)
        {
            return new ProcessingResult(true, first.SequenceEqual(second, comparer), true);
        }
    }
}
