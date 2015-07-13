using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GlobalsFramework.Linq.Helpers;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.ExpressionProcessing
{
    internal sealed class NewArrayExpressionProcessor : IExpressionProcessor
    {
        public bool CanProcess(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.NewArrayBounds:
                case ExpressionType.NewArrayInit:
                    return true;
                default:
                    return false;
            }
        }

        public ProcessingResult ProcessExpression(Expression expression, List<NodeReference> references)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.NewArrayBounds:
                    return ProcessNewArrayBoundsExpression(expression as NewArrayExpression, references);
                case ExpressionType.NewArrayInit:
                    return ProcessNewArrayInitExpression(expression as NewArrayExpression, references);
                default:
                    return ProcessingResult.Unsuccessful;
            }
        }

        private static ProcessingResult ProcessNewArrayBoundsExpression(NewArrayExpression expression, List<NodeReference> references)
        {
            var dimensionsLengths = CallProcessingHelper.ProcessArguments(expression.Expressions, references);
            if (!dimensionsLengths.All(d => d.IsSuccess))
                return ProcessingResult.Unsuccessful;

            var arrayElementType = expression.Type.GetElementType();
            var elementTypeResult = new ProcessingResult(true, arrayElementType, true);

            return CallProcessingHelper.ProcessCall(dimensionsLengths, elementTypeResult, (elementType, lengths) =>
            {
                var type = elementType as Type;
                var dimensions = lengths.Cast<int>().ToArray();
                return type != null
                    ? new ProcessingResult(true, Array.CreateInstance(type, dimensions), true)
                    : ProcessingResult.Unsuccessful;
            });
        }

        private static ProcessingResult ProcessNewArrayInitExpression(NewArrayExpression expression, List<NodeReference> references)
        {
            var items = CallProcessingHelper.ProcessArguments(expression.Expressions, references);
            if (!items.All(i => i.IsSuccess))
                return ProcessingResult.Unsuccessful;

            var arrayElementType = expression.Type.GetElementType();
            var elementTypeResult = new ProcessingResult(true, arrayElementType, true);

            return CallProcessingHelper.ProcessCall(items, elementTypeResult, ArrayInitResolver);
        }

        private static ProcessingResult ArrayInitResolver(object elementType, object[] items)
        {
            var type = elementType as Type;
            if (type == null)
                return ProcessingResult.Unsuccessful;

            var arrayInstance = Array.CreateInstance(type, items.Length);

            for (var i = 0; i < items.Length; i++)
            {
                arrayInstance.SetValue(items[i], i);
            }

            return new ProcessingResult(true, arrayInstance, true);
        }
    }
}
