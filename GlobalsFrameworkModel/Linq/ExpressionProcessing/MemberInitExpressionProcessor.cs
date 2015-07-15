using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GlobalsFramework.Linq.Helpers;
using GlobalsFramework.Linq.MemberBindingEvaluation;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.ExpressionProcessing
{
    internal sealed class MemberInitExpressionProcessor : IExpressionProcessor
    {
        public bool CanProcess(Expression expression)
        {
            return expression.NodeType == ExpressionType.MemberInit;
        }

        public ProcessingResult ProcessExpression(Expression expression, List<NodeReference> references)
        {
            var memberInitExpression = expression as MemberInitExpression;
            if (memberInitExpression == null)
                return ProcessingResult.Unsuccessful;

            var instanceResult = ExpressionProcessingHelper.ProcessExpression(memberInitExpression.NewExpression, references);
            if (!instanceResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            var evaluatedBindings = MemberBindingEvaluator.EvaluateBindings(memberInitExpression.Bindings.ToList(), references);

            if (evaluatedBindings.Any(b => !b.IsSuccess))
                return ProcessingResult.Unsuccessful;

            if (instanceResult.IsSingleItem && evaluatedBindings.Any(b => !b.IsSingle))
            {
                instanceResult = ExpressionProcessingHelper.CopyInstances(instanceResult, references.Count,
                    () => ExpressionProcessingHelper.ProcessExpression(memberInitExpression.NewExpression, references).Result);
            }

            return MemberBindingProcessingHelper.ProcessBindings(evaluatedBindings, instanceResult);
        }
    }
}
