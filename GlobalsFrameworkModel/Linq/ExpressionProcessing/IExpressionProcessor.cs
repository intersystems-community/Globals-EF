using System.Collections.Generic;
using System.Linq.Expressions;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.ExpressionProcessing
{
    internal interface IExpressionProcessor
    {
        bool CanProcess(Expression expression);
        ProcessingResult ProcessExpression(Expression expression, List<NodeReference> references);
    }
}
