using System.Collections.Generic;
using System.Linq.Expressions;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.ExpressionProcessing
{
    internal sealed class ConstantExpressionProcessor : IExpressionProcessor
    {
        public bool CanProcess(Expression expression)
        {
            return expression.NodeType == ExpressionType.Constant;
        }

        public ProcessingResult ProcessExpression(Expression expression, List<NodeReference> references)
        {
            var constantExpression = expression as ConstantExpression;

            return constantExpression != null
                ? new ProcessingResult(true, constantExpression.Value, true)
                : ProcessingResult.Unsuccessful;
        }
    }
}
