using System;
using System.Collections;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;

namespace GlobalsFramework.Linq.QueryProcessing.MathQueries
{
    internal abstract class MathQueryProcessor : IQueryProcessor
    {
        public abstract bool CanProcess(MethodCallExpression query);

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult, DataContext context)
        {
            if (!parentResult.IsDeferred())
                return ProcessingResult.Unsuccessful;

            var hasSelector = query.Arguments.Count > 1;
            var elementType = QueryProcessingHelper.GetReturnParameterType(query);

            if (!hasSelector)
            {
                var items = parentResult.GetLoadedItems(elementType);
                return new ProcessingResult(true, ProcessQuery(items), true);
            }

            var unaryExpression = (UnaryExpression) query.Arguments[1];
            var selectorLambda = (LambdaExpression)unaryExpression.Operand;

            var selectorResult = ExpressionProcessingHelper.ProcessExpression(selectorLambda.Body, parentResult.GetDeferredList(), context);

            if (!selectorResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            if (selectorResult.IsSingleItem)
            {
                var result = Convert.ChangeType(selectorResult.Result, elementType);
                return new ProcessingResult(true, result, true);
            }

            var loadedItems = selectorResult.GetLoadedItems(elementType);
            return new ProcessingResult(true, ProcessQuery(loadedItems), true);
        }

        protected abstract object ProcessQuery(IEnumerable items);
    }
}
