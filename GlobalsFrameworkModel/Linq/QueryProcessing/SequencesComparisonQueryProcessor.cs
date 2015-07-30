using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal sealed class SequencesComparisonQueryProcessor : IQueryProcessor
    {
        public bool CanProcess(MethodCallExpression query)
        {
            switch (query.Method.Name)
            {
                case "Except":
                case "Intersect":
                case "SequenceEqual":
                case "Union":
                    return true;
                default:
                    return false;
            }
        }

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
                .GetMethod("ResolveQuery", BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(sourceType);

            return (ProcessingResult) resolver.Invoke(null, new[] {query, source1, source2, comparer});
        }

        //ReSharper disable once UnusedMember.Local
        //Method is called via reflection
        private static ProcessingResult ResolveQuery<TSource>(MethodCallExpression query, IEnumerable<TSource> first,
            IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            switch (query.Method.Name)
            {
                case "Except":
                    return new ProcessingResult(true, first.Except(second, comparer));
                case "Intersect":
                    return new ProcessingResult(true, first.Intersect(second, comparer));
                case "SequenceEqual":
                    return new ProcessingResult(true, first.SequenceEqual(second, comparer), true);
                case "Union":
                    return new ProcessingResult(true, first.Union(second, comparer));
                default:
                    throw new InvalidOperationException(string.Format("Unanable to process {0} expression", query.Method.Name));
            }


        }
    }
}
