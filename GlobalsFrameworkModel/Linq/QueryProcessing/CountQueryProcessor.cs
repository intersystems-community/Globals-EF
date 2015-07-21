using System.Collections;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal sealed class CountQueryProcessor : IQueryProcessor
    {
        public bool CanProcess(MethodCallExpression query)
        {
            switch (query.Method.Name)
            {
                case "Count":
                case "LongCount":
                case "Any":
                    return true;
                default:
                    return false;
            }
        }

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            var hasPredicate = query.Arguments.Count == 2;

            if (!hasPredicate)
                return ProcessTargetQuery(query, parentResult);

            if (!parentResult.IsDeferred())
                return ProcessingResult.Unsuccessful;

            var predicate = query.Arguments[1];
            var predicateResult = ExpressionProcessingHelper.ProcessPredicate(predicate, parentResult.GetDeferredItems());

            return predicateResult.IsSuccess ? ProcessTargetQuery(query, predicateResult) : ProcessingResult.Unsuccessful;
        }

        private static ProcessingResult ProcessTargetQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            var items = parentResult.GetItems();

            if (items == null)
                return ProcessingResult.Unsuccessful;

            switch (query.Method.Name)
            {
                case "Count":
                    return ProcessCount(items.GetEnumerator());
                case "LongCount":
                    return ProcessLongCount(items.GetEnumerator());
                case "Any":
                    return ProcessAny(items.GetEnumerator());
                default:
                    return ProcessingResult.Unsuccessful;
            }
        }

        private static ProcessingResult ProcessCount(IEnumerator enumerator)
        {
            var count = 0;

            while (enumerator.MoveNext())
                count++;

            return new ProcessingResult(true, count, true);
        }

        private static ProcessingResult ProcessLongCount(IEnumerator enumerator)
        {
            long longCount = 0;

            while (enumerator.MoveNext())
                longCount++;

            return new ProcessingResult(true, longCount, true);
        }

        private static ProcessingResult ProcessAny(IEnumerator enumerator)
        {
            return new ProcessingResult(true, enumerator.MoveNext(), true);
        }
    }
}
