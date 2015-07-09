using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GlobalsFramework.Linq.Helpers;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.ExpressionProcessing
{
    internal sealed class CallExpressionProcessor : IExpressionProcessor
    {
        public bool CanProcess(Expression expression)
        {
            return expression.NodeType == ExpressionType.Call;
        }

        public ProcessingResult ProcessExpression(Expression expression, List<NodeReference> references)
        {
            var callExpression = expression as MethodCallExpression;
            if (callExpression == null)
                return ProcessingResult.Unsuccessful;

            var processedArguments = CallProcessingHelper.ProcessArguments(callExpression.Arguments, references);
            if (processedArguments.Any(a => !a.IsSuccess))
                return ProcessingResult.Unsuccessful;

            var processedObject = ProcessObjectExpression(callExpression.Object, references);
            if (!processedObject.IsSuccess)
                return ProcessingResult.Unsuccessful;

            return CallProcessingHelper.ProcessCall(processedArguments, processedObject,
                (obj, arguments) => new ProcessingResult(true, callExpression.Method.Invoke(obj, arguments)));
        }

        private static ProcessingResult ProcessObjectExpression(Expression objectExpression, List<NodeReference> references)
        {
            if (objectExpression == null)
                return new ProcessingResult(true, null, true);

            var objectResult = PredicateExpressionProcessor.ProcessExpression(objectExpression, references);
            if (!objectResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            var value = objectResult.IsSingleItem
                ? objectResult.GetLoadedItem(objectExpression.Type)
                : objectResult.GetLoadedItems(objectExpression.Type);

            return new ProcessingResult(true, value, objectResult.IsSingleItem);
        }
    }
}
