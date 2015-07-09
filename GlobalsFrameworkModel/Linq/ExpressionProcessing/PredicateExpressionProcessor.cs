using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.ExpressionProcessing
{
    internal static class PredicateExpressionProcessor
    {
        internal static readonly List<IExpressionProcessor> ExpressionProcessors; 

        static PredicateExpressionProcessor()
        {
            ExpressionProcessors = new List<IExpressionProcessor>
            {
                new BinaryExpressionProcessor(),
                new CallExpressionProcessor(),
                new ConstantExpressionProcessor(),
                new MemberExpressionProcessor(),
                new ParameterExpressionProcessor(),
                new UnaryExpressionProcessor(),
                new ConditionalExpressionProcessor(),
                new InvokeExpressionProcessor()
            };
        }

        internal static ProcessingResult ProcessPredicate(Expression predicateExpression, IEnumerable<NodeReference> references)
        {
            var unaryExpression = predicateExpression as UnaryExpression;

            if (unaryExpression == null)
                return ProcessingResult.Unsuccessful;

            var lambdaExpression = unaryExpression.Operand as LambdaExpression;

            return (lambdaExpression != null)
                ? ProcessPredicateInternal(lambdaExpression.Body, references.ToList())
                : ProcessingResult.Unsuccessful;
        }

        internal static ProcessingResult ProcessExpression(Expression expression, List<NodeReference> references)
        {
            foreach (var expressionProcessor in ExpressionProcessors)
            {
                if (expressionProcessor.CanProcess(expression))
                    return expressionProcessor.ProcessExpression(expression, references);
            }

            return ProcessingResult.Unsuccessful;
        }

        private static ProcessingResult ProcessPredicateInternal(Expression predicateExpression, List<NodeReference> references)
        {
            var resultReferences = new List<NodeReference>();

            var processingResult = ProcessExpression(predicateExpression, references);
            if (!processingResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            if (processingResult.IsSingleItem)
            {
                var boolValue = processingResult.GetLoadedItem(predicateExpression.Type) as bool?;

                if (!boolValue.HasValue)
                    return ProcessingResult.Unsuccessful;

                if (boolValue.Value)
                    resultReferences = references;
                return new ProcessingResult(true, resultReferences, true);
            }

            var resultList = processingResult.GetLoadedItems(predicateExpression.Type);

            if (resultList == null)
                return ProcessingResult.Unsuccessful;

            var index = 0;

            foreach (var item in resultList)
            {
                var boolItemValue = item as bool?;

                if (!boolItemValue.HasValue)
                    return ProcessingResult.Unsuccessful;

                if (boolItemValue.Value)
                    resultReferences.Add(references[index]);

                index++;
            }

            return new ProcessingResult(true, resultReferences);
        }
    }
}
