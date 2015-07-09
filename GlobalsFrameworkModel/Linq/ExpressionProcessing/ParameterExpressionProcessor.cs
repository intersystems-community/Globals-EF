using System.Collections.Generic;
using System.Linq.Expressions;
using GlobalsFramework.Extensions;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.ExpressionProcessing
{
    internal sealed class ParameterExpressionProcessor : IExpressionProcessor
    {
        public bool CanProcess(Expression expression)
        {
            return expression.NodeType == ExpressionType.Parameter;
        }

        public ProcessingResult ProcessExpression(Expression expression, List<NodeReference> references)
        {
            return new ProcessingResult(true, references.DeepCopy());
        }
    }
}
