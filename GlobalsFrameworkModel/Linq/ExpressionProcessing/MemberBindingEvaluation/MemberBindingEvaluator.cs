using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GlobalsFramework.Linq.Helpers;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.ExpressionProcessing.MemberBindingEvaluation
{
    internal static class MemberBindingEvaluator
    {
        internal static List<EvaluatedMemberBinding> EvaluateBindings(List<MemberBinding> bindings, List<NodeReference> references)
        {
            var result = new List<EvaluatedMemberBinding>();

            foreach (var memberBinding in bindings)
            {
                var item = new EvaluatedMemberBinding(memberBinding.BindingType, memberBinding.Member);

                switch (memberBinding.BindingType)
                {
                    case MemberBindingType.Assignment:
                        EvaluateMemeberAssignment(item, memberBinding as MemberAssignment, references);
                        break;
                    case MemberBindingType.ListBinding:
                        EvaluateListBinding(item, memberBinding as MemberListBinding, references);
                        break;
                    case MemberBindingType.MemberBinding:
                        EvaluateMemberBinding(item, memberBinding as MemberMemberBinding, references);
                        break;
                }

                result.Add(item);

                if (!item.IsSuccess)
                    return result;
            }

            return result;
        }

        private static void EvaluateMemeberAssignment(EvaluatedMemberBinding result, MemberAssignment assignment,
            List<NodeReference> references)
        {
            var processingResult = ExpressionProcessingHelper.ProcessExpression(assignment.Expression, references);
            result.AddResult(LoadData(processingResult, assignment.Expression.Type));
        }

        private static void EvaluateListBinding(EvaluatedMemberBinding result, MemberListBinding listBinding, List<NodeReference> references)
        {
            foreach (var initializer in listBinding.Initializers)
            {
                var processingResults = initializer.Arguments
                    .Select(a => LoadData(ExpressionProcessingHelper.ProcessExpression(a, references), a.Type))
                    .ToList();

                if (processingResults.Any(r => !r.IsSuccess))
                {
                    result.MarkUnsuccessful();
                    return;
                }

                result.AddInitializer(initializer.AddMethod, processingResults);
            }
        }

        private static void EvaluateMemberBinding(EvaluatedMemberBinding result, MemberMemberBinding memberBinding, List<NodeReference> references)
        {
            var childMemberBindings = EvaluateBindings(memberBinding.Bindings.ToList(), references);
            if (childMemberBindings.Any(b => !b.IsSuccess))
            {
                result.MarkUnsuccessful();
                return;
            }

            foreach (var evaluatedMemberBinding in childMemberBindings)
            {
                result.AddChildBinding(evaluatedMemberBinding);
            }
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
