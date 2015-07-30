using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;

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

            var method = GetType().GetMethod("Distinct", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(elementType);

            var result = method.Invoke(null, new[] { items, comparer });

            return new ProcessingResult(true, result, true);
        }

        //ReSharper disable once UnusedMember.Local
        //Method is called via reflection
        private static List<T> Distinct<T>(IEnumerable<T> items, IEqualityComparer<T> comparer = null)
        {
            return items.Distinct(comparer).ToList();
        }
    }
}
