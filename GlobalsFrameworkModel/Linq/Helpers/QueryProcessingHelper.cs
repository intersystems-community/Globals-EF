using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GlobalsFramework.Access;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.QueryProcessing;
using GlobalsFramework.Utils.TypeDescription;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.Helpers
{
    internal static class QueryProcessingHelper
    {
        private static readonly List<IQueryProcessor> QueryProcessors;

        static QueryProcessingHelper()
        {
            QueryProcessors = new List<IQueryProcessor>
            {
                new SelectQueryProcessor(),
                new WhereQueryProcessor(),
                new CountQueryProcessor(),
                new FirstQueryProcessor(),
                new ElementAtQueryProcessor(),
                new SingleQueryProcessor(),
                new LastQueryProcessor(),
                new AllQueryProcessor()
            };
        }

        internal static object ProcessQueries(NodeReference node, DataContext context, Expression queryExpression)
        {
            var queries = GetQueriesInTurn(queryExpression);
            var nodes = DatabaseManager.GetEntitiesNodes(node, context.GetConnection());

            var result = new ProcessingResult(true, nodes);
            result = queries.Aggregate(result, (current, query) => ProcessQuery(query, current));

            if (result.IsDeferred())
            {
                return result.IsSingleItem
                    ? result.GetLoadedItem(GetReturnParameterType(queries.Last()))
                    : result.GetLoadedItems(GetReturnParameterType(queries.Last()));
            }

            return result.Result;
        }

        internal static Type GetReturnParameterType(MethodCallExpression query)
        {
            var returnParameter = query.Method.ReturnParameter;

            if (returnParameter == null)
                throw new InvalidOperationException("Unable to perform operation");

            if (EntityTypeDescriptor.IsNullableType(returnParameter.ParameterType))
                return returnParameter.ParameterType;

            return returnParameter.ParameterType.IsGenericType
                ? returnParameter.ParameterType.GetGenericArguments().Single()
                : returnParameter.ParameterType;
        }

        internal static ProcessingResult ProcessSingleResultQuery(MethodCallExpression query, ProcessingResult parentResult,
            Func<IEnumerator, Type, ProcessingResult> queryResolver)
        {
            var hasPredicate = query.Arguments.Count == 2;

            if (!hasPredicate)
                return queryResolver(parentResult.GetItems().GetEnumerator(), GetReturnParameterType(query));

            if (!parentResult.IsDeferred())
                return ProcessingResult.Unsuccessful;

            var predicate = query.Arguments[1];
            var predicateResult = ExpressionProcessingHelper.ProcessPredicate(predicate, parentResult.GetDeferredItems());

            return predicateResult.IsSuccess
                ? queryResolver(predicateResult.GetItems().GetEnumerator(), GetReturnParameterType(query))
                : ProcessingResult.Unsuccessful;
        }

        private static ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            foreach (var queryProcessor in QueryProcessors)
            {
                if (queryProcessor.CanProcess(query))
                {
                    var result = queryProcessor.ProcessQuery(query, parentResult);

                    return result.IsSuccess
                        ? result
                        : ProcessQueryByDefault(query, parentResult);
                }
            }

            return ProcessQueryByDefault(query, parentResult);
        }

        private static ProcessingResult ProcessQueryByDefault(MethodCallExpression query, ProcessingResult parentResult)
        {
            var result = InvokeQuery(query, parentResult.GetLoadedItems(GetSourceParameterType(query)));
            return new ProcessingResult(true, result);
        }

        private static object InvokeQuery(MethodCallExpression query, object sourse)
        {
            //source parameter always at first place
            var methodParameters = query.Method.GetParameters().Skip(1).ToList();
            var invocationParameters = new List<object>(methodParameters.Count()) {((IEnumerable) sourse).AsQueryable()};

            foreach (var methodParameter in methodParameters)
            {
                var expressionArgument = query.Arguments.Single(arg => arg.Type == methodParameter.ParameterType);

                if (expressionArgument is UnaryExpression)
                    invocationParameters.Add((expressionArgument as UnaryExpression).Operand);
                else
                    invocationParameters.Add(((ConstantExpression) expressionArgument).Value);
            }
            try
            {
                return query.Method.Invoke(null, invocationParameters.ToArray());
            }
                //First, Single or Last expressions can throw exception
            catch(Exception e)
            {
                //TargetInvocationException must be displayed
                throw e.InnerException;
            }
        }

        private static List<MethodCallExpression> GetQueriesInTurn(Expression query)
        {
            var result = new List<MethodCallExpression>();

            var mcall = query as MethodCallExpression;

            while (mcall != null && mcall.Arguments.Count > 0)
            {
                result.Add(mcall);
                mcall = mcall.Arguments[0] as MethodCallExpression;
            }

            result.Reverse();

            return result;
        }

        private static Type GetSourceParameterType(MethodCallExpression query)
        {
            //source parameter always at first place
            var sourceParameter = query.Method.GetParameters().First();

            return sourceParameter.ParameterType.IsGenericType
                ? sourceParameter.ParameterType.GetGenericArguments().Single()
                : sourceParameter.ParameterType;
        }

    }
}
