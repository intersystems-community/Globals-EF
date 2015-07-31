using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;

namespace GlobalsFramework.Linq.QueryProcessing.SequenceComparisonQueries
{
    internal sealed class IntersectQueryProcessor : SequencesComparisonQueryProcessor
    {
        public override bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "Intersect";
        }

        protected override ProcessingResult ProcessQuery<TSource>(MethodCallExpression query, IEnumerable<TSource> first, IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer)
        {
            return new ProcessingResult(true, first.Intersect(second, comparer));
        }
    }
}
