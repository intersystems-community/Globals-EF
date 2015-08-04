using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal sealed class WhereQueryProcessor : IQueryProcessor
    {
        public bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "Where";
        }

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult, DataContext context)
        {
            if (!parentResult.IsDeferred())
                return ProcessingResult.Unsuccessful;

            var predicateResult = ExpressionProcessingHelper.ProcessPredicate(query.Arguments[1], parentResult.GetDeferredItems(), context);
            return predicateResult.IsSuccess ? predicateResult : ProcessingResult.Unsuccessful;
        }
    }
}
