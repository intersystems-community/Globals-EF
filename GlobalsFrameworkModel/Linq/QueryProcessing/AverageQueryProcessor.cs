using System;
using System.Collections;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal sealed class AverageQueryProcessor : IQueryProcessor
    {
        public bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "Average";
        }

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            if (!parentResult.IsDeferred())
                return ProcessingResult.Unsuccessful;

            var hasSelector = query.Arguments.Count > 1;
            var elementType = QueryProcessingHelper.GetReturnParameterType(query);

            if (!hasSelector)
            {
                var items = parentResult.GetLoadedItems(elementType);
                return new ProcessingResult(true, GetAverage(items), true);
            }

            var unaryExpression = (UnaryExpression) query.Arguments[1];
            var selectorLambda = (LambdaExpression)unaryExpression.Operand;

            var selectorResult = ExpressionProcessingHelper.ProcessExpression(selectorLambda.Body, parentResult.GetDeferredList());

            if (!selectorResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            if (selectorResult.IsSingleItem)
            {
                var result = Convert.ChangeType(selectorResult.Result, elementType);
                return new ProcessingResult(true, result, true);
            }

            var loadedItems = selectorResult.GetLoadedItems(elementType);
            return new ProcessingResult(true, GetAverage(loadedItems), true);
        }

        private static dynamic GetAverage(IEnumerable items)
        {
            dynamic sum = 0;
            var count = 0;

            foreach (dynamic item in items)
            {
                sum += item;
                count++;
            }

            if (count == 0)
                throw new InvalidOperationException("Sequence contains no elements");

            return (sum*1.0)/count;
        }
    }
}
