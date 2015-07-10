using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.ExpressionProcessing
{
    internal sealed class TypeIsExpressionProcessor : IExpressionProcessor
    {
        public bool CanProcess(Expression expression)
        {
            return expression.NodeType == ExpressionType.TypeIs;
        }

        public ProcessingResult ProcessExpression(Expression expression, List<NodeReference> references)
        {
            var typeBinaryExpression = expression as TypeBinaryExpression;
            if (typeBinaryExpression == null)
                return ProcessingResult.Unsuccessful;

            var result = PredicateExpressionProcessor.ProcessExpression(typeBinaryExpression.Expression, references);
            if (!result.IsSuccess)
                return ProcessingResult.Unsuccessful;

            if (result.IsSingleItem)
            {
                var objectType = result.GetLoadedItem(typeBinaryExpression.Expression.Type).GetType();
                return new ProcessingResult(true, TypeIs(objectType, typeBinaryExpression.TypeOperand), true);
            }

            if (result.IsDeferred())
            {
                var objectType = typeBinaryExpression.Expression.Type;
                var typeIsResult = TypeIs(objectType, typeBinaryExpression.TypeOperand);
                return new ProcessingResult(true, Enumerable.Repeat(typeIsResult, references.Count).ToList());
            }

            var resultList = new List<bool>();
            var loadedItems = result.GetLoadedItems(typeBinaryExpression.Expression.Type);

            foreach (var loadedItem in loadedItems)
            {
                resultList.Add(TypeIs(loadedItem.GetType(), typeBinaryExpression.TypeOperand));
            }

            return new ProcessingResult(true, resultList);
        }

        private static bool TypeIs(Type objectType, Type testType)
        {
            return testType.IsAssignableFrom(objectType);
        }
    }
}
