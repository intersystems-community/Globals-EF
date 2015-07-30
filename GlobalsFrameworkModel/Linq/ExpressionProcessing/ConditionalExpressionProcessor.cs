using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GlobalsFramework.Linq.Helpers;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.ExpressionProcessing
{
    internal sealed class ConditionalExpressionProcessor : IExpressionProcessor
    {
        public bool CanProcess(Expression expression)
        {
            return expression.NodeType == ExpressionType.Conditional;
        }

        public ProcessingResult ProcessExpression(Expression expression, List<NodeReference> references)
        {
            var conditionalExpression = expression as ConditionalExpression;
            if (conditionalExpression == null)
                return ProcessingResult.Unsuccessful;

            var testResult = ExpressionProcessingHelper.ProcessExpression(conditionalExpression.Test, references);
            if (!testResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            if (testResult.IsSingleItem)
            {
                var testValue = (bool) testResult.GetLoadedItem(typeof (bool));
                var resultExpression = testValue ? conditionalExpression.IfTrue : conditionalExpression.IfFalse;
                return ExpressionProcessingHelper.ProcessExpression(resultExpression, references);
            }

            var resultValues = testResult.GetLoadedItems(typeof (bool));
            var results = new List<object>();

            var index = 0;
            foreach (var resultValue in resultValues)
            {
                var resultExpression = (bool) resultValue ? conditionalExpression.IfTrue : conditionalExpression.IfFalse;

                var processingResult = ExpressionProcessingHelper.ProcessExpression(resultExpression, new List<NodeReference>(1) {references[index]});
                if (!processingResult.IsSuccess)
                    return ProcessingResult.Unsuccessful;

                results.Add(ResolveValue(processingResult, resultExpression.Type));
                index++;
            }

            return new ProcessingResult(true, results);
        }

        private static object ResolveValue(ProcessingResult processingResult, Type resultType)
        {
            if (processingResult.IsSingleItem)
                return processingResult.GetLoadedItem(resultType);

            var values = processingResult.GetLoadedItems(resultType);
            foreach (var item in values)
                return item;

            throw new InvalidOperationException("Value can not be resolved");
        }
    }
}
