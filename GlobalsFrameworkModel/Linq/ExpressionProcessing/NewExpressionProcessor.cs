using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GlobalsFramework.Linq.Helpers;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.ExpressionProcessing
{
    internal sealed class NewExpressionProcessor : IExpressionProcessor
    {
        public bool CanProcess(Expression expression)
        {
            return expression.NodeType == ExpressionType.New;
        }

        public ProcessingResult ProcessExpression(Expression expression, List<NodeReference> references)
        {
            var newExpression = expression as NewExpression;
            if (newExpression == null)
                return ProcessingResult.Unsuccessful;

            var argumentsResult = CallProcessingHelper.ProcessArguments(newExpression.Arguments, references);
            if (!argumentsResult.All(a => a.IsSuccess))
                return ProcessingResult.Unsuccessful;

            var constructorResult = new ProcessingResult(true, newExpression.Constructor, true);

            return CallProcessingHelper.ProcessCall(argumentsResult, constructorResult, (constructor, arguments) =>
            {
                var constructorInfo = constructor as ConstructorInfo;
                return constructorInfo != null
                    ? new ProcessingResult(true, constructorInfo.Invoke(arguments))
                    : ProcessingResult.Unsuccessful;
            });
        }
    }
}
