using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GlobalsFramework.Linq.Helpers;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.ExpressionProcessing
{
    internal sealed class InvokeExpressionProcessor : IExpressionProcessor
    {
        public bool CanProcess(Expression expression)
        {
            return expression.NodeType == ExpressionType.Invoke;
        }

        public ProcessingResult ProcessExpression(Expression expression, List<NodeReference> references, DataContext context)
        {
            var invocationExpression = expression as InvocationExpression;
            if (invocationExpression == null)
                return ProcessingResult.Unsuccessful;

            var argumentsResults = CallProcessingHelper.ProcessArguments(invocationExpression.Arguments, references, context);
            if (!argumentsResults.All(r => r.IsSuccess))
                return ProcessingResult.Unsuccessful;

            var delegateResult = ExpressionProcessingHelper.ProcessExpression(invocationExpression.Expression, references, context);
            if (!delegateResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            return CallProcessingHelper.ProcessCall(argumentsResults, delegateResult, (del, arguments) =>
            {
                var delegateInstance = del as Delegate;
                return delegateInstance != null
                    ? new ProcessingResult(true, delegateInstance.DynamicInvoke(arguments))
                    : ProcessingResult.Unsuccessful;
            });
        }
    }
}
