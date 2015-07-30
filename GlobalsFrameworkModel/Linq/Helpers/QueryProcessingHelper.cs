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
                new AllQueryProcessor(),
                new AverageQueryProcessor(),
                new ConcatQueryProcessor(),
                new ContainsQueryProcessor(),
                new DefaultIfEmptyProcessor(),
                new OrderByQueryProcessor(),
                new DistinctQueryProcessor(),
                new SequencesComparisonQueryProcessor()
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

            var parentType = query.Arguments[0].Type;
            return ResolveParameterType(returnParameter.ParameterType, parentType);
        }

        internal static Type GetSourceParameterType(MethodCallExpression query)
        {
            //source parameter always at first place
            var sourceParameter = query.Method.GetParameters().First();
            var sourceType = sourceParameter.ParameterType;
            var parentType = query.Arguments[0].Type;

            return ResolveParameterType(sourceType, parentType);
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

        internal static ProcessingResult NormalizeMultipleResult(ProcessingResult result, Type targetItemType)
        {
            if (result.IsDeferred())
                return result;

            var sourceItems = result.GetItems();
            var resultList = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(targetItemType));

            foreach (var sourceItem in sourceItems)
            {
                resultList.Add(sourceItem);
            }

            return new ProcessingResult(true, resultList);
        }

        internal static ProcessingResult ProcessQueryByDefault(MethodCallExpression query, ProcessingResult parentResult)
        {
            var result = InvokeQuery(query, parentResult.GetLoadedItems(GetSourceParameterType(query)));
            return new ProcessingResult(true, result);
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

        private static object InvokeQuery(MethodCallExpression query, object sourse)
        {
            var invocationParameters = new List<object>{((IEnumerable) sourse).AsQueryable()};
            invocationParameters.AddRange(query.Arguments.Skip(1).Select(ResolveQueryArgument));

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

        private static object ResolveQueryArgument(Expression argumentExpression)
        {
            var unaryExpression = argumentExpression as UnaryExpression;
            if (unaryExpression != null)
                return unaryExpression.Operand;

            var constantExpression = argumentExpression as ConstantExpression;
            if (constantExpression != null)
                return constantExpression.Value;

            var callExpression = argumentExpression as MethodCallExpression;
            if (callExpression != null)
            {
                return ProcessQuery(callExpression,
                    new ProcessingResult(true, ResolveQueryArgument(callExpression.Arguments[0]))).Result;
            }

            throw new NotSupportedException("Unable to process query argument");
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

        private static Type ResolveParameterType(Type type, Type parentType)
        {
            if (EntityTypeDescriptor.IsNullableType(type))
                return type;

            if (type == typeof (IQueryable))
                return parentType.GetGenericArguments().First();

            var genericArguments = type.GetGenericArguments();

            if (genericArguments.Length == 1)
                return genericArguments.Single();

            return type;
        }
    }
}
