using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GlobalsFramework.Access;
using GlobalsFramework.Linq.Helpers;
using GlobalsFramework.Utils.TypeDescription;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.ExpressionProcessing
{
    internal sealed class BinaryExpressionProcessor : IExpressionProcessor
    {
        private static readonly Dictionary<ExpressionType, Func<dynamic, dynamic, object>> BinaryOperationHandler;

        static BinaryExpressionProcessor()
        {
            BinaryOperationHandler = new Dictionary<ExpressionType, Func<dynamic, dynamic, object>>();
            InitBinaryOperationHandler();
        }

        public bool CanProcess(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:

                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:

                case ExpressionType.Add:
                case ExpressionType.AddChecked:

                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:

                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:

                case ExpressionType.LeftShift:
                case ExpressionType.RightShift:

                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.ArrayIndex:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.Coalesce:

                case ExpressionType.And:
                case ExpressionType.AndAlso:

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return IsPublicTypes(expression as BinaryExpression);
                default:
                    return false;
            }
        }

        public ProcessingResult ProcessExpression(Expression expression, List<NodeReference> references, DataContext context)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.ArrayIndex:
                    return ProcessArrayIndexExpression(expression as BinaryExpression, references, context);
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    return ProcessShortCircuitingExpression(expression as BinaryExpression, references, expression.NodeType, context);
                default:
                    return ProcessExpressionByDefault(expression as BinaryExpression, references, context);
            }
        }

        private static ProcessingResult ProcessArrayIndexExpression(BinaryExpression expression, List<NodeReference> references, DataContext context)
        {
            var arrayResult = ExpressionProcessingHelper.ProcessExpression(expression.Left, references, context);
            if (!arrayResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            var indexResult = ExpressionProcessingHelper.ProcessExpression(expression.Right, references, context);
            if (!indexResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            var processingResult = IterateNodesItems(arrayResult, indexResult, ApplyArrayIndex);
            var resultData = processingResult.Result;

            if (!arrayResult.IsDeferred())
                return new ProcessingResult(processingResult.IsSuccess, resultData, processingResult.IsSingleItem);

            var deferredList = processingResult.GetItems();
            if (deferredList != null)
                resultData = deferredList.Cast<NodeReference>().ToList();

            return new ProcessingResult(processingResult.IsSuccess, resultData, processingResult.IsSingleItem);

        }

        private static ProcessingResult ProcessShortCircuitingExpression(BinaryExpression expression,
            List<NodeReference> references, ExpressionType operationType, DataContext context)
        {
            if (!(expression.Left.Type == typeof (bool) && expression.Right.Type == typeof (bool)))
                return ProcessExpressionByDefault(expression, references, context);

            var leftResult = ExpressionProcessingHelper.ProcessExpression(expression.Left, references, context);
            if (!leftResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            var operationResult = IterateNodesItems(leftResult, new ProcessingResult(true, references),
                (left, right) =>
                    PerformShortCircuitingOperation((bool) left, expression.Right, (NodeReference) right, operationType, context));

            var resultList = new List<object>();

            if (operationResult.IsSingleItem)
                return new ProcessingResult(true, (bool) operationResult.Result, true);

            foreach (ProcessingResult item in operationResult.GetItems())
            {
                if (!item.IsSuccess)
                    return ProcessingResult.Unsuccessful;
                resultList.Add((bool) item.Result);
            }

            return new ProcessingResult(true, resultList);

        }

        private static ProcessingResult ProcessExpressionByDefault(BinaryExpression expression, List<NodeReference> references, DataContext context)
        {
            var leftResult = ExpressionProcessingHelper.ProcessExpression(expression.Left, references, context);
            if (!leftResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            var rightResult = ExpressionProcessingHelper.ProcessExpression(expression.Right, references, context);
            if (!rightResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            var leftDataType = expression.Left.Type;
            var rightDataType = expression.Right.Type;

            return IterateNodesItems(leftResult, rightResult,
                (left, right) => PerformBinaryOperation(left, leftDataType, right, rightDataType, expression.NodeType));
        }

        private static ProcessingResult IterateNodesItems(ProcessingResult leftNode, ProcessingResult rightNode, Func<object, object, object> func)
        {
            var result = new List<object>();

            //left and right nodes are not enumerable
            if ((leftNode.IsSingleItem) && (rightNode.IsSingleItem))
            {
                var leftData = leftNode.Result;
                var rightData = rightNode.Result;
                return new ProcessingResult(true, func(leftData, rightData), true);
            }

            //right node is enumerable only
            if (leftNode.IsSingleItem)
            {
                var leftData = leftNode.Result;
                var rightData = rightNode.GetItems();
                foreach (var rightItem in rightData)
                    result.Add(func(leftData, rightItem));
            }

            //left node is enumerable only
            else if (rightNode.IsSingleItem)
            {
                var leftData = leftNode.GetItems();
                var rightData = rightNode.Result;
                foreach (var leftItem in leftData)
                    result.Add(func(leftItem, rightData));
            }

            //left and right nodes are enumerable
            else
            {
                var leftData = leftNode.GetItems();
                var rightData = rightNode.GetItems();
                var rightEnumerator = rightData.GetEnumerator();
                rightEnumerator.MoveNext();

                foreach (var leftItem in leftData)
                {
                    var rightItem = rightEnumerator.Current;
                    result.Add(func(leftItem, rightItem));
                    rightEnumerator.MoveNext();
                }
            }

            return new ProcessingResult(true, result);
        }

        private static void InitBinaryOperationHandler()
        {
            BinaryOperationHandler.Add(ExpressionType.Equal, (a, b) => a == b);
            BinaryOperationHandler.Add(ExpressionType.NotEqual, (a, b) => a != b);

            BinaryOperationHandler.Add(ExpressionType.LessThan, (a, b) => a < b);
            BinaryOperationHandler.Add(ExpressionType.LessThanOrEqual, (a, b) => a <= b);
            BinaryOperationHandler.Add(ExpressionType.GreaterThan, (a, b) => a > b);
            BinaryOperationHandler.Add(ExpressionType.GreaterThanOrEqual, (a, b) => a >= b);

            BinaryOperationHandler.Add(ExpressionType.Add, (a, b) => a + b);
            BinaryOperationHandler.Add(ExpressionType.AddChecked, (a, b) => checked (a + b));

            BinaryOperationHandler.Add(ExpressionType.Subtract, (a, b) => a - b);
            BinaryOperationHandler.Add(ExpressionType.SubtractChecked, (a, b) => checked (a - b));

            BinaryOperationHandler.Add(ExpressionType.Multiply, (a, b) => a * b);
            BinaryOperationHandler.Add(ExpressionType.MultiplyChecked, (a, b) => checked (a * b));

            BinaryOperationHandler.Add(ExpressionType.LeftShift, (a, b) => a << b);
            BinaryOperationHandler.Add(ExpressionType.RightShift, (a, b) => a >> b);

            BinaryOperationHandler.Add(ExpressionType.Divide, (a, b) => a / b);
            BinaryOperationHandler.Add(ExpressionType.Modulo, (a, b) => a % b);
            BinaryOperationHandler.Add(ExpressionType.ExclusiveOr, (a, b) => a ^ b);
            BinaryOperationHandler.Add(ExpressionType.Coalesce, (a, b) => a ?? b);

            BinaryOperationHandler.Add(ExpressionType.And, (a, b) => a & b);
            BinaryOperationHandler.Add(ExpressionType.AndAlso, (a, b) => a && b);

            BinaryOperationHandler.Add(ExpressionType.Or, (a, b) => a | b);
            BinaryOperationHandler.Add(ExpressionType.OrElse, (a, b) => a || b);
        }

        private static object PerformBinaryOperation(object left, Type leftDataType, object right,
            Type rightDataType, ExpressionType nodeType)
        {
            var leftNode = left as NodeReference;
            var rightNode = right as NodeReference;

            var leftData = (leftNode == null) ? left : DatabaseManager.ReadNode(leftNode, leftDataType);
            var rightData = (rightNode == null) ? right : DatabaseManager.ReadNode(rightNode, rightDataType);

            if ((EntityTypeDescriptor.IsNullableType(leftDataType) || EntityTypeDescriptor.IsNullableType(rightDataType)) &&
                (leftData == null || rightData == null))
            {
                return ResolveNullableOperation(leftData, rightData, nodeType);
            }

            return BinaryOperationHandler[nodeType](leftData, rightData);
        }

        private static object ResolveNullableOperation(object leftData, object rightData, ExpressionType operationType)
        {
            switch (operationType)
            {
                case ExpressionType.Equal:
                    return leftData == rightData;
                case ExpressionType.NotEqual:
                    return leftData != rightData;

                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                    return false;

                case ExpressionType.Coalesce:
                    return leftData ?? rightData;

                default:
                    return null;
            }
        }

        private static object ApplyArrayIndex(object arrayData, object indexData)
        {
            var node = arrayData as NodeReference;
            if (node != null)
            {
                DatabaseManager.NavigateToIndex(node, Convert.ToInt32(indexData));
                return node;
            }

            var array = arrayData as Array;
            //ReSharper disable once PossibleNullReferenceException
            //arrayData ia always instance of Array for ArrayIndex Expression where not deferred
            return array.GetValue(Convert.ToInt32(indexData));
        }

        private static ProcessingResult PerformShortCircuitingOperation(bool left, Expression rightExpression,
            NodeReference reference, ExpressionType operationType, DataContext context)
        {
            if ((operationType == ExpressionType.AndAlso) && (!left))
                return new ProcessingResult(true, false, true);

            if ((operationType == ExpressionType.OrElse) && (left))
                return new ProcessingResult(true, true, true);

            var rightResult = ExpressionProcessingHelper.ProcessExpression(rightExpression,
                new List<NodeReference>(1) {reference}, context);
            if (!rightResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            if (rightResult.IsSingleItem)
                return new ProcessingResult(true, (bool) rightResult.Result, true);

            var rightValues = rightResult.GetLoadedItems(typeof (bool));

            foreach (var rightValue in rightValues)
                return new ProcessingResult(true, (bool) rightValue, true);

            return ProcessingResult.Unsuccessful;
        }

        private static bool IsPublicTypes(BinaryExpression expression)
        {
            return expression.Left.Type.IsPublic && expression.Right.Type.IsPublic;
        }
    }
}
