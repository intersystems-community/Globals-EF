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

            var unaryExpression = query.Arguments[1] as UnaryExpression;
            if (unaryExpression == null)
                return ProcessingResult.Unsuccessful;

            var lambdaExpression = unaryExpression.Operand as LambdaExpression;
            if (lambdaExpression == null)
                return ProcessingResult.Unsuccessful;

            var memberExpression = lambdaExpression.Body;
            var nodeReferences = (List<NodeReference>) parentResult.Result;

            var result = ExpressionProcessingHelper.ProcessExpression(memberExpression, nodeReferences);

            if (!result.IsSuccess)
                return ProcessingResult.Unsuccessful;

            if (!result.IsSingleItem)
                return result;

            var multipleResult = ExpressionProcessingHelper.CopyInstances(result, nodeReferences.Count,
                () => ExpressionProcessingHelper.ProcessExpression(memberExpression, nodeReferences));

            return new ProcessingResult(true, multipleResult.Result);
        }
    }
}
