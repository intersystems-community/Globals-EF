using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal sealed class ContainsQueryProcessor:IQueryProcessor
    {
        public bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "Contains";
        }

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            var elementType = QueryProcessingHelper.GetSourceParameterType(query);

            var items = parentResult.GetLoadedItems(elementType);
            var value = ((ConstantExpression) query.Arguments[1]).Value;
            var comparer = query.Arguments.Count > 2 ? ((ConstantExpression)query.Arguments[2]).Value : null;

            var method = GetType().GetMethod("Contains", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(elementType);

            var result = method.Invoke(null, new[] {items, value, comparer});

            return new ProcessingResult(true, result, true);
        }

        //ReSharper disable once UnusedMember.Local
        //method is called via reflection
        private static bool Contains<T>(IEnumerable<T> items, T value, IEqualityComparer<T> comparer=null )
        {
            var targetComparer = comparer ?? EqualityComparer<T>.Default;
            return items.Any(item => targetComparer.Equals(item, value));
        }
    }
}
