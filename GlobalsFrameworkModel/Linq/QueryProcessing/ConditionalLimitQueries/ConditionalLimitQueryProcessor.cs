using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.QueryProcessing.ConditionalLimitQueries
{
    internal abstract class ConditionalLimitQueryProcessor : IQueryProcessor
    {
        public abstract bool CanProcess(MethodCallExpression query);

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            if (!parentResult.IsDeferred())
                return ProcessingResult.Unsuccessful;

            var predicateExpression = query.Arguments[1];

            var items = parentResult.GetItems();
            var enumerator = items.GetEnumerator();

            var resultList = new List<NodeReference>();

            while (enumerator.MoveNext())
            {
                var predicateResult = ExpressionProcessingHelper.ProcessPredicate(predicateExpression,
                    new List<NodeReference>(1) {(NodeReference) enumerator.Current});

                if (!predicateResult.IsSuccess)
                    return ProcessingResult.Unsuccessful;

                var predicateValue = ((IList<NodeReference>) predicateResult.Result).Any();

                if (predicateValue)
                {
                    ConditionTrueCallback(resultList, enumerator.Current);
                    continue;
                }

                ConditionFalseCallback(resultList, enumerator.Current);
                break;
            }

            while (enumerator.MoveNext())
                ConditionFalseCallback(resultList, enumerator.Current);

            return new ProcessingResult(true, resultList);
        }

        protected virtual void ConditionTrueCallback(IList resultItems, object item) { }

        protected virtual void ConditionFalseCallback(IList resultItems, object item) { }
    }
}
