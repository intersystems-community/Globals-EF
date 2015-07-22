using System.Linq;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal sealed class AllQueryProcessor : IQueryProcessor
    {
        public bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "All";
        }

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            if (!parentResult.IsDeferred())
                return ProcessingResult.Unsuccessful;

            var predicate = query.Arguments[1];
            var predicateResult = ExpressionProcessingHelper.ProcessPredicate(predicate, parentResult.GetDeferredItems());

            if (!predicateResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            var parentResultCount = parentResult.GetDeferredItems().Count();
            var predicateResultCount = predicateResult.GetDeferredItems().Count();

            var result = (parentResultCount == predicateResultCount);

            return new ProcessingResult(true, result, true);
        }
    }
}
