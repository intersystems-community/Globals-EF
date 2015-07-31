using System;
using System.Collections;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;

namespace GlobalsFramework.Linq.QueryProcessing.SingleResultQueries
{
    internal abstract class SingleResultQueryProcessor : IQueryProcessor
    {
        public abstract bool CanProcess(MethodCallExpression query);

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            var hasPredicate = query.Arguments.Count == 2;

            if (!hasPredicate)
                return ProcessQuery(parentResult.GetItems().GetEnumerator(), QueryProcessingHelper.GetReturnParameterType(query));

            if (!parentResult.IsDeferred())
                return ProcessingResult.Unsuccessful;

            var predicate = query.Arguments[1];
            var predicateResult = ExpressionProcessingHelper.ProcessPredicate(predicate, parentResult.GetDeferredItems());

            return predicateResult.IsSuccess
                ? ProcessQuery(predicateResult.GetItems().GetEnumerator(), QueryProcessingHelper.GetReturnParameterType(query))
                : ProcessingResult.Unsuccessful;
        
        }

        protected abstract ProcessingResult ProcessQuery(IEnumerator enumerator, Type elementType);
    }
}
