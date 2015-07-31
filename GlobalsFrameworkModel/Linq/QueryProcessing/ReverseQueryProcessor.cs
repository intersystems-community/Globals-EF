using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal sealed class ReverseQueryProcessor : IQueryProcessor
    {
        public bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "Reverse";
        }

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            var itemType = parentResult.IsDeferred()
                ? typeof(NodeReference)
                : QueryProcessingHelper.GetSourceParameterType(query);

            var reverseMethod = GetType()
                .GetMethod("Reverse", BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(itemType);

            var result = reverseMethod.Invoke(null, new[] {parentResult.Result});
            return new ProcessingResult(true, result);

        }

        //ReSharper disable once UnusedMember.Local
        //Method is used via reflection
        private static IEnumerable<TSource> Reverse<TSource>(IEnumerable<TSource> source)
        {
            return source.Reverse();
        }
    }
}
