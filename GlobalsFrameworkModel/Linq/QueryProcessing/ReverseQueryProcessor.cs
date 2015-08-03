using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;
using GlobalsFramework.Utils.RuntimeMethodInvocation;
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

            var result = RuntimeMethodInvoker.InvokeFuncCached<IEnumerable<RuntimeType1>, IEnumerable<RuntimeType1>>(
                Reverse, new RuntimeTypeBinding {new RuntimeType1(itemType)}, parentResult.Result);

            return new ProcessingResult(true, result);
        }

        private static IEnumerable<TSource> Reverse<TSource>(IEnumerable<TSource> source)
        {
            return source.Reverse();
        }
    }
}
