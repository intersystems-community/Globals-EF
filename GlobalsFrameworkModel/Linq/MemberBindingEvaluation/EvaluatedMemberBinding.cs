using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GlobalsFramework.Linq.ExpressionProcessing;

namespace GlobalsFramework.Linq.MemberBindingEvaluation
{
    internal sealed class EvaluatedMemberBinding
    {
        internal EvaluatedMemberBinding(MemberBindingType type, MemberInfo member)
        {
            BindingType = type;
            Member = member;

            IsSuccess = true;
            IsSingle = true;

            if (type == MemberBindingType.ListBinding)
                EvaluatedInitializers = new List<EvaluatedListInitializer>();

            if (type == MemberBindingType.MemberBinding)
                EvaluatedBindings = new List<EvaluatedMemberBinding>();
        }

        internal MemberBindingType BindingType { get; private set; }
        internal List<EvaluatedListInitializer> EvaluatedInitializers { get; private set; }
        internal List<EvaluatedMemberBinding> EvaluatedBindings { get; private set; }
        internal ProcessingResult Result { get; private set; }
        internal MemberInfo Member { get; private set; }

        internal bool IsSuccess { get; private set; }
        internal bool IsSingle { get; private set; }

        internal void AddResult(ProcessingResult result)
        {
            if (!result.IsSuccess)
                IsSuccess = false;

            if (!result.IsSingleItem)
                IsSingle = false;

            Result = result;
        }
        internal void AddInitializer(MethodInfo method, List<ProcessingResult> result)
        {
            if (result.Any(r => !r.IsSuccess))
                IsSuccess = false;

            if (result.Any(r => !r.IsSingleItem))
                IsSingle = false;

            EvaluatedInitializers.Add(new EvaluatedListInitializer(method, result));
        }
        internal void AddChildBinding(EvaluatedMemberBinding binding)
        {
            if (!binding.IsSuccess)
                IsSuccess = false;

            if (!binding.IsSingle)
                IsSingle = false;

            EvaluatedBindings.Add(binding);
        }

        internal void MarkUnsuccessful()
        {
            IsSuccess = false;
        }
    }
}
