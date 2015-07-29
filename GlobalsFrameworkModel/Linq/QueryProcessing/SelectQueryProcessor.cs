using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;

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

            if (lambdaExpression.Parameters.Count > 1)
                return ProcessingResult.Unsuccessful;

            var memberExpression = lambdaExpression.Body;
            var nodeReferences = parentResult.GetDeferredList();

            var result = ExpressionProcessingHelper.ProcessExpression(memberExpression, nodeReferences);

            if (!result.IsSuccess)
                return ProcessingResult.Unsuccessful;

            var targetItemType = QueryProcessingHelper.GetReturnParameterType(query);

            if (!result.IsSingleItem)
                return QueryProcessingHelper.NormalizeMultipleResult(result, targetItemType);

            var multipleResult = ExpressionProcessingHelper.CopyInstances(result, nodeReferences.Count,
                () => ExpressionProcessingHelper.ProcessExpression(memberExpression, nodeReferences).Result);

            return new ProcessingResult(true, QueryProcessingHelper.NormalizeMultipleResult(multipleResult, targetItemType).Result);
        }
    }
}
