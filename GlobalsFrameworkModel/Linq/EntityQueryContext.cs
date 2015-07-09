using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GlobalsFramework.Access;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;
using GlobalsFramework.Utils.TypeDescription;
using InterSystems.Globals;

namespace GlobalsFramework.Linq
{
    internal static class EntityQueryContext
    {
        private static readonly Dictionary<string, ExpressionHandler>
            OptimizedHandlers = new Dictionary<string, ExpressionHandler>
            {
                {"Any", ProcessCountExpression},
                {"Count", ProcessCountExpression},
                {"ElementAt", ProcessElementAt},
                {"ElementAtOrDefault", ProcessElementAt},
                {"First", ProcessSelectionExpression},
                {"FirstOrDefault", ProcessSelectionExpression},
                {"Select", ProcessSelectExpression},
                {"Single", ProcessSelectionExpression},
                {"SingleOrDefault", ProcessSelectionExpression},
                {"Last", ProcessSelectionExpression},
                {"LastOrDefault", ProcessSelectionExpression},
                {"LongCount", ProcessCountExpression},
                {"Take", ProcessSkipExpression},
                {"Skip", ProcessSkipExpression},
                {"Where", ProcessWhereExpression}
            };

        private delegate object ExpressionHandler(
            MethodCallExpression expression, object source, ref bool isDeferred);

        internal static object Execute(NodeReference node, DataContext context, Expression expression)
        {
            var expressions = GetExpressionsInTurn(expression);
            var isDeferred = true;
            var nodes = DatabaseManager.GetEntitiesNodes(node, context.GetConnection());

            var result = ProcessExpressions(expressions, nodes, ref isDeferred);

            return result;
        }

        private static IEnumerable<MethodCallExpression> GetExpressionsInTurn(Expression expression)
        {
            var result = new List<MethodCallExpression>();

            var mcall = expression as MethodCallExpression;

            while (mcall != null && mcall.Arguments.Count > 0)
            {
                result.Add(mcall);
                mcall = mcall.Arguments[0] as MethodCallExpression;
            }

            result.Reverse();

            return result;
        }

        private static object ProcessExpressions(IEnumerable<MethodCallExpression> expressions, IEnumerable<NodeReference> nodes, ref bool isDeferred)
        {
            object source = nodes;

            foreach (var expression in expressions)
            {
                source = OptimizedHandlers.ContainsKey(expression.Method.Name)
                    ? OptimizedHandlers[expression.Method.Name](expression, source, ref isDeferred)
                    : ProcessExpressionByDefault(expression, source, ref isDeferred);
            }
            
            if (isDeferred)
            {
                var elementType = ExpressionHelper.GetReturnParameterType(expressions.Last());
                source = GetActualValues(source as IEnumerable<NodeReference>, elementType);
            }

            return source;
        }

        private static object ProcessExpressionByDefault(MethodCallExpression expression, object source, ref bool isDeferred)
        {
            if (isDeferred)
            {
                var elementType = ExpressionHelper.GetSourceParameterType(expression);
                source = GetActualValues(source as IEnumerable<NodeReference>, elementType);
                isDeferred = false;
            }

            return ExpressionHelper.InvokeLinqExpression(expression, source);
        }

        private static object ProcessSelectionExpression(MethodCallExpression expression, object source, ref bool isDeferred)
        {
            //check for expression does not contains predicate
            if (!isDeferred || expression.Arguments.Count > 1)
                return ProcessExpressionByDefault(expression, source, ref isDeferred);

            var nodes = (source as List<NodeReference>);

            if (nodes == null)
                throw new InvalidOperationException();

            NodeReference expectedNode = null;

            switch (expression.Method.Name)
            {
                case "First":
                    expectedNode = nodes.First();
                    break;
                case "FirstOrDefault":
                    expectedNode = nodes.FirstOrDefault();
                    break;
                case "Single":
                    expectedNode = nodes.Single();
                    break;
                case "SingleOrDefault":
                    expectedNode = nodes.SingleOrDefault();
                    break;
                case "Last":
                    expectedNode = nodes.Last();
                    break;
                case "LastOrDefault":
                    expectedNode = nodes.LastOrDefault();
                    break;
            }

            isDeferred = false;
            return expectedNode == null ? null : GetActualValue(expectedNode, ExpressionHelper.GetSourceParameterType(expression));
        }

        private static object ProcessCountExpression(MethodCallExpression expression, object source, ref bool isDeferred)
        {
            //check for expression does not contains predicate
            if (!isDeferred || expression.Arguments.Count > 1)
                return ProcessExpressionByDefault(expression, source, ref isDeferred);

            var nodes = (source as List<NodeReference>);

            if (nodes == null)
                throw new InvalidOperationException();

            isDeferred = false;

            switch (expression.Method.Name)
            {
                case "Any":
                    return nodes.Count > 0;
                case "Count":
                    return nodes.Count;
                case "LongCount":
                    return nodes.LongCount();
            }

            throw new InvalidOperationException();
        }

        private static object ProcessElementAt(MethodCallExpression expression, object source, ref bool isDeferred)
        {
            //Second argument is always ConstantExpression for this linq command
            // ReSharper disable once PossibleNullReferenceException
            var index = Convert.ToInt32((expression.Arguments[1] as ConstantExpression).Value);

            var items = source as IEnumerable<object>;

            object element = null;

            switch (expression.Method.Name)
            {
                case "ElementAt":
                    element = items.ElementAt(index);
                    break;
                case "ElementAtOrDefault":
                    element = items.ElementAtOrDefault(index);
                    break;
            }

            if (element == null)
            {
                isDeferred = false;
                return null;
            }

            var result = isDeferred
                ? GetActualValue((NodeReference) element, ExpressionHelper.GetSourceParameterType(expression))
                : element;

            isDeferred = false;
            return result;
        }

        private static object ProcessSelectExpression(MethodCallExpression expression, object source, ref bool isDeferred)
        {
            if (!isDeferred)
                return ProcessExpressionByDefault(expression, source, ref isDeferred);

            //select predicate is always UnaryExpression
            // ReSharper disable once PossibleNullReferenceException
            var lambda = (expression.Arguments[1] as UnaryExpression).Operand as LambdaExpression;

            if (lambda == null)
                return ProcessExpressionByDefault(expression, source, ref isDeferred);

            var isArrayLengthExpression = lambda.Body.NodeType == ExpressionType.ArrayLength;
            var isMemberExpression = lambda.Body.NodeType == ExpressionType.MemberAccess || lambda.Body.NodeType == ExpressionType.Parameter;

            if (!isArrayLengthExpression && !isMemberExpression)
                return ProcessExpressionByDefault(expression, source, ref isDeferred);

            //null reference exception is impossible in this case
            // ReSharper disable once PossibleNullReferenceException
            var memberAccessExpression = (isArrayLengthExpression ? (lambda.Body as UnaryExpression).Operand : lambda.Body);

            if (!ExpressionHelper.IsFullyMemberAccessExpression(memberAccessExpression))
                return ProcessExpressionByDefault(expression, source, ref isDeferred);

            var membersChain = ExpressionHelper.GetMembersChain(memberAccessExpression);
            var parameterType = ExpressionHelper.GetSourceParameterType(expression);

            bool hasCountRequest;

            if (!IsRequestedOnlyColumns(membersChain, parameterType, out hasCountRequest))
                return ProcessExpressionByDefault(expression, source, ref isDeferred);

            AppendMembersPath(source as List<NodeReference>, membersChain, parameterType);

            if (isArrayLengthExpression || hasCountRequest)
            {
                isDeferred = false;
                return (source as List<NodeReference>).Select(DatabaseManager.GetEnumerableCount);
            }

            return source;
        }

        private static object ProcessSkipExpression(MethodCallExpression expression, object source, ref bool isDeferred)
        {
            if (!isDeferred)
                return ProcessExpressionByDefault(expression, source, ref isDeferred);

            //Second argument is always ConstantExpression for this linq command
            // ReSharper disable once PossibleNullReferenceException
            var count = Convert.ToInt32((expression.Arguments[1] as ConstantExpression).Value);

            var items = source as List<NodeReference>;

            switch (expression.Method.Name)
            {
                case "Take":
                    return items.Take(count);
                case "Skip":
                    return items.Skip(count);
            }

            throw new InvalidOperationException();
        }

        private static object ProcessWhereExpression(MethodCallExpression expression, object source, ref bool isDeferred)
        {
            if (!isDeferred)
                return ProcessExpressionByDefault(expression, source, ref isDeferred);

            var processingResult = PredicateExpressionProcessor.ProcessPredicate(expression.Arguments[1], source as IEnumerable<NodeReference>);
            return processingResult.IsSuccess
                ? processingResult.Result
                : ProcessExpressionByDefault(expression, source, ref isDeferred);
        }

        private static object GetActualValues(IEnumerable<NodeReference> nodes, Type elementType)
        {
            return DatabaseManager.ReadNodes(nodes, elementType);
        }

        private static object GetActualValue(NodeReference node, Type elementType)
        {
            return DatabaseManager.ReadNode(node, elementType);
        }

        private static bool IsRequestedOnlyColumns(List<MemberInfo> membersChain, Type parameterType,out bool hasCountRequest)
        {
            var description = EntityTypeDescriptor.GetTypeDescription(parameterType);
            hasCountRequest = false;

            var lastElement = membersChain.LastOrDefault();
            if (lastElement != null && lastElement.Name == "Count" && EntityTypeDescriptor.IsSupportedEnumerable(lastElement.ReflectedType))
                hasCountRequest = true;

            if (hasCountRequest)
                membersChain.Remove(membersChain.Last());

            foreach (var member in membersChain)
            {
                description = description.Columns.SingleOrDefault(d => d.ColumnInfo.Name == member.Name);
                if (description == null)
                    return false;
            }

            return true;
        }

        private static void AppendMembersPath(List<NodeReference> nodes, IEnumerable<MemberInfo> memberChain, Type parameterType)
        {
            var description = EntityTypeDescriptor.GetTypeDescription(parameterType);

            foreach (var memberInfo in memberChain)
            {
                description = description.Columns.SingleOrDefault(d => d.ColumnInfo.Name == memberInfo.Name);

                foreach (var nodeReference in nodes)
                {
                    nodeReference.AppendSubscript(description.ColumnAttribute.Name ?? description.ColumnInfo.Name);
                }
            }
        }
    }
}
