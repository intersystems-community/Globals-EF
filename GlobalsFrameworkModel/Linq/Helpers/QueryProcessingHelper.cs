using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GlobalsFramework.Access;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.QueryProcessing;
using GlobalsFramework.Linq.QueryProcessing.ConditionalLimitQueries;
using GlobalsFramework.Linq.QueryProcessing.MathQueries;
using GlobalsFramework.Linq.QueryProcessing.OrderingQueries;
using GlobalsFramework.Linq.QueryProcessing.SequenceComparisonQueries;
using GlobalsFramework.Linq.QueryProcessing.SingleResultQueries;
using GlobalsFramework.Utils.TypeDescription;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.Helpers
{
    internal static class QueryProcessingHelper
    {
        private static readonly List<IQueryProcessor> QueryProcessors;

        static QueryProcessingHelper()
        {
            QueryProcessors = new List<IQueryProcessor>();
            InitializeQueryProcessors(QueryProcessors);
        }

        internal static object ProcessQueries(NodeReference node, DataContext context, Expression queryExpression)
        {
            var queries = GetQueriesInTurn(queryExpression);
            var nodes = DatabaseManager.GetEntitiesNodes(node, context);

            var result = new ProcessingResult(true, nodes);
            result = queries.Aggregate(result, (current, query) => ProcessQuery(query, current, context));

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

        internal static ProcessingResult ProcessQueryByDefault(MethodCallExpression query, ProcessingResult parentResult, DataContext context)
        {
            var result = InvokeQuery(query, parentResult.GetLoadedItems(GetSourceParameterType(query)), context);
            return new ProcessingResult(true, result);
        }

        private static ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult, DataContext context)
        {
            foreach (var queryProcessor in QueryProcessors)
            {
                if (queryProcessor.CanProcess(query))
                {
                    var result = queryProcessor.ProcessQuery(query, parentResult, context);

                    return result.IsSuccess
                        ? result
                        : ProcessQueryByDefault(query, parentResult, context);
                }
            }

            return ProcessQueryByDefault(query, parentResult, context);
        }

        private static object InvokeQuery(MethodCallExpression query, object sourse, DataContext context)
        {
            var invocationParameters = new List<object>{((IEnumerable) sourse).AsQueryable()};
            invocationParameters.AddRange(query.Arguments.Skip(1).Select(arg=>ResolveQueryArgument(arg, context)));

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

        private static object ResolveQueryArgument(Expression argumentExpression, DataContext context)
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
                    new ProcessingResult(true, ResolveQueryArgument(callExpression.Arguments[0], context)), context).Result;
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

        private static void InitializeQueryProcessors(ICollection<IQueryProcessor> queryProcessors)
        {
            queryProcessors.Add(new AverageQueryProcessor());
            queryProcessors.Add(new MaxQueryProcessor());
            queryProcessors.Add(new MinQueryProcessor());
            queryProcessors.Add(new SumQueryProcessor());

            queryProcessors.Add(new ExceptQueryProcessor());
            queryProcessors.Add(new IntersectQueryProcessor());
            queryProcessors.Add(new SequenceEqualQueryProcessor());
            queryProcessors.Add(new UnionQueryProcessor());

            queryProcessors.Add(new OrderByQueryProcessor());
            queryProcessors.Add(new ThenByQueryProcessor());

            queryProcessors.Add(new AnyQueryProcessor());
            queryProcessors.Add(new CountQueryProcessor());
            queryProcessors.Add(new LongCountQueryProcessor());
            queryProcessors.Add(new FirstOrDefaultQueryProcessor());
            queryProcessors.Add(new FirstQueryProcessor());
            queryProcessors.Add(new LastOrDefaultQueryProcessor());
            queryProcessors.Add(new LastQueryProcessor());
            queryProcessors.Add(new SingleOrDefaultQueryProcessor());
            queryProcessors.Add(new SingleQueryProcessor());

            queryProcessors.Add(new SelectQueryProcessor());
            queryProcessors.Add(new WhereQueryProcessor());

            queryProcessors.Add(new ElementAtQueryProcessor());
            queryProcessors.Add(new ElementAtOrDefaultQueryProcessor());

            queryProcessors.Add(new ConcatQueryProcessor());
            queryProcessors.Add(new ContainsQueryProcessor());
            queryProcessors.Add(new DefaultIfEmptyProcessor());
            queryProcessors.Add(new DistinctQueryProcessor());
            queryProcessors.Add(new AllQueryProcessor());
            queryProcessors.Add(new ReverseQueryProcessor());

            queryProcessors.Add(new SkipQueryProcessor());
            queryProcessors.Add(new TakeQueryProcessor());

            queryProcessors.Add(new SkipWhileQueryProcessor());
            queryProcessors.Add(new TakeWhileQueryProcessor());
        }
    }
}
