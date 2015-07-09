using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionCaching;
using GlobalsFramework.Linq.ExpressionProcessing;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.Helpers
{
    internal static class CallProcessingHelper
    {
        internal static List<ProcessingResult> ProcessArguments(IEnumerable<Expression> argumentExpressions, List<NodeReference> references)
        {
            var result = new List<ProcessingResult>();

            foreach (var argumentExp in argumentExpressions)
            {
                if (argumentExp.NodeType == ExpressionType.Lambda)
                {
                    try
                    {
                        var compiledLambda = CompiledExpressionStorage.GetOrAddCompiledLambda(argumentExp as LambdaExpression);
                        result.Add(new ProcessingResult(true, compiledLambda, true));
                    }
                    //exception is thrown where lambda contains references to variable from udefined scope
                    catch (InvalidOperationException)
                    {
                        result.Add(ProcessingResult.Unsuccessful);
                    }
                    
                    continue;
                }

                var processingResult = PredicateExpressionProcessor.ProcessExpression(argumentExp, references);
                if (!processingResult.IsSuccess)
                {
                    result.Add(ProcessingResult.Unsuccessful);
                    return result;
                }

                var argumentResult = processingResult.IsSingleItem
                    ? new ProcessingResult(true, processingResult.GetLoadedItem(argumentExp.Type), true)
                    : new ProcessingResult(true, processingResult.GetLoadedItems(argumentExp.Type));
                result.Add(argumentResult);
            }

            return result;
        }

        internal static ProcessingResult ProcessCall(List<ProcessingResult> argumentResults, ProcessingResult targetResult,
            Func<object, object[], ProcessingResult> resolver)
        {
            if (argumentResults.All(a => a.IsSingleItem) && targetResult.IsSingleItem)
            {
                var result = resolver(targetResult.Result, argumentResults.Select(a => a.Result).ToArray());
                return !result.IsSuccess
                    ? ProcessingResult.Unsuccessful
                    : new ProcessingResult(true, result.Result, true);
            }

            var enumerators = new List<ProcessingResult> { targetResult }
                .Concat(argumentResults)
                .Select(item => new
                {
                    Enumerator = item.IsSingleItem ? null : item.GetItems().GetEnumerator(),
                    IsSingle = item.IsSingleItem,
                    Value = item.Result
                })
                .ToList();

            var resultsList = new List<object>();
            var canMoveNext = enumerators.Where(e => !e.IsSingle).All(e => e.Enumerator.MoveNext());

            while (canMoveNext)
            {
                var values = enumerators.Select(e => e.IsSingle ? e.Value : e.Enumerator.Current).ToList();
                var objectValue = values.First();
                var argumentsValues = values.Skip(1).ToArray();

                var result = resolver(objectValue, argumentsValues);
                if (!result.IsSuccess)
                    return ProcessingResult.Unsuccessful;

                resultsList.Add(result.Result);
                canMoveNext = enumerators.Where(e => !e.IsSingle).All(e => e.Enumerator.MoveNext());
            }

            return new ProcessingResult(true, resultsList);
        }
    }
}
