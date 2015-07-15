using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using GlobalsFramework.Access;
using GlobalsFramework.Linq.Helpers;
using GlobalsFramework.Utils.TypeConversion;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.ExpressionProcessing
{
    internal sealed class UnaryExpressionProcessor : IExpressionProcessor
    {
        public bool CanProcess(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus:
                    return IsPublicType(expression as UnaryExpression);
                default:
                    return false;
            }
        }

        public ProcessingResult ProcessExpression(Expression expression, List<NodeReference> references)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Convert:
                    return ProcessConvertExpression(expression as UnaryExpression, references);
                case ExpressionType.ConvertChecked:
                    return ProcessConvertExpression(expression as UnaryExpression, references, true);
                case ExpressionType.ArrayLength:
                    return ProcessArrayLengthExpression(expression as UnaryExpression, references);
                case ExpressionType.Negate:
                    return ProcessNegateExpression(expression as UnaryExpression, references);
                case ExpressionType.NegateChecked:
                    return ProcessNegateExpression(expression as UnaryExpression, references, true);
                case ExpressionType.Not:
                    return ProcessNotExpression(expression as UnaryExpression, references);
                case ExpressionType.TypeAs:
                    return ProcessTypeAsExpression(expression as UnaryExpression, references);
                case ExpressionType.UnaryPlus:
                    return ProcessUnaryPlusExpression(expression as UnaryExpression, references);
                default:
                    return ProcessingResult.Unsuccessful;
            }
        }

        private static ProcessingResult ProcessArrayLengthExpression(UnaryExpression expression, List<NodeReference> references)
        {
            var parentResult = ExpressionProcessingHelper.ProcessExpression(expression.Operand, references);
            if (!parentResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            if (parentResult.IsSingleItem)
            {
                var deferredResult = parentResult.GetDeferredItem();
                return deferredResult != null
                    ? new ProcessingResult(true, DatabaseManager.GetEnumerableCount(deferredResult), true)
                    : new ProcessingResult(true, (parentResult.Result as Array).Length, true);
            }

            var deferredItems = parentResult.GetDeferredItems();

            if (deferredItems != null)
            {
                var resultList = deferredItems.Select(DatabaseManager.GetEnumerableCount).ToList();
                return new ProcessingResult(true, resultList);
            }

            var resultItems = parentResult.GetItems();
            var result = new List<object>();

            foreach (var resultItem in resultItems)
            {
                var array = resultItem as Array;
                result.Add(array.Length);
            }

            return new ProcessingResult(true, result);
        }

        private static ProcessingResult ProcessConvertExpression(UnaryExpression expression, List<NodeReference> references, bool isChecked = false)
        {
            if (references.Count == 0)
                return new ProcessingResult(true, new ArrayList());

            var parentResult = ExpressionProcessingHelper.ProcessExpression(expression.Operand, references);
            if (!parentResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            if (parentResult.IsSingleItem)
            {
                var item = parentResult.GetLoadedItem(expression.Operand.Type);

                var result = !isChecked
                    ? TypeConverter.Instance.ConvertUnchecked(item, expression.Type)
                    : TypeConverter.Instance.ConvertChecked(item, expression.Type);

                return new ProcessingResult(true, result, true);
            }

            var items = parentResult.GetLoadedItems(expression.Operand.Type);
            var resultList = new ArrayList();

            foreach (var item in items)
                resultList.Add(!isChecked
                    ? TypeConverter.Instance.ConvertUnchecked(item, expression.Type)
                    : TypeConverter.Instance.ConvertChecked(item, expression.Type));

            return new ProcessingResult(true, resultList);
        }

        private static ProcessingResult ProcessNegateExpression(UnaryExpression expression, List<NodeReference> references, bool isChecked = false)
        {
            var operandResult = ExpressionProcessingHelper.ProcessExpression(expression.Operand, references);
            if (!operandResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            Func<dynamic, dynamic> negateMethod;

            if (!isChecked)
                negateMethod = (value) => unchecked (-value);
            else
                negateMethod = (value) => checked (-value);

            if (operandResult.IsSingleItem)
            {
                var value = operandResult.GetLoadedItem(expression.Operand.Type);
                return new ProcessingResult(true, negateMethod(value), true);
            }

            var values = operandResult.GetLoadedItems(expression.Operand.Type);
            var resultList = new List<object>();

            foreach (var value in values)
            {
                resultList.Add(negateMethod(value));
            }

            return new ProcessingResult(true, resultList);
        }

        private static ProcessingResult ProcessNotExpression(UnaryExpression expression, List<NodeReference> references)
        {
            var operandResult = ExpressionProcessingHelper.ProcessExpression(expression.Operand, references);
            if (!operandResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            Func<dynamic, bool> notOperatorMethod = (value) => !value;

            if (operandResult.IsSingleItem)
            {
                var value = operandResult.GetLoadedItem(expression.Operand.Type);
                return new ProcessingResult(true, notOperatorMethod(value), true);
            }

            var values = operandResult.GetLoadedItems(expression.Operand.Type);
            var resultList = new List<bool>();

            foreach (var value in values)
            {
                resultList.Add(notOperatorMethod(value));
            }

            return new ProcessingResult(true, resultList);
        }

        private static ProcessingResult ProcessTypeAsExpression(UnaryExpression expression, List<NodeReference> references)
        {
            var operandResult = ExpressionProcessingHelper.ProcessExpression(expression.Operand, references);
            if (!operandResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            if (operandResult.IsSingleItem)
            {
                var value = operandResult.GetLoadedItem(expression.Operand.Type);
                return new ProcessingResult(true, TypeConverter.Instance.TypeAsOperation(value, expression.Type), true);
            }

            var values = operandResult.GetLoadedItems(expression.Operand.Type);
            var resultList = new List<object>();

            foreach (var value in values)
            {
                resultList.Add(TypeConverter.Instance.TypeAsOperation(value, expression.Type));
            }

            return new ProcessingResult(true, resultList);
        }

        private static ProcessingResult ProcessUnaryPlusExpression(UnaryExpression expression, List<NodeReference> references)
        {
            var operandResult = ExpressionProcessingHelper.ProcessExpression(expression.Operand, references);
            if (!operandResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            Func<dynamic, dynamic> unaryPlusOperatorMethod = (value) => +value;

            if (operandResult.IsSingleItem)
            {
                var value = operandResult.GetLoadedItem(expression.Operand.Type);
                return new ProcessingResult(true, unaryPlusOperatorMethod(value), true);
            }

            var values = operandResult.GetLoadedItems(expression.Operand.Type);
            var resultList = new List<object>();

            foreach (var value in values)
            {
                resultList.Add(unaryPlusOperatorMethod(value));
            }

            return new ProcessingResult(true, resultList);
        }

        private static bool IsPublicType(UnaryExpression expression)
        {
            return expression.Operand.Type.IsPublic;
        }
    }
}
