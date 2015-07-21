using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal sealed class SelectQueryProcessor : IQueryProcessor
    {
        public bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "Select";
        }

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            if (!parentResult.IsDeferred())
                return ProcessingResult.Unsuccessful;

            var unaryExpression = (UnaryExpression)query.Arguments[1];
            var lambdaExpression = (LambdaExpression)unaryExpression.Operand;

            var memberExpression = lambdaExpression.Body;
            var nodeReferences = (List<NodeReference>) parentResult.Result;

            var result = ExpressionProcessingHelper.ProcessExpression(memberExpression, nodeReferences);

            if (!result.IsSuccess)
                return ProcessingResult.Unsuccessful;

            if (!result.IsSingleItem)
                return NormalizeResult(result, query);

            var multipleResult = ExpressionProcessingHelper.CopyInstances(result, nodeReferences.Count,
                () => ExpressionProcessingHelper.ProcessExpression(memberExpression, nodeReferences));

            return new ProcessingResult(true, NormalizeResult(multipleResult, query).Result);
        }

        private static ProcessingResult NormalizeResult(ProcessingResult result, MethodCallExpression query)
        {
            if (result.IsDeferred())
                return result;

            var sourceItems = result.GetItems();
            var resultList = (IList) Activator.CreateInstance(
                typeof (List<>).MakeGenericType(QueryProcessingHelper.GetReturnParameterType(query)));

            foreach (var sourceItem in sourceItems)
            {
                resultList.Add(sourceItem);
            }

            return new ProcessingResult(true, resultList);
        }
    }
}
