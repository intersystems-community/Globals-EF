using System;
using System.Collections;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal sealed class MathQueryProcessor : IQueryProcessor
    {
        public bool CanProcess(MethodCallExpression query)
        {
            switch (query.Method.Name)
            {
                case "Average":
                case "Max":
                case "Min":
                case "Sum":
                    return true;
                default:
                    return false;
            }
        }

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            if (!parentResult.IsDeferred())
                return ProcessingResult.Unsuccessful;

            var hasSelector = query.Arguments.Count > 1;
            var elementType = QueryProcessingHelper.GetReturnParameterType(query);
            var resolver = GetResolver(query);

            if (!hasSelector)
            {
                var items = parentResult.GetLoadedItems(elementType);
                return new ProcessingResult(true, resolver(items), true);
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
            return new ProcessingResult(true, resolver(loadedItems), true);
        }

        private static Func<IEnumerable, dynamic> GetResolver(MethodCallExpression query)
        {
            switch (query.Method.Name)
            {
                case "Average":
                    return AverageResolver;
                case "Max":
                    return MaxResolver;
                case "Min":
                    return MinResolver;
                case "Sum":
                    return SumResolver;
                    
                default:
                    throw new InvalidOperationException(string.Format("Unable to process {0} expression", query.Method.Name));
            }
        }

        private static dynamic AverageResolver(IEnumerable items)
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

        private static dynamic MaxResolver(IEnumerable items)
        {
            var enumerator = items.GetEnumerator();

            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Sequence contains no elements");

            var max = (dynamic)enumerator.Current;

            while (enumerator.MoveNext())
            {
                var current = (dynamic) enumerator.Current;

                if (current > max)
                    max = current;
            }

            return max;
        }

        private static dynamic MinResolver(IEnumerable items)
        {
            var enumerator = items.GetEnumerator();

            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Sequence contains no elements");

            var min = (dynamic)enumerator.Current;

            while (enumerator.MoveNext())
            {
                var current = (dynamic)enumerator.Current;

                if (current < min)
                    min = current;
            }

            return min;
        }

        private static dynamic SumResolver(IEnumerable items)
        {
            dynamic sum = 0;

            foreach (dynamic item in items)
            {
                sum += item;
            }

            return sum;
        }
    }
}
