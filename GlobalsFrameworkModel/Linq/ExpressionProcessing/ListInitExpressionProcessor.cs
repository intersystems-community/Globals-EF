using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GlobalsFramework.Linq.Helpers;
using GlobalsFramework.Linq.MemberBindingEvaluation;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.ExpressionProcessing
{
    internal sealed class ListInitExpressionProcessor : IExpressionProcessor
    {
        public bool CanProcess(Expression expression)
        {
            return expression.NodeType == ExpressionType.ListInit;
        }

        public ProcessingResult ProcessExpression(Expression expression, List<NodeReference> references, DataContext context)
        {
            var listInitExpression = expression as ListInitExpression;
            if (listInitExpression == null)
                return ProcessingResult.Unsuccessful;

            var instanceResult = ExpressionProcessingHelper.ProcessExpression(listInitExpression.NewExpression, references, context);
            if (instanceResult == null)
                return ProcessingResult.Unsuccessful;

            var evaluatedInitializers = EvaluateInitializers(listInitExpression.Initializers, references, context);

            var isSuccess = evaluatedInitializers.All(i => i.Arguments.All(a => a.IsSuccess));
            if (!isSuccess)
                return ProcessingResult.Unsuccessful;

            var isSingleInitializers = evaluatedInitializers.All(i => i.Arguments.All(a => a.IsSingleItem));

            if (instanceResult.IsSingleItem && !isSingleInitializers)
            {
                instanceResult = ExpressionProcessingHelper.CopyInstances(instanceResult, references.Count,
                    () => ExpressionProcessingHelper.ProcessExpression(listInitExpression.NewExpression, references, context).Result);
            }

            instanceResult = evaluatedInitializers.Aggregate(instanceResult,
                (current, initializer) => MemberBindingProcessingHelper.ProcessListInitializer(initializer, current));

            return instanceResult;
        }

        private static List<EvaluatedListInitializer> EvaluateInitializers(IEnumerable<ElementInit> initializers,
            List<NodeReference> references, DataContext context)
        {
            var result = new List<EvaluatedListInitializer>();

            foreach (var initializer in initializers)
            {
                var processingResults = initializer.Arguments
                    .Select(a => LoadData(ExpressionProcessingHelper.ProcessExpression(a, references, context), a.Type))
                    .ToList();

                result.Add(new EvaluatedListInitializer(initializer.AddMethod, processingResults));

                if (processingResults.Any(r => !r.IsSuccess))
                    break;
            }

            return result;
        }

        private static ProcessingResult LoadData(ProcessingResult result, Type dataType)
        {
            if (!result.IsSuccess)
                return ProcessingResult.Unsuccessful;

            var loadedResult = result.IsSingleItem
                ? result.GetLoadedItem(dataType)
                : result.GetLoadedItems(dataType);

            return new ProcessingResult(true, loadedResult, result.IsSingleItem);
        }
    }
}
